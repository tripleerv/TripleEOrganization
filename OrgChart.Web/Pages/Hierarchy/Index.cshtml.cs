using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrgChart.Business;
using System.Linq.Expressions;

namespace OrgChart.Web.Pages.Hierarchy
{
    public class IndexModel : PageModel
    {
        private readonly IHierarchyManager _hierarchyManager;
        private readonly IBistrainerManager _bistrainerManager;

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

        public IndexModel(IHierarchyManager hierarchyManager, IBistrainerManager bistrainerManager)
        {
            _hierarchyManager = hierarchyManager;
            _bistrainerManager = bistrainerManager;
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

        public IActionResult OnPostExport()
        {
            var hierarchies = _hierarchyManager.HierarchyFlatTreeV2();
            var bistrainer_data = _bistrainerManager.UserFlatTree();
            var bistrainer_location_data = _bistrainerManager.LocationFlatTree();

            using (var workbook = new XLWorkbook())
            {
                #region Hierarchy

                var worksheet = workbook.Worksheets.Add("Hierarchies");
                var curRow = 2;

                foreach (var node in hierarchies)
                {
                    if (node != null)
                    {
                        if (node.Employee != null && node.Level > 1)
                        {
                            if (!string.IsNullOrEmpty(node.Employee.FirstName))
                            {
                                //worksheet.Cell(curRow, node.Level - 1).Value = $"({node.Level})";
                                worksheet.Cell(curRow, node.Level).Value = $"{node.Description} ({node.Employee.FullName})";
                                worksheet.Cell(curRow, node.Level).Style.Font.Bold = true;
                            }
                            else
                            {
                                //worksheet.Cell(curRow, node.Level - 1).Value = $"({node.Level})";
                                worksheet.Cell(curRow, node.Level).Value = $"{node.Description}";
                            }

                        }
                        else
                        {
                            worksheet.Cell(curRow, node.Level).Value = node.Description;
                        }
                        worksheet.Cell(1, node.Level).Value = $"Level {node.Level}";
                        curRow++;
                    }

                }



                //worksheet.Columns("A", "AL").AdjustToContents();
                //worksheet.Rows(1, 1).AdjustToContents();
                //worksheet.Rows(2, curRow).Height = 13.5;
                #endregion

                #region Bistrainer

                var worksheet_2 = workbook.Worksheets.Add("Bistrainer");
                curRow = 2;

                foreach (var node in bistrainer_data)
                {
                    if (node != null)
                    {
                        string location_name = node.Location == null ? "" : node.Location.LocationName;

                        worksheet_2.Cell(curRow, node.Level).Value = $"{location_name} ({node.FirstName} {node.LastName})";
                        worksheet_2.Cell(1, node.Level).Value = $"Level {node.Level}";
                        curRow++;
                    }

                }

                #endregion

                #region Bistrainer_Locations

                var worksheet_3 = workbook.Worksheets.Add("Locations");
                curRow = 2;

                foreach (var node in bistrainer_location_data)
                {
                    if (node != null)
                    {
                        worksheet_3.Cell(curRow, node.Level).Value = $"{node.LocationName}";
                        worksheet_3.Cell(1, node.Level).Value = $"Level {node.Level}";
                        curRow++;
                    }

                }

                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = "OrgChart_" + DateTime.Now + ".xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                };
            };
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

    public class Response
    {
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int Id { get; set; }
        public string EmployeeFullName { get; set; } = null!;
        public int EmployeeId { get; set; }
        public int Type { get; set; }
        public string Description { get; set; } = null!;
    }
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Number { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Department { get; set; } = null!;
        public int HierarchyId { get; set; }
        public bool Active { get; set; }
    }

    public class FlatHeirarchy
    {
        public string Department { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int Level { get; set; }
    }
}
