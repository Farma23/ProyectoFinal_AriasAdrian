using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_AriasAdrian.Models
{
    public class Carrera
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre de la Carrera")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Semestres")]
        public int Semestres { get; set; }

        [Required]
        [Display(Name = "Créditos Totales")]
        public int CreditosTotales { get; set; }

        // Navegación
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    }
}
