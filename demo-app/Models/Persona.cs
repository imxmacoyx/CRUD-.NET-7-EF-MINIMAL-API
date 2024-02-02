using System;
using System.ComponentModel.DataAnnotations;

namespace demo_app.Models
{
	public class Persona
	{
        public int Id { get; set; }

        public required string Nombre { get; set; }

        public required int Edad { get; set; }

        public required string Email { get; set; }

        public bool Eliminado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

