using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrgChart.Business;
using OrgChart.Business.Models;

namespace OrgChart.Web.Pages.Locations
{
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly OrgChart.Business.IBistrainerManager _bistrainerManager;

        public List<LocationViewModel> Locations { get; set; } = null!;
        public ParentLocationViewModel ParentLocation { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int LocationId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Level { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ParentLocationId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string LocationName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EmployeeId { get; set; }

        public List<UserViewModel> Users { get; set; }

        public IndexModel(IBistrainerManager bistrainerManager, IMapper mapper)
        {
            _bistrainerManager = bistrainerManager;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            ParentLocation = new ParentLocationViewModel
            {
                Locations = _bistrainerManager.LocationTree.Select(s => _mapper.Map<LocationViewModel>(s)).ToList(),
                ParentLevel = 0
            };

            return Page();
        }

        public IActionResult OnPostExportTopMatches()
        {
            TopEmployeeMatchViewModel results;
            var location = _bistrainerManager.Locations.FirstOrDefault(x => x.LocationId == LocationId);

            if (location != null)
            {
                results = new TopEmployeeMatchViewModel
                {
                    LocationCompanyRoles = location.CompanyRoles,
                    Users = _bistrainerManager.TopMatchedEmployees(LocationId, 10).Select(s => new UserViewModel
                    {
                        EmployeeId = s.EmployeeId,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        MatchedPercentage = s.MatchedPercentage,
                        MissingCompanyRoles = s.MissingCompanyRoles
                    }).ToList() ?? new List<UserViewModel>()
                };
            }
            else
            {
                return RedirectToPage("/Locations/Index");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Top Matched Users");
                worksheet.Style.Font.FontSize = 12;
                int curRow = 1;

                worksheet.Cell(curRow, 1).Value = "Location Required Roles";
                curRow++;

                foreach (var role in location.CompanyRoles)
                {
                    worksheet.Cell(curRow, 1).Value = role;
                    curRow++;
                }

                curRow += 2;
                worksheet.Cell(curRow, 1).Value = "Employee Number";
                worksheet.Cell(curRow, 2).Value = "Full Name";
                worksheet.Cell(curRow, 3).Value = "Matched Role Percentage";
                worksheet.Cell(curRow, 4).Value = "Missing Roles";
                curRow++;
                //worksheet.Row(curRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                //worksheet.Row(curRow).Style.Border.BottomBorderColor = XLColor.Black;
                //curRow++;

                foreach (var user in results.Users)
                {
                    worksheet.Cell(curRow, 1).Value = user.EmployeeId;
                    worksheet.Cell(curRow, 2).Value = user.FullName;
                    worksheet.Cell(curRow, 3).Value = user.MatchedPercentage;
                    worksheet.Cell(curRow, 4).Value = user.MissingCompanyRolesToString;
                    curRow++;
                }

                //worksheet.RangeUsed().SetAutoFilter();
                worksheet.Columns("A", "AA").AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string file_name = "TopMatchedUsers_" + DateTime.Now + ".xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                }
            }
        }
        public IActionResult OnPostExportLocations()
        {
            var locations = _bistrainerManager.LocationFlatTree();

            using (var workbook = new XLWorkbook())
            {
                var worksheet_1 = workbook.Worksheets.Add("Locations");
                var worksheet_2 = workbook.Worksheets.Add("Location Tree");
                worksheet_1.Style.Font.FontSize = 12;
                worksheet_2.Style.Font.FontSize = 12;

                int curRow = 1;
                int curCol = 1;

                worksheet_1.Cell(curRow, 1).Value = "Location Id";
                worksheet_1.Cell(curRow, 2).Value = "Location Name";
                worksheet_1.Cell(curRow, 3).Value = "Employee Id";
                worksheet_1.Cell(curRow, 4).Value = "Employee First Name";
                worksheet_1.Cell(curRow, 5).Value = "Employee Last Name";

                worksheet_1.Row(curRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet_1.Row(curRow).Style.Border.BottomBorderColor = XLColor.Black;
                curRow++;

                foreach (var l in locations.Where(x => x != null).ToList())
                {
                    curCol = l.Level;

                    worksheet_1.Cell(curRow, 1).Value = l.LocationId;
                    worksheet_1.Cell(curRow, 2).Value = l.LocationName;
                    worksheet_2.Cell(curRow, curCol).Value = l.LocationName;
                    worksheet_2.Cell(1, curCol).Value = $"Level {curCol}";
                    if (l.User != null)
                    {
                        worksheet_1.Cell(curRow, 3).Value = l.User.EmployeeId;
                        worksheet_1.Cell(curRow, 4).Value = l.User.FirstName;
                        worksheet_1.Cell(curRow, 5).Value = l.User.LastName;

                        worksheet_2.Cell(curRow, curCol + 1).Value = l.User.FullName;
                    }

                    curRow++;
                }

                worksheet_1.RangeUsed().SetAutoFilter();
                worksheet_1.Columns("A", "E").AdjustToContents();
                worksheet_2.Columns("A", "AA").AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string file_name = "Locations_" + DateTime.Now + ".xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                }
            }
        }
        public IActionResult OnPostExportTreeLocations()
        {
            var locations = _bistrainerManager.LocationFlatTree();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Location Tree");
                worksheet.Style.Font.FontSize = 12;
                int curRow = 1;
                int curCol = 1;

                foreach (var l in locations.Where(x => x != null).ToList())
                {
                    curCol = l.Level;

                    worksheet.Cell(curRow, curCol).Value = l.LocationName;

                    if (l.User != null)
                    {
                        worksheet.Cell(curRow, curCol + 1).Value = l.User.FullName;
                    }
                    else
                    {
                        worksheet.Cell(curRow, curCol + 1).Value = "-";
                    }

                    curRow++;
                }

                //worksheet.RangeUsed().SetAutoFilter();
                worksheet.Columns("A", "AA").AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string file_name = "Locations_Tree_" + DateTime.Now + ".xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                }
            }
        }

        public JsonResult OnGetUpdateLocationName()
        {
            var update = _bistrainerManager.Update(LocationId, LocationName);

            if (update == null)
                return new JsonResult(new ResponseViewModel { Status = "Fail", Message = "Error" });

            return new JsonResult(new ResponseViewModel { Status = "Success", Message = "Location Updated", LocationId = update.LocationId, LocationName = update.LocationName, ParentLocationId = update.ParentLocationId, ParentLocationName = update.ParentLocationName });
        }
        public JsonResult OnGetAddLocation()
        {
            var add = _bistrainerManager.Add(string.Empty, ParentLocationId);

            if (add == null)
                return new JsonResult(new ResponseViewModel { Status = "Fail", Message = "Error" });

            return new JsonResult(new ResponseViewModel { Status = "Success", Message = "Location Added", LocationId = add.LocationId, LocationName = add.LocationName, ParentLocationId = add.ParentLocationId, ParentLocationName = add.ParentLocationName, Level = add.Level });
        }
        public JsonResult OnGetRemoveLocation()
        {
            if (_bistrainerManager.HasChildren(LocationId))
                return new JsonResult(new ResponseViewModel { Status = "Fail", Message = "Location cannot be removed until all children are removed first." });

            if (!_bistrainerManager.Remove(LocationId))
                return new JsonResult(new ResponseViewModel { Status = "Fail", Message = "Location Was Not Removed." });

            return new JsonResult(new ResponseViewModel { Status = "Success", Message = "Location Removed." });
        }
        public JsonResult OnGetUpdateEmployee()
        {
            if (_bistrainerManager.UpdateEmployee(LocationId, EmployeeId) == null)
                return new JsonResult(new ResponseViewModel { Status = "Fail", Message = "Employee was not updated." });

            return new JsonResult(new ResponseViewModel { Status = "Success", Message = "Employee Updated." });
        }

        public JsonResult OnGetUsers()
        {
            return new JsonResult(_bistrainerManager.Users);
        }
        public PartialViewResult OnGetLocationPartial()
        {
            ParentLocation = new ParentLocationViewModel
            {
                Locations = _bistrainerManager.LocationTreeById(LocationId, Level).Select(s => _mapper.Map<LocationViewModel>(s)).ToList(),
                ParentLevel = Level,
                LocationId = LocationId,
                ParentLocationId = ParentLocationId,
                ReloadChildren = true
            };

            return Partial("_LocationPartial", ParentLocation);
        }
        public PartialViewResult OnGetTreeNodePartial()
        {
            Locations = _bistrainerManager.LocationTreeById(LocationId, Level).Select(s => _mapper.Map<LocationViewModel>(s)).ToList();

            return Partial("_TreeNodePartial", Locations.FirstOrDefault(x => x.LocationId == LocationId));
        }
        public PartialViewResult OnGetTopEmployeeMatchesPartial()
        {
            TopEmployeeMatchViewModel results = null!;

            var location = _bistrainerManager.Locations.FirstOrDefault(x => x.LocationId == LocationId);

            if (location != null)
            {
                results = new TopEmployeeMatchViewModel
                {
                    LocationCompanyRoles = location.CompanyRoles,
                    Users = _bistrainerManager.TopMatchedEmployees(LocationId, 10).Select(s => new UserViewModel
                    {
                        EmployeeId = s.EmployeeId,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        MatchedPercentage = s.MatchedPercentage,
                        MissingCompanyRoles = s.MissingCompanyRoles
                    }).ToList() ?? new List<UserViewModel>()
                };
            }

            return Partial("_TopEmployeeMatchesPartial", results);
        }
    }
    public class ParentLocationViewModel
    {
        public List<LocationViewModel> Locations { get; set; } = null!;
        public int LocationId { get; set; }
        public int ParentLocationId { get; set; }
        public int ParentLevel { get; set; }
        public bool ReloadChildren { get; set; }
    }
    public class LocationViewModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public int ParentLocationId { get; set; }
        public string ParentLocationName { get; set; } = null!;
        public int Level { get; set; }
        public bool FirstLevelNode { get; set; }
        public UserViewModel User { get; set; } = null!;
        public decimal? TeamHealth { get; set; } = null!;
        public bool CollapseAll { get; set; }

        public List<LocationViewModel> Children { get; set; } = null!;

        public LocationViewModel()
        {
            this.Children = new List<LocationViewModel>();
        }
    }
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string EmployeeId { get; set; } = null!;
        public string ReportsToEmployeeId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int LocationId { get; set; }
        public double MatchedPercentage { get; set; }

        public string CompanyRole { get; set; } = string.Empty;
        public List<string> CompanyRoles { get; set; } = null!;
        public List<string> MissingCompanyRoles { get; set; } = null!;

        public string MissingCompanyRolesToString
        {
            get
            {
                if (MissingCompanyRoles == null || !MissingCompanyRoles.Any())
                    return string.Empty;

                return string.Join(", ", MissingCompanyRoles);
            }
        }

    }

    public class ResponseViewModel
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int ParentLocationId { get; set; }
        public string ParentLocationName { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    public class TopEmployeeMatchViewModel
    {
        public List<string> LocationCompanyRoles { get; set; } = null!;
        public string str_LocationCompanyRoles
        {
            get
            {
                return string.Join(", ", LocationCompanyRoles);
            }
        }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }

}
