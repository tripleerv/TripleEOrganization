using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OrgChart.Web.Pages.Locations
{
    public class StatisticsModel : PageModel
    {
        private readonly Business.IBistrainerManager _bistrainerManager;

        public StatisticsModel(Business.IBistrainerManager bistrainerManager)
        {
            _bistrainerManager = bistrainerManager;
        }
        public void OnGet()
        {
            var departments = _bistrainerManager.Departments();
        }
    }
}
