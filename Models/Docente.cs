using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_AriasAdrian.Models
{
    public class Docente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Especialidad")]
        public string? Especialidad { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
