using System.ComponentModel.DataAnnotations;

namespace GestionDeArchivos.Data.Entities
{
    public class Areas
    {
        public int Id { get; set; }

        [Display(Name = "Nombre área")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public int DocumentNumberArea => Documents == null ? 0 : Documents.Count;

    }
}
