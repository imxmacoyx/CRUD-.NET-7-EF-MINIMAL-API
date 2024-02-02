using System;
using System.ComponentModel.DataAnnotations;

namespace demo_app.Dtos
{
	public class PersonaUpdateDto
	{
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public  string? Nombre { get; set; }

        [Range(0, 120, ErrorMessage = "La edad debe ser un número válido.")]
        public  int? Edad { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public  string? Email { get; set; }
    }
}

