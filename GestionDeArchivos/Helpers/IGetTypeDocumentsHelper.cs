using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionDeArchivos.Helpers
{
    public interface IGetTypeDocuementsHelper
    {
        Task<IEnumerable<SelectListItem>> GetTypeDocuementsAsync();
    }
}
