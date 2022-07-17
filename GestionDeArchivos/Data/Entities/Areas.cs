using System.ComponentModel.DataAnnotations;

namespace GestionDeArchivos.Data.Entities
{
    public class Areas
    {
        public int id { get; set; }

        [Display(Name = "Nombre Archivo")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string name { get; set; }
        public List<Document>? Documents { get; set; }

    }
}
