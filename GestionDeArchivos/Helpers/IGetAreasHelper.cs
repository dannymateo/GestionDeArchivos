using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionDeArchivos.Helpers
{
    public interface IGetAreasHelper
    {
        Task<IEnumerable<SelectListItem>> GetAreasAsync();
    }
}
