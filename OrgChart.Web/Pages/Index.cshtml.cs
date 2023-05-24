using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrgChart.Business;
using System.DirectoryServices.AccountManagement;

namespace OrgChart.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHierarchyManager _hierarchyManager;
        public List<HierarchyModel> Employees { get; set; }

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        public IndexModel(IHierarchyManager hierarchyManager)
        {
            _hierarchyManager = hierarchyManager;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Locations/Index");
        }
        public JsonResult OnGetEmployeeData()
        {
            var hierarchy_tree = _hierarchyManager.HierarchyFlatTreeV2();
            string image_path = @"/photos/";

            Employees = hierarchy_tree.Where(x => x != null).Select(s => new HierarchyModel
            {
                Id = s.Id,
                ParentId = s.Parent == null ? 0 : s.Parent.Id,
                Description = s.Description,
                Duplicate = s.Duplicate,
                Level = s.Level,
                Sequence = s.Sequence,
                Type = s.Type,                
                Employee = s.Employee.Id != 0 ? new EmployeeViewModel
                {
                    Id = s.Employee.Id,
                    FirstName = s.Employee.FirstName,
                    LastName = s.Employee.LastName,
                    //ImageUrl = s.Employee.ImageUrl == null ? "defaultProfilePicture.png" : s.Employee.ImageUrl.Split("\\")[4],
                    ImageUrl = s.Employee.ImageUrl.Split("\\").Length == 5 ? image_path + s.Employee.ImageUrl.Split("\\")[4] : @"/images/" + "defaultProfilePicture.png",
                    Position = s.Employee.Position,
                    //ImageUrl = @"/images/" + "defaultProfilePicture.png"
                    HireDate = s.Employee.HireDate,
                    str_HireDate = s.Employee.str_HireDate
                } : new EmployeeViewModel { ImageUrl = "defaultProfilePicture.png" }
            }).ToList();

            return new JsonResult(Employees);
        }
        public JsonResult OnGetSelectEmployee()
        {            
            var temp_employee = _hierarchyManager.Employees.FirstOrDefault(x => x.Id == EmployeeId);
            EmployeeViewModel results = null!;
            if (temp_employee != null)
            {
                string image_path = @"/photos/";
                var employee = new EmployeeViewModel
                {
                    Id = temp_employee.Id,
                    FirstName = temp_employee.FirstName,
                    LastName = temp_employee.LastName,
                    Position = temp_employee.Position,
                    //ImageUrl = @"/images/" + "defaultProfilePicture.png",
                    ImageUrl = temp_employee.ImageUrl.Split("\\").Length == 5 ? image_path + temp_employee.ImageUrl.Split("\\")[4] : @"/images/" + "defaultProfilePicture.png",
                    DepartmentId = temp_employee.Department.DepartmentId,
                    DepartmentName = temp_employee.Department.SubDepartmentName,
                    Number = temp_employee.Number,
                    HireDate = temp_employee.HireDate,
                    str_HireDate = temp_employee.str_HireDate
                };

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    var ad_user = UserPrincipal.FindByIdentity(pc, IdentityType.Name, employee.FullName);

                    if (ad_user != null) 
                    {
                        employee.AD = new ActiveDirectoryAccount
                        { 
                            UserName = ad_user.SamAccountName,
                            Email = ad_user.EmailAddress
                        };
                    }
                }

                results = employee;
            }

            return new JsonResult(results);
        }
    }
    public class HierarchyModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public EHierarchyType Type { get; set; }
        public string Description { get; set; } = null!;
        public EmployeeViewModel Employee { get; set; } = null!;
        public int Level { get; set; }
        public int Sequence { get; set; }
        public bool Duplicate { get; set; }
    }
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string Position { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string Number { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string str_HireDate { get; set; } = null!;
        public ActiveDirectoryAccount AD { get; set; } = null!;
    }
    public class ActiveDirectoryAccount
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}