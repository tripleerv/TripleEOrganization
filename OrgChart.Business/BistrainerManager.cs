using AutoMapper.Configuration.Annotations;
using OrgChart.Business.Models;
using OrgChart.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrgChart.Business
{
    public interface IBistrainerManager
    {
        public List<BistrainerUserModel> Users { get; }
        public List<BistrainerUserModel> UserTree { get; }
        public List<BistrainerUserModel> UserFlatTree();

        public List<BistrainerLocationModel> Locations { get; }
        public List<BistrainerLocationModel> LocationTree { get; }
        public List<BistrainerLocationModel> LocationTreeById(int location_id);
        public List<BistrainerLocationModel> LocationTreeById(int location_id, int level);
        public BistrainerLocationModel LocationTreeById1(int location_id, int level);
        public List<BistrainerLocationModel> LocationFlatTree();
        public List<BistrainerLocationModel> LocationFlatTree(int location_id);

        public List<string> Departments();
        public int OpenLocationsByLocation(int locationId);
        public int OpenLocationsByDepartment(string department);
        public List<BistrainerRankedUserModel> TopMatchedEmployees(int location_id, int take);

        BistrainerLocationModel Add(string name, int parent_id);
        BistrainerLocationModel Update(int id, string name);
        BistrainerLocationModel ChangeParent(int id, int parent_id);
        BistrainerLocationModel UpdateEmployee(int location_id, string employee_id);
        bool HasChildren(int id);
        bool Remove(int id);
    }
    public class BistrainerManager : IBistrainerManager
    {
        public static string GLOBAL_FIRST_LEVEL = string.Empty;

        //private readonly Repository.IBistrainerLocationRepository _bistrainerLocationRepository;

        private readonly Repository.IBistrainerTempLocationRepository _bistrainerLocationRepository;
        private readonly Repository.IBistrainerUserRepository _bistrainerUserRepository;

        //public BistrainerManager(Repository.IBistrainerLocationRepository bistrainerLocationRepository, Repository.IBistrainerUserRepository bistrainerUserRepository, Repository.IBistrainerTempLocationRepository bistrainerTempLocationRepository)
        public BistrainerManager(Repository.IBistrainerUserRepository bistrainerUserRepository, Repository.IBistrainerTempLocationRepository bistrainerTempLocationRepository)
        {
            _bistrainerLocationRepository = bistrainerTempLocationRepository;
            _bistrainerUserRepository = bistrainerUserRepository;
            //_bistrainerTempLocationRepository = bistrainerTempLocationRepository;
        }

        #region Users

        public List<BistrainerUserModel> Users
        {
            get
            {
                List<BistrainerUserModel> results = null!;

                results = _bistrainerUserRepository.Users.Select(s => new BistrainerUserModel(s.EmployeeId, s.FirstName, s.LastName, s.CompanyRole)
                {
                    Id = s.Id,
                    ReportsToEmployeeId = s.ReportsToEmployeeId,
                    Status = s.Status.Trim(),
                    LocationId = s.LocationId,
                }).ToList();


                return results;
            }
        }

        public List<BistrainerUserModel> UserTree
        {
            get
            {
                List<BistrainerUserModel> results = new List<BistrainerUserModel>();

                var all_users = _bistrainerUserRepository.Users.Where(x => x.Status.Contains("A")).ToList();

                var node = all_users.FirstOrDefault(x => x.EmployeeId.Equals("9906"))!;

                int level = 1;

                if (node == null)
                    return null!;

                var temp = new BistrainerUserModel(node.EmployeeId, node.FirstName, node.LastName, node.CompanyRole)
                {
                    Id = node.Id,
                    ReportsToEmployeeId = node.ReportsToEmployeeId,
                    Status = node.Status,
                    Location = Locations.Where(x => x.LocationId == node.LocationId).Select(s => new BistrainerLocationModel
                    {
                        LocationId = s.LocationId,
                        LocationName = s.LocationName,
                        ParentLocationId = s.ParentLocationId,
                        ParentLocationName = s.ParentLocationName
                    }).FirstOrDefault()!,
                    Level = level,
                    Parent = null!,
                    Children = GetChildren(all_users, node.EmployeeId, level++)
                };

                results.Add(temp);

                return results;
            }
        }

        private List<BistrainerUserModel> GetChildren(List<Repository.Models.UserRepositoryModel> all_nodes, string parent_id, int level)
        {
            var children = all_nodes.Where(x => x.ReportsToEmployeeId.Equals(parent_id)).OrderBy(o => o.LocationId).ToList();

            if (children == null)
            {
                level--;
                return null!;
            }

            BistrainerUserModel parent = all_nodes.Where(x => x.EmployeeId.Equals(parent_id)).Select(s => new BistrainerUserModel(s.EmployeeId, s.FirstName, s.LastName, s.CompanyRole)
            {
                Id = s.Id,
                ReportsToEmployeeId = s.ReportsToEmployeeId,
                Status = s.Status,
                Location = Locations.Where(x => x.LocationId == s.LocationId).Select(ss => new BistrainerLocationModel
                {
                    LocationId = ss.LocationId,
                    LocationName = ss.LocationName,
                    ParentLocationId = ss.ParentLocationId,
                    ParentLocationName = ss.ParentLocationName
                }).FirstOrDefault()!,
                Level = level,
                Parent = null!,
                Children = null!
            }).FirstOrDefault()!;

            level++;

            return children.Select(s => new BistrainerUserModel(s.EmployeeId, s.FirstName, s.LastName, s.CompanyRole)
            {
                Id = s.Id,
                ReportsToEmployeeId = s.ReportsToEmployeeId,
                Status = s.Status,
                Location = Locations.Where(x => x.LocationId == s.LocationId).Select(ss => new BistrainerLocationModel
                {
                    LocationId = ss.LocationId,
                    LocationName = ss.LocationName,
                    ParentLocationId = ss.ParentLocationId,
                    ParentLocationName = ss.ParentLocationName
                }).FirstOrDefault()!,
                Level = level,
                Parent = parent!,
                Children = GetChildren(all_nodes, s.EmployeeId, level)!
            }).ToList();
        }

        public List<BistrainerUserModel> UserFlatTree()
        {
            List<BistrainerUserModel> results = new List<BistrainerUserModel>();

            foreach (var node in UserTree)
            {
                results.Add(AddChildren(results, node.Children, node));
            }

            return results;
        }

        private BistrainerUserModel AddChildren(List<BistrainerUserModel> final_list, List<BistrainerUserModel> children, BistrainerUserModel parent)
        {

            final_list.Add(parent);

            if (children == null)
                return null!;

            foreach (var child in children)
            {
                final_list.Add(AddChildren(final_list, child.Children, child));
            }

            return null!;
        }

        #endregion

        #region Locations

        public List<BistrainerLocationModel> Locations
        {
            get
            {
                List<BistrainerLocationModel> results = null!;

                results = _bistrainerLocationRepository.Locations.Select(s => new BistrainerLocationModel
                {
                    LocationId = s.LocationId,
                    LocationName = s.LocationName,
                    ParentLocationId = s.ParentLocationId,
                    ParentLocationName = s.ParentLocationName,
                    User = Users.FirstOrDefault(x => x.EmployeeId.Equals(s.EmployeeId))!,
                    CompanyRole = s.CompanyRole
                }).ToList();

                //foreach (var loc in results)
                //{
                //    var new_loc = _bistrainerTempLocationRepository.Add(loc.LocationId, loc.LocationName, loc.ParentLocationId, string.Empty);

                //    if (new_loc != null && loc.User != null)
                //        _bistrainerTempLocationRepository.UpdateEmployee(new_loc.LocationId, loc.User.EmployeeId);
                //}

                //foreach(var loc in results)
                //{
                //    if (loc.User != null)
                //    {
                //        if (!string.IsNullOrEmpty(loc.User.CompanyRole))
                //        {
                //            _bistrainerRepository.UpdateCompanyRole(loc.LocationId, loc.User.CompanyRole);
                //        }
                //    }
                //}

                return results;
            }
        }

        public List<BistrainerLocationModel> LocationTree
        {
            get
            {
                //var tete = Locations;

                List<BistrainerLocationModel> results = new List<BistrainerLocationModel>();
                GLOBAL_FIRST_LEVEL = "1";
                var all_locations = _bistrainerLocationRepository.Locations;


                var node = all_locations.FirstOrDefault(x => x.ParentLocationId == 0)!;
                int level = 1;

                if (node == null)
                    return null!;

                var temp = new BistrainerLocationModel
                {
                    LocationId = node.LocationId,
                    LocationName = node.LocationName,
                    ParentLocationId = node.ParentLocationId,
                    ParentLocationName = node.ParentLocationName,
                    Level = level,
                    FirstLevelNode = !GLOBAL_FIRST_LEVEL.Contains(level.ToString()),
                    Parent = null!,
                    User = Users.FirstOrDefault(x => x.EmployeeId.Equals(node.EmployeeId))!,
                    Children = GetChildren(all_locations, node.LocationId, level++)
                };

                results.Add(temp);


                return results;
            }
        }

        public List<BistrainerLocationModel> LocationTreeById(int location_id, int level)
        {
            List<BistrainerLocationModel> results = new List<BistrainerLocationModel>();

            var all_locations = _bistrainerLocationRepository.Locations;

            var node = all_locations.FirstOrDefault(x => x.LocationId == location_id)!;

            if (node == null)
                return null!;

            var temp = new BistrainerLocationModel
            {
                LocationId = node.LocationId,
                LocationName = node.LocationName,
                ParentLocationId = node.ParentLocationId,
                ParentLocationName = node.ParentLocationName,
                Level = level,
                Parent = null!,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(node.EmployeeId))!,
                Children = GetChildren(all_locations, node.LocationId, level++)
            };

            results.Add(temp);


            return results;
        }

        private List<BistrainerLocationModel> GetChildren(List<Repository.Models.LocationRepositoryModel> all_nodes, int parent_id, int level)
        {
            if (level == 1)
            {
                GLOBAL_FIRST_LEVEL = level.ToString();
            }
            else
            {
                if (!GLOBAL_FIRST_LEVEL.Contains(level.ToString()))
                    GLOBAL_FIRST_LEVEL += $",{level}";
            }

            var children = all_nodes.Where(x => x.ParentLocationId == parent_id).ToList();

            if (children == null)
            {
                level--;
                return null!;
            }

            BistrainerLocationModel parent = all_nodes.Where(x => x.LocationId == parent_id).Select(s => new BistrainerLocationModel
            {
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                ParentLocationId = s.ParentLocationId,
                ParentLocationName = s.ParentLocationName,
                Level = level,
                FirstLevelNode = level < 4 && !GLOBAL_FIRST_LEVEL.Contains(level.ToString()),
                Parent = null!,
                Children = null!,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(s.EmployeeId))!,
            }).FirstOrDefault()!;

            level++;

            return children.Select(s => new BistrainerLocationModel
            {
                LocationId = s.LocationId,
                LocationName = s.LocationName,
                ParentLocationId = s.ParentLocationId,
                ParentLocationName = s.ParentLocationName,
                Level = level,
                FirstLevelNode = level < 4 && !GLOBAL_FIRST_LEVEL.Contains(level.ToString()),
                Parent = parent!,
                Children = GetChildren(all_nodes, s.LocationId, level)!,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(s.EmployeeId))!,
            }).ToList();
        }

        public List<BistrainerLocationModel> LocationFlatTree()
        {
            List<BistrainerLocationModel> results = new List<BistrainerLocationModel>();

            foreach (var node in LocationTree)
            {
                results.Add(AddChildren(results, node.Children, node));
            }

            return results;
        }

        public List<BistrainerLocationModel> LocationFlatTree(int location_id)
        {
            List<BistrainerLocationModel> results = new List<BistrainerLocationModel>();

            var parent_node = LocationTreeById1(location_id,0);

            if (parent_node == null)
                return null!;

            results.Add(parent_node);

            foreach (var node in parent_node.Children)
            {
                results.Add(AddChildren(results, node.Children, node));
            }

            return results;
        }

        private BistrainerLocationModel AddChildren(List<BistrainerLocationModel> final_list, List<BistrainerLocationModel> children, BistrainerLocationModel parent)
        {

            final_list.Add(parent);

            if (children == null)
                return null!;

            foreach (var child in children)
            {
                final_list.Add(AddChildren(final_list, child.Children, child));
            }

            return null!;
        }

        #region CRUD Options

        public BistrainerLocationModel Add(string name, int parent_id)
        {
            var parent = Locations.FirstOrDefault(x => x.LocationId == parent_id);

            if (parent == null)
                return null!;

            var add = _bistrainerLocationRepository.Add(name, parent_id, parent.ParentLocationName);

            if (add == null)
                return null!;

            return new BistrainerLocationModel
            {
                LocationId = add.LocationId,
                LocationName = add.LocationName,
                ParentLocationId = add.ParentLocationId,
                ParentLocationName = add.ParentLocationName,
                Parent = null!,
                Children = null!
            };
        }

        public BistrainerLocationModel Update(int id, string name)
        {
            var update = _bistrainerLocationRepository.Locations.FirstOrDefault(x => x.LocationId == id);

            if (update == null)
                return null!;

            update = _bistrainerLocationRepository.Update(id, name, update.ParentLocationId, update.ParentLocationName);

            if (update == null)
                return null!;

            return new BistrainerLocationModel
            {
                LocationId = update.LocationId,
                LocationName = update.LocationName,
                ParentLocationId = update.ParentLocationId,
                ParentLocationName = update.ParentLocationName,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(update.EmployeeId))!,
                Parent = null!,
                Children = null!
            };
        }

        public BistrainerLocationModel ChangeParent(int id, int parent_id)
        {
            var update = _bistrainerLocationRepository.Locations.FirstOrDefault(x => x.LocationId == id);

            if (update == null)
                return null!;

            var parent = _bistrainerLocationRepository.Locations.FirstOrDefault(x => x.LocationId == parent_id);

            if (parent == null)
                return null!;

            update = _bistrainerLocationRepository.Update(id, update.LocationName, parent.LocationId, parent.LocationName);

            if (update == null)
                return null!;

            return new BistrainerLocationModel
            {
                LocationId = update.LocationId,
                LocationName = update.LocationName,
                ParentLocationId = update.ParentLocationId,
                ParentLocationName = update.ParentLocationName,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(update.EmployeeId))!,
                Parent = null!,
                Children = null!
            };
        }

        public bool HasChildren(int id)
        {
            return _bistrainerLocationRepository.Locations.Any(x => x.ParentLocationId == id);
        }

        public bool Remove(int id)
        {
            return _bistrainerLocationRepository.Remove(id);
        }

        public BistrainerLocationModel UpdateEmployee(int location_id, string employee_id)
        {
            var results = _bistrainerLocationRepository.UpdateEmployee(location_id, employee_id);

            if (results == null)
                return null!;

            return new BistrainerLocationModel
            {
                LocationId = results.LocationId,
                LocationName = results.LocationName,
                ParentLocationId = results.ParentLocationId,
                ParentLocationName = results.ParentLocationName,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(results.EmployeeId))!,
                Parent = null!,
                Children = null!
            };
        }

        public List<BistrainerRankedUserModel> TopMatchedEmployees(int location_id, int take)
        {
            take = 999;
            List<BistrainerRankedUserModel> results = new List<BistrainerRankedUserModel>();

            var location = Locations.FirstOrDefault(x => x.LocationId == location_id);

            if (location == null)
                return null!;

            if (!location.CompanyRoles.Any())
                return new List<BistrainerRankedUserModel>();

            List<int> ids = new List<int> { 7, 46, 162, 165, 174, 207, 227, 315, 382, 449, 459, 508, 532, 629 };

            var location_company_roles = location.CompanyRole;

            foreach (var user in Users)
            {
                var user_company_roles = user.CompanyRole;

                var new_users = new BistrainerRankedUserModel(user.EmployeeId, user.FirstName, user.LastName, user.CompanyRole, location.CompanyRoles);

                if (new_users.MatchedPercentage >= 50)
                {
                    results.Add(new_users);

                    //if (results.Count > take)
                    //{
                    //    return results.Take(take).ToList();
                    //}
                }
            }

            return results.Where(x => x.MatchedPercentage >= 50).OrderByDescending(o => o.MatchedPercentage).ToList();
        }

        #endregion

        #endregion

        public List<string> Departments()
        {
            var temp = GetOpenLocations(8);

            return _bistrainerLocationRepository.Departments;
        }

        public int OpenLocationsByLocation(int locationId)
        {
            throw new NotImplementedException();
        }

        public int OpenLocationsByDepartment(string department)
        {
            throw new NotImplementedException();
        }

        private int GetOpenLocations(int locationId)
        {
            var location = LocationTreeById1(locationId, 0);
            if (location == null)
                return 0;

            int count = location.User == null ? 1 : 0;

            foreach (var child in location.Children)
            {
                count += GetOpenLocations(child.LocationId);
            }

            return count;
        }

        public List<BistrainerLocationModel> LocationTreeById(int location_id)
        {
            throw new NotImplementedException();
        }

        public BistrainerLocationModel LocationTreeById1(int location_id, int level)
        {
           BistrainerLocationModel results = new BistrainerLocationModel();

            var all_locations = _bistrainerLocationRepository.Locations;

            var node = all_locations.FirstOrDefault(x => x.LocationId == location_id)!;

            if (node == null)
                return null!;

            results = new BistrainerLocationModel
            {
                LocationId = node.LocationId,
                LocationName = node.LocationName,
                ParentLocationId = node.ParentLocationId,
                ParentLocationName = node.ParentLocationName,
                Level = level,
                Parent = null!,
                User = Users.FirstOrDefault(x => x.EmployeeId.Equals(node.EmployeeId))!,
                Children = GetChildren(all_locations, node.LocationId, level++)
            };


            return results;
        }
    }
}
