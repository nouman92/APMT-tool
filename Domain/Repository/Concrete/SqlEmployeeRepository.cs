using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Repository.Abstract;
using Domain.Entities;
using System.Configuration;

/*-------------------- About this Class ---------------------*
 *                                                           *
 * This class is for the interaction with Database           *
 * in contex with Employee relevant data manipulation. Like: *
 * Creating, Updating the employee. Creating and editing cus-*
 * tom fields and likewise some other operations.            *
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlEmployeeRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public bool VerifyUser(string pUserName, string pPassword, out long pEmpId, out string pEmpName, out string pEmpDesignation)
        {
            bool verified = false;
            long tempID = 0;
            pEmpId = -1;
            pEmpName = "";
            pEmpDesignation = "";

            // Searching the User Name and getting the tuple against that record if any.
            var idFromName = from x in _objectModel.EmpAttributes
                             where x.EmpAttName == "User Name"
                             from y in _objectModel.EmpAttValues
                             where y.EmpAttID == x.EmpAttID && y.Value == pUserName
                             select y;

            // Ths above sql query is not case sensitve. But the user name is
            // case sensitve so verifing the user name with case sensitive comparison.
            foreach (var obj in idFromName)
            {
                if (obj.Value == pUserName)
                {
                    verified = true;
                    tempID = obj.EmpID;
                }
            }

            if (verified)
            {
                verified = false;
                // Getting the password against the EmpID found in above query.
                var idFromPassword = from x in _objectModel.EmpAttributes
                                     where x.EmpAttName == "Password"
                                     from y in _objectModel.EmpAttValues
                                     where y.EmpAttID == x.EmpAttID && y.EmpID == tempID && y.Value == pPassword
                                     select y;

                // Ths above sql query is not case sensitve. But the password is
                // case sensitve so verifing the password with case sensitive comparison.
                foreach (var obj in idFromPassword)
                {
                    if (obj.Value == pPassword)
                    {
                        verified = true;
                    }
                }

                if (verified)
                {
                    // Getting employee name.
                    var empName = from x in _objectModel.EmpAttributes
                                  where x.EmpAttName == "Employee Name"
                                  from y in _objectModel.EmpAttValues
                                  where (y.EmpAttID == x.EmpAttID) && (y.EmpID == tempID)
                                  select y;

                    // Getting employee designation.
                    var empDesignation = from x in _objectModel.EmpAttributes
                                         where x.EmpAttName == "Role"
                                         from y in _objectModel.EmpAttValues
                                         where (y.EmpAttID == x.EmpAttID) && (y.EmpID == tempID)
                                         select y;

                    pEmpName = empName.First().Value;
                    pEmpDesignation = empDesignation.First().Value;
                    pEmpId = tempID;
                }
            }

            return verified;
        }

        public bool AddEmployee(string[] pAttValues, int[] pAttIDs)
        {
            int index = 0;
            long nextID = 1;
            bool result = false;

            if (_objectModel.Employees.FirstOrDefault() != null)
            {
                var maxID = (from x in _objectModel.Employees
                             select x).Max(y => y.EmpID);

                nextID = maxID + 1;
            }

            _objectModel.Employees.InsertOnSubmit(new Employee
            {
                EmpID = nextID
            }
                                                );

            foreach (int id in pAttIDs)
            {
                _objectModel.EmpAttValues.InsertOnSubmit(new EmpAttValue
                {
                    EmpAttID = id,
                    EmpID = nextID,
                    Value = pAttValues[index++]
                }
                                                         );
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

        public bool EditEmployee(string[] pAttValues, int[] pAttIDs, long pEmployeeID)
        {
            int index = 0;
            bool result = false;

            var employee = from x in _objectModel.EmpAttValues
                           where x.EmpID == pEmployeeID
                           select x;

            foreach (int id in pAttIDs)
            {
                employee.Where(x => x.EmpAttID == id).First().Value = pAttValues[index++];
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

        public bool IsUserNameExist(string pUserName)
        {
            var isExist = from x in _objectModel.EmpAttributes
                          where x.EmpAttName == "User Name"
                          from y in _objectModel.EmpAttValues
                          where x.EmpAttID == y.EmpAttID && y.Value == pUserName
                          select y;
            if (isExist.Count() > 0)
                return true;
            return false;
        }

        public int CreateCustomField(string[] data)
        {
            int result = -1;

            var attribute = from x in _objectModel.EmpAttributes
                            where x.EmpAttName == data[0]
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                // If regular expression is not selected then insert null in DB.
                // As the RegularExpression is of type int?, so, it can save null.
                int? nullValue = null;
                int fieldTypeID = Convert.ToInt32(data[1]);

                // If field type is check box or list then some values are null and 
                // need to pass manually.
                if (fieldTypeID != 4 && fieldTypeID != 5)
                {
                    _objectModel.EmpAttributes.InsertOnSubmit(new EmpAttribute
                    {
                        EmpAttName = data[0],
                        FieldType = fieldTypeID,
                        DefaultValue = (data[2] == "") ? null : data[2],
                        CanNull = Convert.ToBoolean(data[3]),
                        RegularExpression = (data[4] == "") ? nullValue : Convert.ToInt32(data[4]),
                        IsSystemLevel = Convert.ToBoolean(data[5])
                    }
                    );
                }

                // For check box and list.
                else
                {
                    _objectModel.EmpAttributes.InsertOnSubmit(new EmpAttribute
                    {
                        EmpAttName = data[0],
                        FieldType = fieldTypeID,
                        // If check box then put null in default value.
                        DefaultValue = (fieldTypeID == 5) ? null : data[2],
                        CanNull = false,
                        RegularExpression = nullValue,
                        IsSystemLevel = Convert.ToBoolean(data[5])
                    }
                    );
                }

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

        // User can only change the Field Name property. And, if the field is of
        // type list then user can specify new option(s) for list which will be added
        // in existing list. So, third parameter is for that purpose.
        public int EditField(int pFieldID, string pFieldName, string pListOptions)
        {
            int result = -1;

            var attribute = from x in _objectModel.EmpAttributes
                            where x.EmpAttName == pFieldName && x.EmpAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                _objectModel.EmpAttributes.Where(x => x.EmpAttID == pFieldID).First();

                var empAtt = (from x in _objectModel.EmpAttributes
                              where x.EmpAttID == pFieldID
                              select x).First();

                empAtt.EmpAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    empAtt.DefaultValue = empAtt.DefaultValue + ";" + pListOptions;
                }

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

        public bool IsFieldExist(string pFieldName, int pFieldID)
        {
            IQueryable<EmpAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.EmpAttributes
                        where x.EmpAttName == pFieldName
                        select x;
            }

            else
            {
                field = from x in _objectModel.EmpAttributes
                        where x.EmpAttName == pFieldName && x.EmpAttID != pFieldID
                        select x;
            }

            return (field.Count() != 0);
        }

        public bool ChangePassword(long pEmpID, string pNewPassword)
        {
            var passwords = from x in _objectModel.EmpAttributes
                            where x.EmpAttName == "Password"
                            from y in _objectModel.EmpAttValues
                            where y.EmpAttID == x.EmpAttID && y.EmpID == pEmpID
                            select y;
            // Ths above sql query is not case sensitve. But the password is
            // case sensitve so verifing the password with case sensitive comparison.
            foreach (var obj in passwords)
            {
                if (obj.Value == pNewPassword)
                {
                    return false;
                }
            }

            passwords.First().Value = pNewPassword;
            try
            {
                _objectModel.SubmitChanges();
                return true;
            }

            catch (Exception ex)
            {
            }
            return false;
        }

        public List<string> RightsList(long pEmpId)
        {
            var rightsList = from v in _objectModel.EmpAttributes
                             where v.EmpAttName == "Role"                           //Getting the Role attribute
                             from w in _objectModel.EmpAttValues
                             where w.EmpAttID == v.EmpAttID && w.EmpID == pEmpId    //Getting the Role of Spacific User
                             from x in _objectModel.Roles
                             where x.RoleName == w.Value                            //Getting the ID of Role
                             from y in _objectModel.RoleRights
                             where y.RoleID == x.RoleID                             //Getting the Rights of Spacific Role
                             from z in _objectModel.AccessRights
                             where z.RightID == y.RightID                           //Getting the Description of each Right
                             select z.Value;

            return (rightsList).ToList();
        }

        public string[] GetRoleRights(string pRoleName)
        {
            int roleID = (from x in _objectModel.Roles
                         where x.RoleName == pRoleName
                         select x.RoleID).First();

            var rightIDS = (from x in _objectModel.RoleRights
                           where x.RoleID == roleID
                           select x.RightID.ToString()).ToArray();

            return rightIDS;
        }

        public bool HasRoleRight(string pRoleName, int pRightID)
        {
            var roleID = from x in _objectModel.Roles
                         where x.RoleName == pRoleName
                         from y in _objectModel.RoleRights
                         where y.RoleID == x.RoleID && y.RightID == pRightID
                         select y.RoleID;


            return (roleID.Count() > 0);
        }

        public IQueryable<EmpAttValue> GetEmployees()
        {
            var employee = from x in _objectModel.EmpAttributes
                           where x.EmpAttName == "Employee Name"
                           from y in _objectModel.EmpAttValues
                           where y.EmpAttID == x.EmpAttID
                           select y;
            return employee;
        }

        // Returns only those employees who are not working on given issue.
        public IQueryable<EmpAttValue> GetEmployeesFilteredList(long pIssueID)
        {
            var empIDs = (from x in _objectModel.AssignedIssues
                         where x.IssueID == pIssueID
                         select x.EmpID).ToList();

            var employee = from x in _objectModel.EmpAttributes
                           where x.EmpAttName == "Employee Name"
                           from y in _objectModel.EmpAttValues
                           where (!empIDs.Contains(y.EmpID)) && y.EmpAttID == x.EmpAttID
                           select y;

            return employee;
        }

        public IQueryable<FieldType> GetFieldTypes()
        {
            return _objectModel.FieldTypes;
        }

        public IQueryable<RegularExpression> GetRegularExpressions()
        {
            return _objectModel.RegularExpressions;
        }

        // Returns all system level attributes and those custom level attributes which are
        // present in "pCustomAttributes" list.
        public IQueryable<EmpAttribute> GetEmployeeAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes != null && pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.EmpAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.EmpAttID)
                                  select x).OrderBy(x => x.FieldType1.FieldID);
                return attributes;
            }

            else
            {
                return _objectModel.EmpAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType1.FieldID);
            }
        }

        public IQueryable<EmpAttribute> GetEmployeeAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.EmpAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                var attributes = from x in _objectModel.EmpAttributes
                                 where x.EmpAttName != "Password"
                                 select x;
                return attributes;
            }
        }

        public IQueryable<EmpAttValue> GetEmployeeByID(long pEmployeeID, bool flag)
        {
            // if flag is true then employee information is getting for updation.
            // otherwise for view the of employee information.
            if (flag)
            {
                var passwordFeildId = from x in _objectModel.EmpAttributes
                                      where x.EmpAttName == "Password" || x.EmpAttName == "User Name"
                                      select x.EmpAttID;

                var employee = (from x in _objectModel.EmpAttValues
                                where x.EmpID == pEmployeeID && !passwordFeildId.Contains(x.EmpAttID)
                                select x).OrderBy(x => x.EmpAttID);
                return employee;
            }
            else
            {
                var passwordFeildId = from x in _objectModel.EmpAttributes
                                      where x.EmpAttName == "Password"
                                      select x.EmpAttID;

                var employee = (from x in _objectModel.EmpAttValues
                                where x.EmpID == pEmployeeID && !passwordFeildId.Contains(x.EmpAttID)
                                select x).OrderBy(x => x.EmpAttID);
                return employee;
            }
        }

        public string GetEmployeeName(long pEmployeeID)
        {
            // Getting employee name.
            var empName = from x in _objectModel.EmpAttributes
                          where x.EmpAttName == "Employee Name"
                          from y in _objectModel.EmpAttValues
                          where (y.EmpAttID == x.EmpAttID) && (y.EmpID == pEmployeeID)
                          select y;

            return empName.First().Value;
        }

        public List<string> EmployeesNames()
        {
            var Names = from x in _objectModel.EmpAttributes
                        where x.EmpAttName == "Employee Name"
                        from y in _objectModel.EmpAttValues
                        where y.EmpAttID == x.EmpAttID
                        select y.Value;

            return Names.ToList();
        }

        public EmpAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.EmpAttributes.Where(x => x.EmpAttID == pFieldID).First();
        }

        public IQueryable<EmpAttValue> SearchEmployee(int pFieldID, string pFieldValue)
        {
            int fieldType = 0;

            // The value 0 is for search by Employee ID.
            // For this, we have to query with Employees' table
            // rather than with EmpAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.EmpAttributes
                             where f.EmpAttID == pFieldID
                             select f.FieldType).First();
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                var employeeIDs = from x in _objectModel.EmpAttValues
                                  where x.EmpAttID == pFieldID && x.Value == null
                                  select x.EmpID;

                if (employeeIDs.Count() > 0)
                {
                    var employeesNames = from x in _objectModel.EmpAttributes
                                         where x.EmpAttName == "Employee Name"
                                         from y in _objectModel.EmpAttValues
                                         where y.EmpAttID == x.EmpAttID && employeeIDs.Contains(y.EmpID)
                                         select y;
                    return employeesNames;
                }
            }

            else
            {
                IQueryable<long> employeeIDs;

                // If pFieldID = 0 then this means the search is on Employee ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);
                    employeeIDs = from x in _objectModel.Employees
                                  where x.EmpID == id
                                  select x.EmpID;
                }

                else
                {
                    employeeIDs = from x in _objectModel.EmpAttValues
                                  where x.EmpAttID == pFieldID && x.Value == pFieldValue
                                  select x.EmpID;
                }

                if (employeeIDs.Count() > 0)
                {
                    var employeesNames = from x in _objectModel.EmpAttributes
                                         where x.EmpAttName == "Employee Name"
                                         from y in _objectModel.EmpAttValues
                                         where y.EmpAttID == x.EmpAttID && employeeIDs.Contains(y.EmpID)
                                         select y;

                    return employeesNames;
                }
            }
            return null;
        }

        public IQueryable<EmpAttValue> ViewProfile(long pEmployeeID)
        {
            var passwordFeildId = from x in _objectModel.EmpAttributes
                                  where x.EmpAttName == "Password"
                                  select x.EmpAttID;
            var employee = (from x in _objectModel.EmpAttValues
                            where x.EmpID == pEmployeeID && !passwordFeildId.Contains(x.EmpAttID)
                            select x).OrderBy(x => x.EmpAttID);
            return employee;
        }

        public IQueryable<EmpAttValue> EditProfile(long pEmployeeID)
        {
            var feildIds = from x in _objectModel.EmpAttributes
                           where x.EmpAttName == "Password" || x.EmpAttName == "User Name" || x.EmpAttName == "Role"
                           select x.EmpAttID;
            var profile = (from x in _objectModel.EmpAttValues
                           where x.EmpID == pEmployeeID && !feildIds.Contains(x.EmpAttID)
                           select x).OrderBy(x => x.EmpAttID);
            return profile;
        }

        public int ChangePassword(long pEmpID, string pOldPassword, string pNewPassword)
        {
            var pass = from x in _objectModel.EmpAttributes
                       where x.EmpAttName == "Password"
                       from y in _objectModel.EmpAttValues
                       where y.EmpAttID == x.EmpAttID && y.EmpID == pEmpID
                       select y;
            if (pass.Count() < 1)
                return -1;
            if (pass.First().Value == pOldPassword)
                pass.First().Value = pNewPassword;
            else
                return 0;
            try
            {
                _objectModel.SubmitChanges();
                return 1;
            }

            catch (Exception ex)
            {
            }
            return -1;
        }

        public IQueryable<PersonalNote> GetNotes(long pEmpID)
        {
            return _objectModel.PersonalNotes.Where(x => x.EmpID == pEmpID);
        }

        public PersonalNote GetNoteByID(long pNoteID)
        {
            return _objectModel.PersonalNotes.Where(x => x.NoteID == pNoteID).First();
        }

        public bool CreateNote(string pSubject, string pBody, long pEmpID)
        {
            bool result = false;

            _objectModel.PersonalNotes.InsertOnSubmit(new PersonalNote
                {
                    EmpID = pEmpID,
                    Subject = pSubject,
                    Body = pBody,
                    CreationDate = DateTime.Now
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

        public bool EditNote(string pSubject, string pBody, long pNoteID)
        {
            bool result = false;

            var note = (from x in _objectModel.PersonalNotes
                       where x.NoteID == pNoteID
                       select x).First();

            note.Body = pBody;
            note.Subject = pSubject;

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

        public bool DeleteNote(long pNoteID)
        {
            bool result = true;

            var note = (from x in _objectModel.PersonalNotes
                        where x.NoteID == pNoteID
                        select x).First();

            _objectModel.PersonalNotes.DeleteOnSubmit(note);

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

        // This function will return a string containing employees' names 
        // and IDs separated with ; who are working on the given issue.
        public List<string> GetEmployeesNames(long pIssueID)
        {
            List<string> result = new List<string>();

            var empIDs = from x in _objectModel.AssignedIssues
                         where x.IssueID == pIssueID
                         select x.EmpID;

            if (empIDs.Count() > 0)
            {
                var empNames = from x in _objectModel.EmpAttributes
                               where x.EmpAttName == "Employee Name"
                               from y in _objectModel.EmpAttValues
                               where empIDs.Contains(y.EmpID) && y.EmpAttID == x.EmpAttID
                               select y;

                foreach (var emp in empNames)
                {
                    result.Add(emp.Value + ", ID: " + emp.EmpID);
                }
            }

            return result;
        }

        public IQueryable<EmpAttValue> GetEmployees(long pIssueID)
        {
            var empIDs = (from x in _objectModel.AssignedIssues
                         where x.IssueID == pIssueID
                         select x.EmpID).ToList();

            if (empIDs.Count() > 0)
            {
                var empNames = from x in _objectModel.EmpAttributes
                               where x.EmpAttName == "Employee Name"
                               from y in _objectModel.EmpAttValues
                               where empIDs.Contains(y.EmpID) && y.EmpAttID == x.EmpAttID
                               select y;

                return empNames;
            }

            return null;
        }

        // Returns the workload of employee in the specified sprint.
        public double GetEmployeeWorkLoad(long pEmpID, long pSprintID)
        {
            var issuesinSprint = (from x in _objectModel.SprintsBacklogs
                                  where x.SprintID == pSprintID
                                  select x.IssueID).ToList();
 
            var issueIDs = (from x in _objectModel.AssignedIssues
                           where x.EmpID == pEmpID && issuesinSprint.Contains(x.IssueID)
                           select x.IssueID).ToList();

            var issues = (from x in _objectModel.IssueAttributes
                           where x.IssueAttName == "Man Hours"
                           from y in _objectModel.IssueAttValues
                           where issueIDs.Contains(y.IssueID) && y.IssueAttID == x.IssueAttID
                           select y).ToList();

            double totalManHours = 0;
            int emps = 1;

            foreach (var issue in issues)
            {
                // If the issue is assigned more than one user then divide the work load.
                emps = _objectModel.AssignedIssues.Where( x => x.IssueID == issue.IssueID).Select(y => y.EmpID).Count();
                totalManHours += double.Parse(issue.Value) / emps;
            }

            return totalManHours; 
        }

        // Returns those users who are in the specified sprint and the have no meeting
        // on the given date if "pDate" is not null.
        public IQueryable<EmpAttValue> GetSprintTeam(long pSprintID, DateTime? pDate)
        {
            var issueIDs = (from x in _objectModel.SprintsBacklogs
                           where x.SprintID == pSprintID
                           select x.IssueID).ToList();

            if (issueIDs.Count > 0)
            {
                List<long> empIDs;

                if (pDate != null)
                {
                    // IDs of those employees who have the meeting on the specified date.
                    var doneMeetingEmpIDs = (from x in _objectModel.ScrumMeetings
                                             where (DateTime.Compare(x.MeetingDate, (DateTime)pDate) == 0) && x.SprintID == pSprintID
                                             from y in _objectModel.MeetingDetails
                                             where y.MeetingID == x.MeetingID
                                             select y.EmpID).ToList();

                    empIDs = (from x in _objectModel.AssignedIssues
                              where issueIDs.Contains(x.IssueID) && !(doneMeetingEmpIDs.Contains(x.EmpID))
                              select x.EmpID).Distinct().ToList();
                }

                else
                {
                    empIDs = (from x in _objectModel.AssignedIssues
                              where issueIDs.Contains(x.IssueID)
                              select x.EmpID).Distinct().ToList();
                }

                if (empIDs.Count() > 0)
                {
                    var empNames = from x in _objectModel.EmpAttributes
                                   where x.EmpAttName == "Employee Name"
                                   from y in _objectModel.EmpAttValues
                                   where empIDs.Contains(y.EmpID) && y.EmpAttID == x.EmpAttID
                                   select y;

                    return empNames;
                }
            }
            return null;
        }
    }
}