using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeArchivos.Data.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Usuario")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; }

        [Display(Name = "Correo")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Correo { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Display(Name = "Rol")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]

        public string Roles { get; set; }

        public List<Document>? Documents { get; set; }
        [NotMapped]
        public int DocumentsNumberUser => Documents == null ? 0 : Documents.Count;

    }
}
