using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface -------------------*
 *                                                           *
 * This interface is for the interaction with Database in    *
 * contex with Organization relevant data manipulation.      *   
 * Please see "SqlOrganizationRepository" class for further  *
 * details.                                                  * 
 *                                                           *
 *---------------------------------------------------------- */

namespace Domain.Repository.Abstract
{
    public interface IOrganizationRepository
    {
        IQueryable<OrgAttValue> GetOrgInfo();
        bool EditOrgInfo(string[] pAttValues, int[] pAttIDs);
        string GetOrganizationName();
        IQueryable<Risk> RiskList(int[] pFilterList = null);
        Risk GetRiskByID(int pRiskID);
        int AddRiskCategory(string pCategoryName);
        bool AddRisk(string pRiskDescription, int pCategoryID, bool pOrganizationLevel);
        bool EditRisk(int pRiskID, string pRiskDescription, int pRiskCategoryID);
        IQueryable<RiskCategory> RiskCategories();
        IQueryable<Role> RoleList();
        IQueryable<AccessRight> AccessRightList();
        List<int> AccessRightListByID(int pRoleId);
        bool EditRole(string[] pRights, int[] pRightID, int pRoleID);
        int AddRole(int[] pRightIDs, string[] pRights);
        bool IsRoleNameExist(string pRoleName);
        string GetAnnouncement();
        bool UpdateAnnouncement(string pMessage);
        int GetWorkingDays();
        int GetDailyHours();
        int GetNewlyAddedRiskID();
    }
}
