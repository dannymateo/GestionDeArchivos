using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Helpers
{
    public class GetAreasHelper: IGetAreasHelper
    {
        private readonly DataContext _context;
        public GetAreasHelper(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetAreasAsync()
        {
            List<Areas> list;

            Areas selectOption = new Areas { Name = "Seleccione una opción" };

            list = await _context.Areas.Select(a => new Areas
            {
                Id = a.Id,
                Name = a.Name
            })
                .OrderBy(a => a.Name)
                .ToListAsync();

            list.Insert(0, selectOption);
            List<SelectListItem> items = list.ConvertAll(l =>
            {
                return new SelectListItem()
                {
                    Text = l.Name,
                    Value = l.Name,
                    Selected = false
                };
            });
            return items;
        }
    }
}
