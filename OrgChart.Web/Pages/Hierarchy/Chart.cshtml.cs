using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrgChart.Business;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OrgChart.Web.Pages.Hierarchy
{
    public class ChartModel : PageModel
    {
        private readonly IHierarchyManager _hierarchyManager;

        public List<Business.Models.HierarchyModel> Hierarchy { get; set; } = null!;
        public List<EmployeeViewModel> Employees { get; set; } = null!;
        [BindProperty(SupportsGet = true)]
        public string EmployeeSearch { get; set; } = null!;

        [BindProperty]
        public string CollapseState { get; set; } = null!;
        [TempData]
        public string t_CollapseState { get; set; } = null!;

        #region Adding New Node
        [BindProperty]
        public int AddType { get; set; }
        [BindProperty]
        public string AddDescription { get; set; } = null!;
        [BindProperty]
        public int AddEmployeeId { get; set; }
        [BindProperty]
        public int AddParentId { get; set; }
        #endregion

        #region Edit Node
        [BindProperty]
        public int EditId { get; set; }
        [BindProperty]
        public int EditType { get; set; }
        [BindProperty]
        public string EditDescription { get; set; } = null!;
        [BindProperty]
        public int EditEmployeeId { get; set; }
        #endregion

        #region Remove Node
        [BindProperty]
        public int RemoveId { get; set; }
        #endregion

        #region Change Parent
        [BindProperty]
        public int[] ChangeIds { get; set; } = null!;
        [BindProperty]
        public int ChangeParentId { get; set; }
        #endregion

        public ChartModel(IHierarchyManager hierarchyManager)
        {
            _hierarchyManager = hierarchyManager;
        }

        public IActionResult OnGet()
        {
            var h = _hierarchyManager.Hierarchies;
            var ee = _hierarchyManager.Employees;

            Employees = _hierarchyManager.Employees.Where(x => x.Active).OrderBy(o => o.FirstName).ThenBy(o => o.LastName).Select(s => new EmployeeViewModel
            {
                Id = s.Id,
                FullName = s.FullName,
                Number = s.Number,
                Position = s.Position,
                Department = s.Department.SubDepartmentName,
                Active = s.Active,
                HierarchyId = h.Any(x => x.EmployeeId == s.Id) ? h.FirstOrDefault(x => x.EmployeeId == s.Id).Id : 0
            }).ToList();

            Hierarchy = _hierarchyManager.HierarchyTree;

            return Page();
        }
        public IActionResult OnPostAdd()
        {
            var add = _hierarchyManager.Add((EHierarchyType)AddType, AddDescription, AddEmployeeId, AddParentId);

            if (add == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Description Cannot Be Blank" });

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Added", Id = add.Id, EmployeeFullName = add.Employee.FullName, EmployeeId = add.Employee.Id, Type = (int)add.Type, Description = add.Description });
        }
        public IActionResult OnPostEdit()
        {
            var update = _hierarchyManager.Update(EditId, (EHierarchyType)EditType, EditDescription, EditEmployeeId);

            if (update == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Description Cannot Be Blank" });

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Updated", Id = update.Id, EmployeeFullName = update.Employee.FullName, EmployeeId = update.Employee.Id, Type = (int)update.Type, Description = update.Description });
        }
        public IActionResult OnPostRemove()
        {
            if (_hierarchyManager.HasChildren(RemoveId))
            {
                return new JsonResult(new Response { Status = "Fail", Message = "Cannot Remove, There Are Child Elements" });
            }

            var h = _hierarchyManager.Hierarchies.FirstOrDefault(x => x.Id == RemoveId);
            if (h == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Hierarchy Not Found" });

            bool remove = _hierarchyManager.Remove(RemoveId);

            if (!remove)
                return new JsonResult(new Response { Status = "Fail", Message = "Hierarchy Was Not Removed" });

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Removed", Id = h.Id, Type = h.Type, Description = h.Description, EmployeeId = h.EmployeeId, EmployeeFullName = "" });
        }
        public IActionResult OnPostChangeParent()
        {
            Business.Models.HierarchyModel update = new Business.Models.HierarchyModel();

            foreach (int i in ChangeIds)
            {
                update = _hierarchyManager.ChangeParent(i, ChangeParentId);
            }

            if (update == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Hierarchy Not Updated" });

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Updated" });
        }
        public IActionResult OnPostMoveUp()
        {
            t_CollapseState = CollapseState;
            var move = _hierarchyManager.Hierarchies.FirstOrDefault(x => x.Id == EditId);

            if (move == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Hierarchy did not move" });

            _hierarchyManager.MoveUp(move.Id);

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Updated" });
        }
        public IActionResult OnPostMoveDown()
        {
            t_CollapseState = CollapseState;
            var move = _hierarchyManager.Hierarchies.FirstOrDefault(x => x.Id == EditId);

            if (move == null)
                return new JsonResult(new Response { Status = "Fail", Message = "Hierarchy did not move" });

            _hierarchyManager.MoveDown(move.Id);

            return new JsonResult(new Response { Status = "Success", Message = "Hierarchy Updated" });
        }
                 
        public JsonResult OnGetEmployeeList()
        {
            int count = 1;
            var employees = _hierarchyManager.Employees.Where(x => x.Active).OrderBy(o => o.FirstName).ToList();
            var hierarchies = _hierarchyManager.Hierarchies;

            return new JsonResult(employees.Select(s => new
            {
                Count = count++,
                Id = s.Id,
                FullName = s.FullName,
                Number = s.Number,
                Position = s.Position,
                Department = s.Department.SubDepartmentName,
                Disabled = hierarchies.Any(x => x.Id == s.Id)
            }));
        }
        public JsonResult OnGetEmployeeSearch()
        {
            var employees = _hierarchyManager.Employees.Where(x => x.Active).OrderBy(o => o.FirstName).ThenBy(o => o.LastName).ToList();

            if (!string.IsNullOrEmpty(EmployeeSearch))
            {
                if (EmployeeSearch.All(char.IsDigit))
                {
                    employees = employees.Where(x => x.Number.Contains(EmployeeSearch)).ToList();
                }
                else
                {
                    EmployeeSearch = EmployeeSearch.ToUpper();
                    employees = employees.Where(x => x.FullName.ToUpper().Contains(EmployeeSearch) || x.Position.ToUpper().Contains(EmployeeSearch)).ToList();
                }
            }

            var hierarchies = _hierarchyManager.Hierarchies;

            return new JsonResult(employees.Select(s => new EmployeeViewModel
            {
                Id = s.Id,
                FullName = s.FullName,
                Number = s.Number,
                Position = s.Position,
                Department = s.Department.SubDepartmentName,
                HierarchyId = hierarchies.Any(x => x.EmployeeId == s.Id) ? hierarchies.FirstOrDefault(x => x.EmployeeId == s.Id).Id : 0
            }));
        }
    }
}
