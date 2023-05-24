using AutoMapper;

namespace OrgChart.Web.AutoMapperConfig
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<OrgChart.Business.Models.BistrainerLocationModel, OrgChart.Web.Pages.Locations.LocationViewModel>().ReverseMap();
            CreateMap<OrgChart.Business.Models.BistrainerUserModel, OrgChart.Web.Pages.Locations.UserViewModel>().ReverseMap();
        }
    }
}
