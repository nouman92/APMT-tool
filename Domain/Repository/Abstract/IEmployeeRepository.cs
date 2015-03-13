using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Employee relevant data manipulation. Please see *
 * "SqlEmployeeRepository" class for further details.          * 
 *                                                            *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface IEmployeeRepository
    {
        bool VerifyUser(string pUserName, string pPassword, out long pEmpId, out string pEmpName, out string pEmpDesignation);
        List<string> RightsList(long pEmpId);
        bool AddEmployee(string[] pAttValues, int[] pAttIDs);
        bool EditEmployee(string[] pAttValues, int[] pAttIDs, long pEmployeeID);
        bool IsUserNameExist(string pUserName);
        int CreateCustomField(string[] data);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        bool IsFieldExist(string pFieldName, int pFieldID);
        bool ChangePassword(long pEmpID, string pNewPassword);
        IQueryable<EmpAttribute> GetEmployeeAttributes(List<int> pCustomAttributes);
        IQueryable<EmpAttribute> GetEmployeeAttributes(bool pOnlyCustomLevel);
        IQueryable<EmpAttValue> GetEmployees();
        IQueryable<EmpAttValue> GetEmployeesFilteredList(long pIssueID);
        string GetEmployeeName(long pEmployeeID);
        List<string> EmployeesNames();
        IQueryable<EmpAttValue> GetEmployeeByID(long pEmployeeID, bool flag);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        EmpAttribute GetFieldByID(int pFieldID);
        IQueryable<EmpAttValue> SearchEmployee(int pFieldID, string pFieldValue);
        IQueryable<EmpAttValue> ViewProfile(long pEmployeeID);
        IQueryable<EmpAttValue> EditProfile(long pEmployeeID);
        int ChangePassword(long pEmpID, string pOldPassword, string pNewPassword);
        IQueryable<PersonalNote> GetNotes(long pEmpID);
        PersonalNote GetNoteByID(long pNoteID);
        bool CreateNote(string pSubject, string pBody, long pEmpID);
        bool EditNote(string pSubject, string pBody, long pNoteID);
        bool DeleteNote(long pNoteID);
        List<string> GetEmployeesNames(long pIssueID);
        string[] GetRoleRights(string pRoleName);
        bool HasRoleRight(string pRoleName, int pRightID);
        IQueryable<EmpAttValue> GetEmployees(long pIssueID);
        double GetEmployeeWorkLoad(long pEmpID, long pSprintID);
        IQueryable<EmpAttValue> GetSprintTeam(long pSprintID, DateTime? pDate);
    }
}
