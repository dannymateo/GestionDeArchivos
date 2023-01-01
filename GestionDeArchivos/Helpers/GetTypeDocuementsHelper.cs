using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Helpers
{
    public class GetTypeDocumentsHelper: IGetTypeDocuementsHelper
    {
        private readonly DataContext _context;
        public GetTypeDocumentsHelper(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetTypeDocuementsAsync()
        {
            List<DocumentType> list;

            list = await _context.DocumentTypes.Select(a => new DocumentType
            {
                Id = a.Id,
                Name = a.Name
            })
                .OrderBy(a => a.Name)
                .ToListAsync();

            List<SelectListItem> items = list.ConvertAll(l =>
            {
                return new SelectListItem()
                {
                    Text = l.Name,
                    Value = l.Name,
                    Selected = false
                };
            });
            items.Insert(0, new SelectListItem { Text = "Seleccione una opción", Value = "" });
            return items;
        }
    }
}
