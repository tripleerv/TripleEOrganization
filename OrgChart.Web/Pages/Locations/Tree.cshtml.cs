using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrgChart.Business;

namespace OrgChart.Web.Pages.Locations
{
    public class TreeModel : PageModel
    {
        private readonly OrgChart.Business.IBistrainerManager _bistrainerManager;
        private readonly AutoMapper.IMapper _mapper;

        [BindProperty(SupportsGet = true)]
        public int LocationId { get; set; }

        public ParentLocationViewModel ParentLocation { get; set; } = null!;

        public TreeModel(IBistrainerManager bistrainerManager, IMapper mapper)
        {
            _bistrainerManager = bistrainerManager;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            
            return Page();
        }

        public JsonResult OnGetLocationTree()
        {
            return new JsonResult(_bistrainerManager.LocationFlatTree(LocationId).Where(x => x != null && x.User != null).Select(s => _mapper.Map<LocationViewModel>(s)).ToList());
        }
    }
}
