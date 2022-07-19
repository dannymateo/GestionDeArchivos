using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeArchivos.Data.Entities
{
    public class Document
    {
        public int Id { get; set; }

        [Display(Name = "Nombre Documento")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [Display(Name = "Area")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Location { get; set; }
        public int? AreaId { get; set; }
        public int? UsuarioId { get; set; }
        [Display(Name = "Comentarios")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Remark { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Date { get; set; }

        [Display(Name = "Estado")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string DocumentStatus { get; set; }
        [Display(Name = "Usuario")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? User { get; set; }
        [Display(Name = "Recibe")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? UserRecibes { get; set; }
    }
}
