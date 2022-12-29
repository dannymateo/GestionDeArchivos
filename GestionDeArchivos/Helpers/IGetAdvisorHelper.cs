using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionDeArchivos.Helpers
{
    public interface IGetAdvisorHelper
    {
        Task<IEnumerable<SelectListItem>> GetAdvisorsAsync();

    }
}
