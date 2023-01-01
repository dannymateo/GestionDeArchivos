using System.ComponentModel.DataAnnotations;

namespace GestionDeArchivos.Models
{
    public class AddDocumentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre documento")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [Display(Name = "Comentario")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Remark { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Date { get; set; }

        [Display(Name = "Estado")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string DocumentStatus { get; set; }
        [Display(Name = "Area")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Areas { get; set; }
        [Display(Name = "Tipo de documento")]
        [Required(ErrorMessage = "La lista {0} es obligatoria.")]
        public string TypeDocuments { get; set; }
        public string? Advisor { get; set; }

    }
}
