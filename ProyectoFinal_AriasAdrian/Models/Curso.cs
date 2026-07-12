using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal_AriasAdrian.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Créditos")]
        [Range(1, 10)]
        public int Creditos { get; set; }

        [Display(Name = "Cupos Disponibles")]
        public int CuposDisponibles { get; set; } = 30;

        [Display(Name = "Período")]
        [StringLength(20)]
        public string Periodo { get; set; } = "2025-I";

        // Llave foránea Carrera
        [Required(ErrorMessage = "Debe seleccionar una carrera.")]
        [Display(Name = "Carrera")]
        public int CarreraId { get; set; }

        [ForeignKey("CarreraId")]
        public Carrera? Carrera { get; set; }

        // Llave foránea Docente
        [Display(Name = "Docente")]
        public int? DocenteId { get; set; }

        [ForeignKey("DocenteId")]
        public Docente? Docente { get; set; }

        // Navegación
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
