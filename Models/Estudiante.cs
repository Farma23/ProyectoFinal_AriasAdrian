using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal_AriasAdrian.Models
{
    public class Estudiante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(20)]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; } = DateTime.Today;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Llave foránea Carrera
        [Required(ErrorMessage = "Debe seleccionar una carrera.")]
        [Display(Name = "Carrera")]
        public int CarreraId { get; set; }

        [ForeignKey("CarreraId")]
        public Carrera? Carrera { get; set; }

        // Relación con Identity User
        public string? UserId { get; set; }

        // Navegación
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
