using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Repository.Abstract;
using Domain.Entities;
using System.Configuration;

/*-------------------- About this Class ---------------------*
 *                                                           *
 * This class is interface for the interaction with Database *
 * in contex with Organization relevant data manipulation.   *   
 * Like User (User Login, User Creation etc) Management and  *
 * Organization Management (Organization Information).       *                   
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlOrganizationRepository : IOrganizationRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlOrganizationRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public string GetOrganizationName()
        {
            var orgName = from x in _objectModel.OrgAttributes
                          where x.OrgAttName == "Organization Name"
                          from y in _objectModel.OrgAttValues
                          where x.OrgAttID == y.OrgAttID
                          select y.Value;

            if (orgName.First().ToString() != "")
                return orgName.First().ToString();
            else
                return "Your Organization Name";
        }

        public IQueryable<OrgAttValue> GetOrgInfo()
        {
            var orgInfo = from x in _objectModel.OrgAttValues
                          select x;

            return orgInfo;
        }

        public bool EditOrgInfo(string[] pAttValues, int[] pAttIDs)
        {
            int index = 0;
            bool result = false;

            var orgAttributes = from x in _objectModel.OrgAttValues
                                select x;

            foreach (int id in pAttIDs)
            {
                orgAttributes.Where(x => x.OrgAttID == id).First().Value = pAttValues[index++];
            }

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            return result;
        }

        // The parameter "pFillterList" is to filter out the list.
        public IQueryable<Risk> RiskList(int[] pFilterList = null)
        {
            if (pFilterList == null)
            {
                var riskList = from x in _objectModel.Risks
                               where x.OrganizationLevel == true
                               select x;

                return riskList;
            }

            else
            {
                var riskList = from x in _objectModel.Risks
                               where !(pFilterList.Contains(x.RiskID)) && x.OrganizationLevel == true
                               select x;

                return riskList;
            }
        }

        public Risk GetRiskByID(int pRiskID)
        {
            return _objectModel.Risks.Where(x => x.RiskID == pRiskID).First();
        }

        public bool EditRisk(int pRiskID, string pRiskDescription, int pRiskCategoryID)
        {
            bool result = false;
            var risk = _objectModel.Risks.Where(x => x.RiskID == pRiskID).First();

            risk.Description = pRiskDescription;
            risk.Category = pRiskCategoryID;

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            return result;
        }

        public bool AddRisk(string pRiskDescription, int pCategoryID, bool pOrganizationLevel)
        {
            bool result = false;

            _objectModel.Risks.InsertOnSubmit(new Risk
            {
                Description = pRiskDescription,
                Category = pCategoryID,
                OrganizationLevel = pOrganizationLevel
            }
            );

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            return result;
        }
        
        public IQueryable<RiskCategory> RiskCategories()
        {
            return _objectModel.RiskCategories.Select(x => x);
        }

        public int AddRiskCategory(string pCategoryName)
        {
            // If transaction fails then return -1.
            int result = -1;

            if (_objectModel.RiskCategories.Where(x => x.CategoryName == pCategoryName).Count() == 0)
            {
                _objectModel.RiskCategories.InsertOnSubmit(new RiskCategory
                {
                    CategoryName = pCategoryName
                }
                );

                try
                {
                    _objectModel.SubmitChanges();

                    // Returning the newly added category id to be used in list.
                    result = _objectModel.RiskCategories.Select(x => x.CategoryID).Max();
                }

                catch (Exception ex)
                {
                }
            }

            else
            {
                // 0 means category already exists.
                result = 0;
            }

            return result;
        }

        public IQueryable<Role> RoleList()
        {
            var role = from x in _objectModel.Roles
                       select x;
            return role;
        }

        public IQueryable<AccessRight> AccessRightList()
        {
            var right = from x in _objectModel.AccessRights
                        select x;
            return right;
        }

        public List<int> AccessRightListByID(int pRoleID)
        {
            var rightIds = from x in _objectModel.RoleRights
                           where x.RoleID == pRoleID
                           select x.RightID;
            return rightIds.ToList();
        }

        public bool EditRole(string[] pRights, int[] pRightID, int pRoleID)
        {
            int[] assignedRights = new int[pRightID.Count()];
            int[] removededRights = new int[pRightID.Count()];
            int index1 = 0, counter1 = 0;
            int index2 = 0, counter2 = 0;

            foreach (int r in pRightID)
            {
                if (pRights[counter1++] == "Yes")
                    assignedRights[index1++] = r < 0 ? -r : r;
                else
                {
                    counter2++;
                    removededRights[index2++] = r < 0 ? -r : r;
                }
            }

            var rolerights = from x in _objectModel.RoleRights
                             where x.RoleID == pRoleID
                             select x;
            
            if (counter2 > 0)
            {
                foreach (RoleRight r in rolerights)
                {
                    if (removededRights.Contains(r.RightID))
                    {
                        _objectModel.RoleRights.DeleteOnSubmit(r);
                    }
                }
                try
                {
                    _objectModel.SubmitChanges();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            
            var roleright = from x in _objectModel.RoleRights
                            where x.RoleID == pRoleID
                            select x.RightID;
            
            foreach (int r in assignedRights)
            {
                if (!roleright.Contains(r) && r != 0)
                {
                    _objectModel.RoleRights.InsertOnSubmit(new RoleRight { RoleID = pRoleID, RightID = r });
                }
            }
            try
            {
                _objectModel.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int AddRole(int[] pRightIDs, string[] pRights)
        {
            string roleName = "";
            int counter = 0, result = -1;
            int roleID = (from x in _objectModel.Roles
                          select x.RoleID).Max();
            roleID++;

            foreach (int n in pRightIDs)
            {
                if (n == 0)
                    roleName = pRights[counter];

                counter++;
            }

            if (!(IsRoleNameExist(roleName)))
            {
                _objectModel.Roles.InsertOnSubmit(new Role { RoleID = roleID, RoleName = roleName });
                counter = 0;

                foreach (int n in pRightIDs)
                {
                    if (n != 0 && pRights[counter] == "Yes")
                        _objectModel.RoleRights.InsertOnSubmit(new RoleRight { RoleID = roleID, RightID = n });

                    counter++;
                }

                var empRole = from x in _objectModel.EmpAttributes
                              where x.EmpAttName == "Role"
                              select x;

                empRole.First().DefaultValue += ";" + roleName;

                try
                {
                    _objectModel.SubmitChanges();
                    result = 1;
                }
                catch (Exception ex)
                {
                }
            }

            else
            {
                result = 0;
            }

            return result;
        }

        public bool IsRoleNameExist(string pRoleName)
        {
            var role = from x in _objectModel.Roles
                       where x.RoleName == pRoleName
                       select x;

            if (role.Count() > 0)
                return true;

            return false;
        }

        public string GetAnnouncement()
        {
            return _objectModel.Organizations.Select(x => x.Announcement).FirstOrDefault();
        }

        public bool UpdateAnnouncement(string pMessage)
        {
            bool result = false;

            var org = _objectModel.Organizations.First();
            org.Announcement = pMessage;

            try
            {
                _objectModel.SubmitChanges();
                result = true;
            }

            catch (Exception ex)
            {
            }

            return result;
        }

        // Returs the number of working days per week.
        public int GetWorkingDays()
        {
            var days = (from x in _objectModel.OrgAttributes
                        where x.OrgAttName == "Working Days/Week"
                        from y in _objectModel.OrgAttValues
                        where y.OrgAttID == x.OrgAttID
                        select y.Value).First();

            return int.Parse(days);
        }

        public int GetDailyHours()
        {
            var hours = (from x in _objectModel.OrgAttributes
                         where x.OrgAttName == "Working Hours/Day"
                         from y in _objectModel.OrgAttValues
                         where y.OrgAttID == x.OrgAttID
                         select y.Value).First();

            return int.Parse(hours);
        }

        public int GetNewlyAddedRiskID()
        {
            return _objectModel.Risks.Select(x => x.RiskID).Max();
        }
    }
}
