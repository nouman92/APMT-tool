using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface -------------------*
 *                                                           *
 * This interface is for the interaction with Database in    *
 * contex with Issue relevant data manipulation. Please see  *
 * "SqlIssueRepository" class for further details.           * 
 *                                                           *
 *---------------------------------------------------------- */

namespace Domain.Repository.Abstract
{
    public interface IIssueRepository
    {
        IQueryable<IssueAttValue> GetAssignedIssues(long pEmpID);
        IQueryable<IssueAttValue> GetIssueByID(long pIssueID, out string projectName, out string sprintName);
        IQueryable<IssueAttValue> GetIssues(List<long> pIssuesIDs, int pIssueState = 0, int pIssueType = 0, int pIssuePriority = 0, int pAssigned = 0);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        bool EditIssue(string[] pAttValues, int[] pAttIDs, long pIssueID, int[] pOtherInfo);
        IQueryable<IssueAttribute> GetIssueAttributes(List<int> pCustomAttributes);
        bool CreateIssue(string[] pAttValues, int[] pAttIDs, long pProjectID, int[] pOtherInfo);
        int AddIssueType(string pTypeName);
        IQueryable<IssuePriority> GetIssuePriorityList();
        IQueryable<IssueAttribute> GetIssueAttributes(bool pOnlyCustomLevel);
        bool IsFieldExists(string pFieldName, int pFieldID);
        int CreateCustomField(string[] data);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        IssueAttribute GetFieldByID(int pFieldID);
        IQueryable<IssueWorkFlow> GetIssueStateList(bool pActiveOnly = false, int pIncludeID = 0);
        IQueryable<IssueType> GetIssueTypeList();
        IQueryable<IssueAttValue> GetIssueDependency(long pIssueID);
        List<string> GetIssueIDs(long pSearchID, long pProjectID);
        string GetIssueName(long pIssueID);
        int AddIssueDependency(long pIssueID, long pDependsOnID, long pProjectID);
        bool AddIssueDependency(long pIssueID, List<long> pDependsOnIDs);
        int AddIssueInSprint(long pSprintID, long pIssueID, long pProjectID);
        bool AddIssueInSprint(long pSprintID, List<long> pIssueIDs);
        bool RemoveIssueDependency(long pIssueID, long pDependnOnID);
        IQueryable<IssueAttValue> GetIssuesforDependency(List<long> pExceptIDs, long pProjectID);
        IQueryable<IssueAttValue> GetIssuesforSprint(List<long> pExceptIDs, long pProjectID);
        bool UpdateWorkFlow(int[] pStatesIDs, int[] pStatesRanks, bool[] pActive);
        int AddIssueState(string pStateName);
        int EditState(int pStateID, string pStateName);
        bool ExecuteWorkFlow(long pIssueID, long pEmpID);
        int WorkDone(long pIssueID, double pHours);
        double GetIssueManHours(long pIssueID);
        List<long> SearchProjBacklogIssue(int pFieldID, string pFieldValue, long pProjectID);
        List<long> SearchSprintBacklogIssue(int pFieldID, string pFieldValue, long pProjectID);
        bool RemoveSprintIssue(long pIssueID, long pSprintID);
        bool RemoveIssueAssignee(long pIssueID, long pEmpID);
        List<long> GetAllSprintsIssues(long pProjectID);
        bool IsIssueCompleted(long pIssueID);
        IQueryable<IssueAttValue> CheckIssueDependency(long pIssueID);
        bool AssignIssue(long pIssueID, List<long> pEmpsIDs);
    }
}
