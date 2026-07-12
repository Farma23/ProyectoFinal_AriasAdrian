using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal_AriasAdrian.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Matrícula")]
        [DataType(DataType.Date)]
        public DateTime FechaMatricula { get; set; } = DateTime.Today;

        [Display(Name = "Estado")]
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Activa;

        // Llave foránea Estudiante
        [Required]
        [Display(Name = "Estudiante")]
        public int EstudianteId { get; set; }

        [ForeignKey("EstudianteId")]
        public Estudiante? Estudiante { get; set; }

        // Llave foránea Curso
        [Required]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso? Curso { get; set; }
    }

    public enum EstadoMatricula
    {
        Activa,
        Cancelada,
        Completada
    }
}
