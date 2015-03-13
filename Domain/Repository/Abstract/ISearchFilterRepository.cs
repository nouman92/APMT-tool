using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Search Filters relevant data manipulation.     *
 * Please see "SqlSearchFiltersRepository" class for further  * 
 * details.                                                   *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface ISearchFilterRepository
    {
        bool CreateFilter(string pFilterName, string pFilterData, long pEmpID);
        bool IsFilterNameExist(string pFilterName, long pEmpID);
        bool RemoveFilter(long pFilterID, long pEmpID);
        IEnumerable<SearchFilter> GetFilters(long pEmpID);
        EmpAttribute GetEmployeeField(string pSelectedAttribute);
        ProjAttribute GetProjectField(string pSelectedAttribute);
        IssueAttribute GetIssueField(string pSelectedAttribute);
        CompAttribute GetComponentField(string pSelectedAttribute);
        SubCompAttribute GetSubComponentField(string pSelectedAttribute);
        SprintAttribute GetSprintField(string pSelectedAttribute);
        string GetRiskCategories();
        List<string> GetEmployeeAttributes();
        List<string> GetProjectAttributes();
        List<string> GetSprintAttributes();
        List<string> GetIssueAttributes();
        List<string> GetComponentAttributes();
        List<string> GetSubComponentAttributes();
        IEnumerable<EmpAttValue> SearchEmployee(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
        IEnumerable<ProjAttValue> SearchProject(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
        IEnumerable<SprintAttValue> SearchSprint(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
        IEnumerable<IssueAttValue> SearchIssue(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
        IEnumerable<CompAttValue> SearchComponent(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
        IEnumerable<SubCompAttValue> SearchSubComponent(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType);
    }
}
