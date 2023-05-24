using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrgChart.Business.Models;
using OrgChart.Repository;

namespace OrgChart.Business
{
    public enum EHierarchyType
    {
        Department,
        Personnel
    }

    public interface IHierarchyManager
    {
        List<Repository.Models.HierarchyModel> Hierarchies { get; }
        List<HierarchyModel> HierarchyTree { get; }
        List<OrgChartModel> HierarchyFlatTree();
        List<HierarchyModel> HierarchyFlatTreeV2();
        List<EmployeeModel> Employees { get; }
        EmployeeModel Employee(int employee_id);
        HierarchyModel Add(EHierarchyType type, string description, int employee_id, int parent_id);
        HierarchyModel Update(int id, EHierarchyType type, string description, int employee_id);
        HierarchyModel ChangeParent(int id, int parent_id);
        bool MoveUp(int id);
        bool MoveDown(int id);
        bool HasChildren(int id);
        bool Remove(int id);
    }

    public class HierarchyManager : IHierarchyManager
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISubDepartmentRepository _subDepartmentRepository;
        private readonly IHierarchyRepository _hierarchyRepository;

        public HierarchyManager(IEmployeeRepository employeeRepository, IJobRepository jobRepository, ISubDepartmentRepository subDepartmentRepository, IDepartmentRepository departmentRepository, IHierarchyRepository hierarchyRepository)
        {
            _employeeRepository = employeeRepository;
            _jobRepository = jobRepository;
            _departmentRepository = departmentRepository;
            _subDepartmentRepository = subDepartmentRepository;
            _hierarchyRepository = hierarchyRepository;
        }

        public List<HierarchyModel> HierarchyTree
        {
            get
            {
                List<HierarchyModel> results = new List<HierarchyModel>();

                var all_hierarchies = _hierarchyRepository.Hierarchies;
                var e = _employeeRepository.Employees;

                var top_nodes = all_hierarchies.Where(x => x.ParentId == 0).ToList();
                int level = 1;
                foreach (var node in top_nodes)
                {
                    var temp = new HierarchyModel
                    {
                        Id = node.Id,
                        Parent = null,
                        Type = (EHierarchyType)node.Type,
                        Description = node.Description,
                        Sequence = node.Sequence,
                        Level = level,
                        Employee = node.EmployeeId == 0 ? new EmployeeModel() : e.Where(x => x.Id == node.EmployeeId)
                                                                                    .Select(s => new EmployeeModel
                                                                                    {
                                                                                        Id = s.Id,
                                                                                        FirstName = s.FirstName,
                                                                                        LastName = s.LastName,
                                                                                        Number = s.Number,
                                                                                        Active = s.Active,
                                                                                        ImageUrl = s.ImageUrl,
                                                                                        HireDate = s.HireDate == null ? DateTime.MinValue : (DateTime)s.HireDate
                                                                                    })
                                                                                    .FirstOrDefault()!,
                        Children = GetChildren(all_hierarchies, node.Id, level++)
                    };

                    results.Add(temp);
                }

                return results;
            }
        }

        public List<EmployeeModel> Employees
        {
            get
            {
                var departments = _departmentRepository.Departments;
                var sub_departments = _subDepartmentRepository.SubDepartments;

                var employees = _employeeRepository.Employees.Select(s => new EmployeeModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Position = s.Position,
                    Department = s.SubDepartmentId == 0 ? new DepartmentModel() : 
                        sub_departments.Where(x => x.SubDepartmentId == s.SubDepartmentId).Select(ss => new DepartmentModel
                        { 
                            DepartmentId = ss.DepartmentId,
                            SubDepartmentId = ss.SubDepartmentId,
                            SubDepartmentName = ss.SubDepartmentName,
                            DepartmentName = departments.FirstOrDefault(x => x.DepartmentId == ss.DepartmentId)!.DepartmentName
                        }).FirstOrDefault()!,
                    Number = s.Number,
                    Active = s.Active,
                    ImageUrl = s.ImageUrl,
                    HireDate = s.HireDate == null ? DateTime.MinValue : (DateTime)s.HireDate
                }).ToList();

                return employees;
            }
        }

        public List<Repository.Models.HierarchyModel> Hierarchies
        {
            get
            {
                return _hierarchyRepository.Hierarchies;
            }
        }

        private List<HierarchyModel> GetChildren(List<Repository.Models.HierarchyModel> all_nodes, int parent_id, int level)
        {
            var children = all_nodes.Where(x => x.ParentId == parent_id).OrderBy(o => o.Sequence).ToList();

            if (children == null)
            {
                level--;
                return null!;
            }

            var e = _employeeRepository.Employees;
            HierarchyModel parent = all_nodes.Where(x => x.Id == parent_id).Select(s => new HierarchyModel
            {
                Id = s.Id,
                Parent = null,
                Type = (EHierarchyType)s.Type,
                Description = s.Description,
                Sequence = s.Sequence,
                Level = level,
                Employee = s.EmployeeId == 0 ? new EmployeeModel() : e.Where(x => x.Id == s.EmployeeId)
                                                                        .Select(s => new EmployeeModel
                                                                        {
                                                                            Id = s.Id,
                                                                            FirstName = s.FirstName,
                                                                            LastName = s.LastName,
                                                                            Number = s.Number,
                                                                            Active = s.Active,
                                                                            ImageUrl = s.ImageUrl,                                                                            
                                                                        })
                                                                        .FirstOrDefault()!,
                Children = null
            }).FirstOrDefault()!;
            level++;
            return children.Select(s => new HierarchyModel
            {
                Id = s.Id,
                Parent = parent,
                Type = (EHierarchyType)s.Type,
                Description = s.Description,
                Sequence = s.Sequence,
                Level = level,
                Employee = s.EmployeeId == 0 ? new EmployeeModel() : e.Where(x => x.Id == s.EmployeeId)
                                                                        .Select(s => new EmployeeModel
                                                                        {
                                                                            Id = s.Id,
                                                                            FirstName = s.FirstName,
                                                                            LastName = s.LastName,
                                                                            Number = s.Number,
                                                                            Active = s.Active,
                                                                            ImageUrl = s.ImageUrl
                                                                        })
                                                                        .FirstOrDefault()!,
                Children = GetChildren(all_nodes, s.Id, level)
            }).OrderBy(o => o.Sequence).ToList();
        }

        public HierarchyModel Add(EHierarchyType type, string description, int employee_id, int parent_id)
        {
            if (string.IsNullOrEmpty(description))
                return null;

            var children = Hierarchies.Where(x => x.ParentId == parent_id).ToList();

            int sequence = 0;

            switch (children.Count)
            {
                case >= 1: sequence = children.Max(x => x.Sequence) + 10; break;
                default: sequence = 10; break;
            }

            var add = _hierarchyRepository.Add((int)type, description, employee_id, parent_id, sequence);

            if (add == null)
                return null;

            var employee = _employeeRepository.Employee(employee_id);
            HierarchyModel parent = _hierarchyRepository.Hierarchies.Where(x => x.Id == parent_id).Select(s => new HierarchyModel
            {
                Id = s.Id,
                Parent = null,
                Type = (EHierarchyType)s.Type,
                Description = s.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Children = null,
                Sequence = s.Sequence
            }).FirstOrDefault();

            return new HierarchyModel
            {
                Id = add.Id,
                Parent = parent,
                Description = add.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Type = (EHierarchyType)add.Type,
                Children = null,
                Sequence = add.Sequence
            };
        }

        public bool Remove(int id)
        {
            return _hierarchyRepository.Remove(id);
        }

        public HierarchyModel Update(int id, EHierarchyType type, string description, int employee_id)
        {
            var update = _hierarchyRepository.Hierarchies.FirstOrDefault(x => x.Id == id);

            if (update == null)
                return null;

            if (type == 0)
            {
                employee_id = 0;
            }
            update = _hierarchyRepository.Update(id, (int)type, description, employee_id, update.ParentId, update.Sequence);

            if (update == null)
                return null;

            var employee = _employeeRepository.Employee(employee_id);
            HierarchyModel parent = _hierarchyRepository.Hierarchies.Where(x => x.Id == update.ParentId).Select(s => new HierarchyModel
            {
                Id = s.Id,
                Parent = null,
                Type = (EHierarchyType)s.Type,
                Description = s.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Children = null
            }).FirstOrDefault();

            return new HierarchyModel
            {
                Id = update.Id,
                Parent = parent,
                Description = update.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Type = (EHierarchyType)update.Type,
                Children = null,
                Sequence = update.Sequence
            };
        }

        public HierarchyModel ChangeParent(int id, int parent_id)
        {
            var update = _hierarchyRepository.Hierarchies.FirstOrDefault(x => x.Id == id);

            if (update == null)
                return null;

            update = _hierarchyRepository.Update(id, update.Type, update.Description, update.EmployeeId, parent_id, update.Sequence);

            if (update == null)
                return null;

            var employee = _employeeRepository.Employee(update.EmployeeId);
            HierarchyModel parent = _hierarchyRepository.Hierarchies.Where(x => x.Id == update.ParentId).Select(s => new HierarchyModel
            {
                Id = s.Id,
                Parent = null,
                Type = (EHierarchyType)s.Type,
                Description = s.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Children = null
            }).FirstOrDefault();

            return new HierarchyModel
            {
                Id = update.Id,
                Parent = parent,
                Description = update.Description,
                Employee = employee == null ? new EmployeeModel() : new EmployeeModel { Id = employee.Id, FirstName = employee.FirstName, LastName = employee.LastName, Number = employee.Number },
                Type = (EHierarchyType)update.Type,
                Children = null,
                Sequence = update.Sequence
            };
        }

        public bool HasChildren(int id)
        {
            return _hierarchyRepository.Hierarchies.Any(x => x.ParentId == id);
        }

        public bool MoveUp(int id)
        {
            var update = Hierarchies.FirstOrDefault(x => x.Id == id);

            if (update == null)
                return false;

            var children = Hierarchies.Where(x => x.ParentId == update.ParentId).ToList();

            if (children.Count < 1)
                return false;

            int min = children.Min(x => x.Sequence);

            if (update.Sequence == min)
                return false;

            var updated = _hierarchyRepository.Update(update.Id, update.Type, update.Description, update.EmployeeId, update.ParentId, update.Sequence - 11);

            if (updated == null)
                return false;

            return true;
        }

        public bool MoveDown(int id)
        {
            var update = Hierarchies.FirstOrDefault(x => x.Id == id);

            if (update == null)
                return false;

            var children = Hierarchies.Where(x => x.ParentId == update.ParentId).ToList();

            if (children.Count < 1)
                return false;

            int max = children.Max(x => x.Sequence);

            if (update.Sequence == max)
                return false;

            var updated = _hierarchyRepository.Update(update.Id, update.Type, update.Description, update.EmployeeId, update.ParentId, update.Sequence + 11);

            if (updated == null)
                return false;

            return true;
        }

        public List<OrgChartModel> HierarchyFlatTree()
        {
            List<OrgChartModel> results = new List<OrgChartModel>();

            var all_hierarchies = _hierarchyRepository.Hierarchies;
            var e = Employees;

            var top_nodes = all_hierarchies.Where(x => x.ParentId == 0).ToList();

            foreach (var node in top_nodes)
            {
                var employee = node.EmployeeId == 0 ? null : e.FirstOrDefault(x => x.Id == node.EmployeeId);

                OrgEmployeeModel org_employee = null;
                OrgDepartmentModel org_department = null;

                if (employee == null)
                {
                    org_department = new OrgDepartmentModel
                    {
                        DisplayName = node.Description
                    };
                }
                else
                {
                    var jobs = _jobRepository.Jobs;

                    org_employee = new OrgEmployeeModel
                    {
                        EmployeeId = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        ImageUrl = employee.ImageUrl
                    };
                }

                var org = new OrgChartModel
                {
                    Id = node.Id,
                    ParentId = node.ParentId,
                    Department = org_department,
                    Employee = org_employee
                };

                results.Add(AddChildren(results, all_hierarchies, e, org.Id));
            }

            return results;
        }

        private OrgChartModel AddChildren(List<OrgChartModel> final_list, List<Repository.Models.HierarchyModel> all_nodes, List<EmployeeModel> employees, int parent_id)
        {
            var children = all_nodes.Where(x => x.ParentId == parent_id).OrderBy(o => o.Sequence).ToList();

            OrgEmployeeModel org_employee = null;
            OrgDepartmentModel org_department = null;

            var parent = all_nodes.FirstOrDefault(x => x.Id == parent_id);
            var employee = parent.EmployeeId == 0 ? null : employees.FirstOrDefault(x => x.Id == parent.EmployeeId);

            if (employee == null)
            {
                org_department = new OrgDepartmentModel
                {
                    DisplayName = parent.Description
                };
            }
            else
            {
                org_employee = new OrgEmployeeModel
                {
                    EmployeeId = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    ImageUrl = employee.ImageUrl
                };
            }

            var org = new OrgChartModel
            {
                Id = parent.Id,
                ParentId = parent.ParentId,
                Department = org_department,
                Employee = org_employee
            };

            final_list.Add(org);

            if (children == null)
                return null;

            foreach (var child in children)
            {
                final_list.Add(AddChildren(final_list, all_nodes, employees, child.Id));
            }

            return null;
        }

        public List<HierarchyModel> HierarchyFlatTreeV2()
        {
            List<HierarchyModel> results = new List<HierarchyModel>();

            foreach (var node in HierarchyTree)
            {
                results.Add(AddChildrenV2(results, node.Children, node));
            }

            return results;
        }

        private HierarchyModel AddChildrenV2(List<HierarchyModel> final_list, List<HierarchyModel> children, HierarchyModel parent)
        {

            final_list.Add(parent);

            if (children == null)
                return null;

            foreach (var child in children)
            {
                final_list.Add(AddChildrenV2(final_list, child.Children, child));
            }

            return null;
        }

        public EmployeeModel Employee(int employee_id)
        {
            return Employees.FirstOrDefault(x => x.Id == employee_id);
        }
    }
}
