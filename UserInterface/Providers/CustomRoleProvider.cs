using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Domain.Repository.Concrete;
using Domain.Repository.Abstract;

/*******************  About this Provider  *******************
 *                                                           *
 * This is the Custom Role Provider. The Role parameter is   *
 * mapped to role rights according to the AMP Tool Database  * 
 * schema. For further clarification, please see the follow- *
 * ing tables:                                               *
 * 1- AccessRights                                           *
 * 2- Roles                                                  *
 * 3- RoleRights                                             *
 *                                                           *
 *************************************************************/

namespace UserInterface.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        IEmployeeRepository _empRepository = new SqlEmployeeRepository();

        public override string[] GetRolesForUser(string pRoleName)
        {
            return _empRepository.GetRoleRights(pRoleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string pRoleName, string pRightID)
        {
            return _empRepository.HasRoleRight(pRoleName, int.Parse(pRightID));
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}