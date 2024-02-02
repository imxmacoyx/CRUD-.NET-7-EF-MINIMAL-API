using System;
using System.ComponentModel.DataAnnotations;

namespace demo_app.Dtos
{
	public class PersonaCreationDto
	{
        [Required]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public required string Nombre { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "La edad debe ser un número válido.")]
        public required int Edad { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

    }
}

