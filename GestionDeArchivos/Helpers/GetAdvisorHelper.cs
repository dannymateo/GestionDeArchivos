using GestionDeArchivos.Data;
using GestionDeArchivos.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Helpers
{
    public class GetAdvisorHelper : IGetAdvisorHelper
    {
        private readonly DataContext _context;
        public GetAdvisorHelper(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetAdvisorsAsync()
        {
            List<Advisor> list;

            Advisor selectOption = new Advisor { Name = "Seleccione una opción" };

            list = await _context.Advisors.Select(a => new Advisor
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
