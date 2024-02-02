using System.ComponentModel.DataAnnotations;
using System.Linq;
using demo_app.Data;
using demo_app.Dtos;
using demo_app.Models;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString)); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();


app.MapPost("/persona", async (AppDbContext context, PersonaCreationDto personaDto) =>
{

    var emailExists = await context.Personas.AnyAsync(p => p.Email == personaDto.Email);
    if (emailExists)
    {
        return Results.BadRequest(new { Error = "El correo electrónico ya está en uso." });
    }

    var utcNow = DateTime.UtcNow;
    var localTime = new DateTime(utcNow.Ticks, DateTimeKind.Unspecified);

    var persona = new Persona
    {
        Nombre = personaDto.Nombre,
        Edad = personaDto.Edad,
        Email = personaDto.Email,
        CreatedAt = localTime,
        UpdatedAt = localTime
    };

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(persona);
    if (!Validator.TryValidateObject(persona, validationContext, validationResults, true))
    {
        return Results.ValidationProblem(validationResults.ToDictionary(
            vr => vr.MemberNames.FirstOrDefault() ?? "",
            vr => new string[] { vr.ErrorMessage ?? "Un error de validación ocurrió." }
        ));
    }

    context.Personas.Add(persona);
    await context.SaveChangesAsync();
    return Results.Created($"/persona/{persona.Id}", persona);
});


app.MapGet("/personas", async (AppDbContext context) =>
    await context.Personas.Where(p => !p.Eliminado).ToListAsync());

app.MapGet("/persona/{id:int}", async (AppDbContext context, int id) =>
{
    var persona = await context.Personas.FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);
    return persona is not null ? Results.Ok(persona) : Results.NotFound();
});

app.MapGet("/personasPaginadas", async (AppDbContext context, int? page, int? pageSize) =>
{

    var pageNumber = (page.HasValue && page.Value > 0) ? page.Value : 1;
    var pageSizeNumber = (pageSize.HasValue && pageSize.Value > 0) ? pageSize.Value : 10;

    var totalRecords = await context.Personas.CountAsync();
    var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSizeNumber);
    var personas = await context.Personas
                                .Skip((pageNumber - 1) * pageSizeNumber)
                                .Take(pageSizeNumber)
                                .ToListAsync();

    return Results.Ok(new
    {
        TotalRecords = totalRecords,
        Page = page,
        PageSize = pageSize,
        TotalPages = totalPages,
        Data = personas
    });
});

app.MapPut("/persona/{id:int}", async (AppDbContext context, int id, PersonaUpdateDto personaDto) =>
{

    var persona = await context.Personas.FirstOrDefaultAsync(p => p.Id == id);
    if (persona is null || persona.Eliminado) return Results.NotFound();

    if (personaDto.Email != null && await context.Personas.AnyAsync(p => p.Email == personaDto.Email && p.Id != id))
    {
        return Results.BadRequest(new { Error = "El correo electrónico ya está en uso." });
    }

    var utcNow = DateTime.UtcNow;
    var localTime = new DateTime(utcNow.Ticks, DateTimeKind.Unspecified);

    if (personaDto.Nombre != null)
        persona.Nombre = personaDto.Nombre;

    if (personaDto.Edad.HasValue)
        persona.Edad = personaDto.Edad.Value;

    if (personaDto.Email != null)
        persona.Email = personaDto.Email;

    persona.UpdatedAt = localTime;
    await context.SaveChangesAsync();

    return Results.Ok(persona);
});


app.MapDelete("/persona/{id}", async (AppDbContext context, int id) =>
{
    var persona = await context.Personas.FirstOrDefaultAsync(p => p.Id == id);
    if (persona is null || persona.Eliminado) return Results.NotFound();

    persona.Eliminado = true;
    await context.SaveChangesAsync();

    return Results.Ok();
});


app.Run();
