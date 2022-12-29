using System.ComponentModel.DataAnnotations;

namespace GestionDeArchivos.Data.Entities
{
    public class Advisor
    {
        public int Id { get; set; }

        [Display(Name = "Nombre asesor")]
        [MaxLength(150, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }
        public ICollection<Document>? Documents { get; set; }
        [Display(Name = "# Documentos")]
        public int DocumentsNumber => Documents == null ? 0 : Documents.Count;
    }
}
