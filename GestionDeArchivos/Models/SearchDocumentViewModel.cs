
using GestionDeArchivos.Data.Entities;

namespace GestionDeArchivos.Models
{
    public class SearchDocumentViewModel
    {
        public DateTime DateFirst { get; set; }
        public DateTime DateFinish { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
