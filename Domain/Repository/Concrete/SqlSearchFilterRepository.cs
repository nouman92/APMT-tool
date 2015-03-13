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
 * in contex with Search Filters relevant data manipulation. *
 * Like: Searching a Employee, Project, Issue etc.           *
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlSearchFilterRepository : ISearchFilterRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlSearchFilterRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public bool CreateFilter(string pFilterName, string pFilterData, long pEmpID)
        {
            SearchFilter searchFilter = new SearchFilter() { FilterName=pFilterName,Value=pFilterData,EmpID=pEmpID};
            _objectModel.SearchFilters.InsertOnSubmit(searchFilter);
            try
            {
                _objectModel.SubmitChanges();
                return true;
            }
            catch { }
            return false;
        }

        public bool IsFilterNameExist(string pFilterName, long pEmpID)
        {
            var filters = from x in _objectModel.SearchFilters
                          where x.FilterName == pFilterName && x.EmpID == pEmpID
                          select x;
            if (filters.Count() == 0)
                return false;
            else
                return true;
        }

        public bool RemoveFilter(long pFilterID, long pEmpID)
        {
            var filter = from x in _objectModel.SearchFilters
                         where x.FilterID == pFilterID && x.EmpID == pEmpID
                         select x;
            if (filter.Count() == 0)
                return false;
            else
            {
                _objectModel.SearchFilters.DeleteOnSubmit(filter.First());
                try
                {
                    _objectModel.SubmitChanges();
                    return true;
                }
                catch{}
            }
            return false;
        }
        
        public IEnumerable<SearchFilter> GetFilters(long pEmpID)
        {
            var searchFilter = from x in _objectModel.SearchFilters
                               where x.EmpID == pEmpID
                               select x;
            return searchFilter.ToList();
        }

        public EmpAttribute GetEmployeeField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.EmpAttributes
                       where x.EmpAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public ProjAttribute GetProjectField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.ProjAttributes
                       where x.ProjAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public IssueAttribute GetIssueField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.IssueAttributes
                       where x.IssueAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public SprintAttribute GetSprintField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.SprintAttributes
                       where x.SprintAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public CompAttribute GetComponentField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.CompAttributes
                       where x.CompAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public SubCompAttribute GetSubComponentField(string pSelectedAttribute)
        {
            var data = from x in _objectModel.SubCompAttributes
                       where x.SubCompAttName == pSelectedAttribute
                       select x;
            return data.First();
        }

        public string GetRiskCategories()
        {
            var risk = from x in _objectModel.RiskCategories
                       select x.CategoryName;
            string riskCategories = "";
            foreach (string c in risk)
            {
                riskCategories += c + ";";
            }
            return riskCategories.Substring(0, riskCategories.Length - 1);
        }
                
        public List<string> GetEmployeeAttributes()
        {
            var emp = from x in _objectModel.EmpAttributes
                      where x.FieldType!=2
                      select x.EmpAttName;
            return emp.ToList();
        }

        public List<string> GetProjectAttributes()
        {
            var proj = from x in _objectModel.ProjAttributes
                       where x.FieldType != 2
                       select x.ProjAttName;

            return proj.ToList();
        }

        public List<string> GetSprintAttributes()
        {
            var sprint = from x in _objectModel.SprintAttributes
                       where x.FieldType != 2
                       select x.SprintAttName;
            return sprint.ToList();
        }

        public List<string> GetIssueAttributes()
        {
            var issue = from x in _objectModel.IssueAttributes
                        where x.FieldType != 2
                        select x.IssueAttName;
            return issue.ToList();
        }

        public List<string> GetComponentAttributes()
        {
            var comp = from x in _objectModel.CompAttributes
                       where x.FieldType != 2
                       select x.CompAttName;
            return comp.ToList();
        }

        public List<string> GetSubComponentAttributes()
        {
            var subComp = from x in _objectModel.SubCompAttributes
                          where x.FieldType != 2
                          select x.SubCompAttName;
            return subComp.ToList();
        }

        public IEnumerable<EmpAttValue> SearchEmployee(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> employeeIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (int i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Employee")
                {
                    tempIds = SearchEmployee(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }
                else if (pSearchBy[i] == "Project")
                {
                    List<long> projIds = new List<long>();
                    projIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var empIds = from x in _objectModel.ProjectsBacklogs
                                 where projIds.Contains(x.ProjID)
                                 from y in _objectModel.AssignedIssues
                                 where x.IssueID == y.IssueID
                                 select y.EmpID;
                    tempIds = empIds.ToList();
                }
                else if (pSearchBy[i] == "Sprint")
                {
                    List<long> sprintIds = SearchSprint(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var empIds = from x in _objectModel.SprintsBacklogs
                                 where sprintIds.Contains(x.SprintID)
                                 from y in _objectModel.AssignedIssues
                                 where x.IssueID == y.IssueID
                                 select y.EmpID;
                    tempIds = empIds.ToList();
                }
                else if (pSearchBy[i] == "Issue")
                {
                    List<long> issueIds = new List<long>();
                    issueIds = SearchIssue(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var empIds = from x in _objectModel.AssignedIssues
                                 where issueIds.Contains(x.IssueID)
                                 select x.EmpID;
                    tempIds = empIds.ToList();

                }
                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempEmployeeIds = new List<long>();
                    foreach (long empId in employeeIds)
                        if (tempIds.Contains(empId))
                            tempEmployeeIds.Add(empId);
                    employeeIds = tempEmployeeIds;
                }
                else
                    foreach (long empId in tempIds)
                        if (!employeeIds.Contains(empId))
                            employeeIds.Add(empId);
                employeeIds.Sort();
            }
            IEnumerable<EmpAttValue> Employee = GetEmployee(employeeIds);
            return Employee;
        }

        public IEnumerable<ProjAttValue> SearchProject(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> projectIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (long i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Employee")
                {
                    List<long> empIds = SearchEmployee(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var projIds = from x in _objectModel.AssignedIssues
                                  where empIds.Contains(x.EmpID)
                                  from y in _objectModel.ProjectsBacklogs
                                  where y.IssueID == x.IssueID
                                  select y.ProjID;
                    tempIds = projIds.ToList();
                }
                else if (pSearchBy[i] == "Project")
                {
                    tempIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }
                else if (pSearchBy[i] == "Sprint")
                {
                    List<long> sprintIds = SearchSprint(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var projIds = from x in _objectModel.Sprints
                                  where sprintIds.Contains(x.SprintID)
                                  select x.ProjID;
                    tempIds = projIds.ToList();
                }
                else if (pSearchBy[i] == "Issue")
                {
                    List<long> issueIds = SearchIssue(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var projIds = from x in _objectModel.ProjectsBacklogs
                                  where issueIds.Contains(x.IssueID)
                                  select x.ProjID;
                    tempIds = projIds.ToList();
                }
                else if (pSearchBy[i] == "Risk")
                {
                    tempIds = SearchRisk(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }
                else if (pSearchBy[i] == "Component")
                {
                    List<long> componentIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var projIds = from x in _objectModel.Components
                                  where componentIds.Contains(x.CompID)
                                  select x.ProjID;
                    tempIds = projIds.ToList();
                }

                else if (pSearchBy[i] == "SubComponent")
                {
                    List<long> subComponentIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var projIds = from x in _objectModel.SubComponents
                                  where subComponentIds.Contains(x.SubCompID)
                                  from y in _objectModel.Components
                                  where y.CompID == y.CompID
                                  select y.ProjID;
                    tempIds = projIds.ToList();
                }

                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempProjectIds = new List<long>();
                    foreach (long projId in projectIds)
                    {
                        if (tempIds.Contains(projId))
                            tempProjectIds.Add(projId);
                    }
                    projectIds = tempProjectIds;
                }
                else
                {
                    foreach (long projId in tempIds)
                    {
                        if (!projectIds.Contains(projId))
                            projectIds.Add(projId);
                    }
                }
                projectIds.Sort();
            }
            IEnumerable<ProjAttValue> Project = GetProject(projectIds);
            return Project;
        }

        public IEnumerable<SprintAttValue> SearchSprint(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> sprintIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (int i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Employee")
                {
                    List<long> empIds = SearchEmployee(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var spirIds = from x in _objectModel.AssignedIssues
                                  where empIds.Contains(x.EmpID)
                                  from y in _objectModel.SprintsBacklogs
                                  where y.IssueID == x.IssueID
                                  select y.SprintID;
                    tempIds = spirIds.ToList();
                }
                else if (pSearchBy[i] == "Project")
                {
                    List<long> projIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var spirIds = from x in _objectModel.Sprints
                                  where projIds.Contains(x.ProjID)
                                  select x.SprintID;
                    tempIds = spirIds.ToList();
                }
                else if (pSearchBy[i] == "Sprint")
                {
                    tempIds = SearchSprint(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }
                else if (pSearchBy[i] == "Issue")
                {
                    List<long> issueIds = SearchIssue(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var projIds = from x in _objectModel.SprintsBacklogs
                                  where issueIds.Contains(x.IssueID)
                                  select x.SprintID;
                    tempIds = projIds.ToList();
                }

                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempSprintIds = new List<long>();
                    foreach (long spriId in sprintIds)
                    {
                        if (tempIds.Contains(spriId))
                            tempSprintIds.Add(spriId);
                    }
                    sprintIds = tempSprintIds;
                }
                else
                {
                    foreach (long spriId in tempIds)
                    {
                        if (!sprintIds.Contains(spriId))
                            sprintIds.Add(spriId);
                    }
                }
                sprintIds.Sort();
            }
            IEnumerable<SprintAttValue> Sprint = GetSprint(sprintIds);
            return Sprint;
        }

        public IEnumerable<IssueAttValue> SearchIssue(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> issueIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (int i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Employee")
                {
                    List<long> issIds = SearchEmployee(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var issuIds = from x in _objectModel.AssignedIssues
                                  where issIds.Contains(x.EmpID)
                                  select x.IssueID;
                    tempIds = issuIds.ToList();
                }
                else if (pSearchBy[i] == "Project")
                {
                    List<long> projIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var issuIds = from x in _objectModel.ProjectsBacklogs
                                  where projIds.Contains(x.ProjID)
                                  select x.IssueID;
                    tempIds = issuIds.ToList();
                }
                else if (pSearchBy[i] == "Sprint")
                {
                    List<long> sprintIds = SearchSprint(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                    var issuIds = from x in _objectModel.SprintsBacklogs
                                  where sprintIds.Contains(x.SprintID)
                                  select x.IssueID;
                    tempIds = issuIds.ToList();
                }
                else if (pSearchBy[i] == "Issue")
                {
                    tempIds = SearchIssue(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }


                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempIssueIds = new List<long>();
                    foreach (long issuId in issueIds)
                    {
                        if (tempIds.Contains(issuId))
                            tempIssueIds.Add(issuId);
                    }
                    issueIds = tempIssueIds;
                }
                else
                {
                    foreach (long issuId in tempIds)
                    {
                        if (!issueIds.Contains(issuId))
                            issueIds.Add(issuId);
                    }
                }
                issueIds.Sort();
            }
            IEnumerable<IssueAttValue> Issue = GetIssue(issueIds);
            return Issue;
        }

        public IEnumerable<CompAttValue> SearchComponent(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> componentIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (int i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Project")
                {
                    List<long> projIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var compIds = from x in _objectModel.Components
                                  where projIds.Contains(x.ProjID)
                                  select x.CompID;
                    tempIds = compIds.ToList();
                }
                else if (pSearchBy[i] == "Component")
                {
                    tempIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }

                else if (pSearchBy[i] == "SubComponent")
                {
                    List<long> subComponentIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var compIds = from x in _objectModel.SubComponents
                                  where subComponentIds.Contains(x.SubCompID)
                                  select x.CompID;
                    tempIds = compIds.ToList();
                }

                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempCompIds = new List<long>();
                    foreach (long compId in componentIds)
                    {
                        if (tempIds.Contains(compId))
                            tempCompIds.Add(compId);
                    }
                    componentIds = tempCompIds;
                }
                else
                {
                    foreach (long compId in tempIds)
                    {
                        if (!componentIds.Contains(compId))
                            componentIds.Add(compId);
                    }
                }
                componentIds.Sort();
            }
            IEnumerable<CompAttValue> Component = GetComponent(componentIds);
            return Component;
        }

        public IEnumerable<SubCompAttValue> SearchSubComponent(string[] pSearchBy, string[] pAttributes, string[] pValues, int[] pOperator, int[] pWildCard, int[] pValueType)
        {
            List<long> subComponentIds = new List<long>();
            List<long> tempIds = new List<long>();
            for (int i = 0; i < pSearchBy.Count(); i++)
            {
                if (pSearchBy[i] == "Project")
                {
                    List<long> projIds = SearchProject(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var subCompIds = from x in _objectModel.Components
                                     where projIds.Contains(x.ProjID)
                                     from y in _objectModel.SubComponents
                                     where y.CompID == x.CompID
                                     select y.SubCompID;
                    tempIds = subCompIds.ToList();
                }
                else if (pSearchBy[i] == "Component")
                {
                    List<long> compIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);

                    var subCompIds = from x in _objectModel.SubComponents
                                     where compIds.Contains(x.CompID)
                                     select x.SubCompID;
                    tempIds = subCompIds.ToList();
                }

                else if (pSearchBy[i] == "SubComponent")
                {
                    tempIds = SearchComponent(pAttributes[i], pValues[i], pWildCard[i], pValueType[i]);
                }

                // pOperator=1 For AND
                // pOperator=0 For OR
                int oper = i == 0 ? 0 : pOperator[i - 1];
                if (oper == 1)
                {
                    List<long> tempSubCompIds = new List<long>();
                    foreach (long subCompId in subComponentIds)
                    {
                        if (tempIds.Contains(subCompId))
                            tempSubCompIds.Add(subCompId);
                    }
                    subComponentIds = tempSubCompIds;
                }
                else
                {
                    foreach (long subCompId in tempIds)
                    {
                        if (!subComponentIds.Contains(subCompId))
                            subComponentIds.Add(subCompId);
                    }
                }
                subComponentIds.Sort();
            }
            IEnumerable<SubCompAttValue> SubComponent = GetSubComponent(subComponentIds);
            return SubComponent;
        }

        
        public IEnumerable<EmpAttValue> GetEmployee(List<long> pEmp)
        {
            if (pEmp.Count > 0)
            {
                var employee = from x in _objectModel.EmpAttributes
                               where x.EmpAttName == "Employee Name"
                               from y in _objectModel.EmpAttValues
                               where pEmp.Contains(y.EmpID) && x.EmpAttID == y.EmpAttID
                               select y;
                return employee;
            }
            return null;
        }

        public IEnumerable<ProjAttValue> GetProject(List<long> pProj)
        {
            if (pProj.Count > 0)
            {
                var project = from x in _objectModel.ProjAttributes
                              where x.ProjAttName == "Project Name"
                              from y in _objectModel.ProjAttValues
                              where pProj.Contains(y.ProjID) && x.ProjAttID == y.ProjAttID
                              select y;
                return project;
            }
            return null;
        }

        public IEnumerable<SprintAttValue> GetSprint(List<long> pSprint)
        {
            if (pSprint.Count > 0)
            {
                var sprint = from x in _objectModel.SprintAttributes
                             where x.SprintAttName == "Sprint Name"
                             from y in _objectModel.SprintAttValues
                             where pSprint.Contains(y.SprintID) && x.SprintAttID == y.SprintAttID
                             select y;
                return sprint;
            }
            return null;
        }

        public IEnumerable<IssueAttValue> GetIssue(List<long> pIssue)
        {
            if (pIssue.Count > 0)
            {
                var issue = from x in _objectModel.IssueAttributes
                            where x.IssueAttName == "Issue Name"
                            from y in _objectModel.IssueAttValues
                            where pIssue.Contains(y.IssueID) && x.IssueAttID == y.IssueAttID
                            select y;
                return issue;
            }
            return null;
        }

        public IEnumerable<CompAttValue> GetComponent(List<long> pComp)
        {
            if (pComp.Count > 0)
            {
                var component = from x in _objectModel.CompAttributes
                                where x.CompAttName == "Component Name"
                                from y in _objectModel.CompAttValues
                                where pComp.Contains(y.CompID) && x.CompAttID == y.CompAttID
                                select y;
                return component;
            }
            return null;
        }

        public IEnumerable<SubCompAttValue> GetSubComponent(List<long> pSubComp)
        {
            if (pSubComp.Count > 0)
            {
                var subComponent = from x in _objectModel.SubCompAttributes
                                   where x.SubCompAttName == "Sub-Component Name"
                                   from y in _objectModel.SubCompAttValues
                                   where pSubComp.Contains(y.SubCompID) && x.SubCompAttID == y.SubCompAttID
                                   select y;
                return subComponent;
            }
            return null;
        }

        public List<long> SearchEmployee(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Employee ID")
            {
                int value = Convert.ToInt32(pValues);
                var employeeIds = from x in _objectModel.Employees
                                  where pWildCard == 0 ? x.EmpID == value : pWildCard == 1 ? x.EmpID < value : x.EmpID > value
                                  select x.EmpID;
                return employeeIds.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var employeeIds = from x in _objectModel.EmpAttributes
                                      where x.EmpAttName == pAttributes
                                      from y in _objectModel.EmpAttValues
                                      where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.EmpAttID == y.EmpAttID
                                      select y.EmpID;
                    return employeeIds.ToList();
                }
                else
                {
                    var employeeIds = from x in _objectModel.EmpAttributes
                                      where x.EmpAttName == pAttributes
                                      from y in _objectModel.EmpAttValues
                                      where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.EmpAttID == y.EmpAttID
                                      select y.EmpID;
                    return employeeIds.ToList();
                }
            }
        }

        public List<long> SearchProject(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Project ID")
            {
                int value = Convert.ToInt32(pValues);
                var projectIds = from x in _objectModel.Projects
                                 where pWildCard == 0 ? x.ProjID == value : pWildCard == 1 ? x.ProjID < value : x.ProjID > value
                                 select x.ProjID;
                return projectIds.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var projectIds = from x in _objectModel.ProjAttributes
                                     where x.ProjAttName == pAttributes
                                     from y in _objectModel.ProjAttValues
                                     where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.ProjAttID == y.ProjAttID
                                     select y.ProjID;
                    return projectIds.ToList();
                }
                else
                {
                    var projectIds = from x in _objectModel.ProjAttributes
                                     where x.ProjAttName == pAttributes
                                     from y in _objectModel.ProjAttValues
                                     where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.ProjAttID == y.ProjAttID
                                     select y.ProjID;
                    return projectIds.ToList();
                }
            }
        }

        public List<long> SearchSprint(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Sprint ID")
            {
                int value = Convert.ToInt32(pValues);
                var sprintIDs = from x in _objectModel.Sprints
                                where pWildCard == 0 ? x.SprintID == value : pWildCard == 1 ? x.SprintID < value : x.SprintID > value
                                select x.SprintID;
                return sprintIDs.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var sprintIDs = from x in _objectModel.SprintAttributes
                                    where x.SprintAttName == pAttributes
                                    from y in _objectModel.SprintAttValues
                                    where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.SprintAttID == y.SprintAttID
                                    select y.SprintID;
                    return sprintIDs.ToList();
                }
                else
                {
                    var sprintIDs = from x in _objectModel.SprintAttributes
                                    where x.SprintAttName == pAttributes
                                    from y in _objectModel.SprintAttValues
                                    where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.SprintAttID == y.SprintAttID
                                    select y.SprintID;
                    return sprintIDs.ToList();
                }
            }
        }

        public List<long> SearchComponent(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Component ID")
            {
                int value = Convert.ToInt32(pValues);
                var componentIDs = from x in _objectModel.Components
                                   where pWildCard == 0 ? x.CompID == value : pWildCard == 1 ? x.CompID < value : x.CompID > value
                                   select x.CompID;
                return componentIDs.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var componentIDs = from x in _objectModel.CompAttributes
                                       where x.CompAttName == pAttributes
                                       from y in _objectModel.CompAttValues
                                       where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.CompAttID == y.CompAttID
                                       select y.CompID;
                    return componentIDs.ToList();
                }
                else
                {
                    var componentIDs = from x in _objectModel.CompAttributes
                                       where x.CompAttName == pAttributes
                                       from y in _objectModel.CompAttValues
                                       where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.CompAttID == y.CompAttID
                                       select y.CompID;
                    return componentIDs.ToList();
                }
            }
        }

        public List<long> SearchSubComponent(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "SubComponent ID")
            {
                int value = Convert.ToInt32(pValues);
                var subComponentIDs = from x in _objectModel.SubComponents
                                      where pWildCard == 0 ? x.SubCompID == value : pWildCard == 1 ? x.SubCompID < value : x.SubCompID > value
                                      select x.SubCompID;
                return subComponentIDs.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var subComponentIDs = from x in _objectModel.SubCompAttributes
                                          where x.SubCompAttName == pAttributes
                                          from y in _objectModel.SubCompAttValues
                                          where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.SubCompAttID == y.SubCompAttID
                                          select y.SubCompID;
                    return subComponentIDs.ToList();
                }
                else
                {
                    var subComponentIDs = from x in _objectModel.SubCompAttributes
                                          where x.SubCompAttName == pAttributes
                                          from y in _objectModel.SubCompAttValues
                                          where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.SubCompAttID == y.SubCompAttID
                                          select y.SubCompID;
                    return subComponentIDs.ToList();
                }
            }
        }

        public List<long> SearchIssue(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Issue ID")
            {
                int value = Convert.ToInt32(pValues);
                var issueIds = from x in _objectModel.Issues
                               where pWildCard == 0 ? x.IssueID == value : pWildCard == 1 ? x.IssueID < value : x.IssueID > value 
                               select x.IssueID;
                return issueIds.ToList();
            }
            else
            {
                if (pValueType == 3)
                {
                    var issueIds = from x in _objectModel.IssueAttributes
                                   where x.IssueAttName == pAttributes
                                   from y in _objectModel.IssueAttValues
                                   where pValues == "Yes" ? y.Value == "Yes" : y.Value == null && x.IssueAttID == y.IssueAttID
                                   select y.IssueID;
                    return issueIds.ToList();
                }
                else
                {
                    var issueIds = from x in _objectModel.IssueAttributes
                                   where x.IssueAttName == pAttributes
                                   from y in _objectModel.IssueAttValues
                                   where pWildCard == 0 ? y.Value == pValues : pWildCard == 1 ? y.Value.Contains(pValues) : pWildCard == 2 ? y.Value.StartsWith(pValues) : y.Value.EndsWith(pValues) && x.IssueAttID == y.IssueAttID
                                   select y.IssueID;
                    return issueIds.ToList();
                }
            }
        }

        public List<long> SearchRisk(string pAttributes, string pValues, int pWildCard, int pValueType)
        {
            if (pAttributes == "Risk ID")
            {
                int value = Convert.ToInt32(pValues);
                var riskIds = from x in _objectModel.ProjectRisks
                              where pWildCard == 0 ? x.RiskID == value : pWildCard == 1 ? x.RiskID < value : x.RiskID > value
                              select x.ProjID;
                return riskIds.ToList();
            }
            else if (pAttributes == "Category")
            {
                var riskIds = from x in _objectModel.RiskCategories
                              where x.CategoryName == pValues
                              from y in _objectModel.Risks
                              where x.CategoryID == y.Category
                              from z in _objectModel.ProjectRisks
                              where y.RiskID == z.RiskID
                              select z.ProjID;
                return riskIds.ToList();
            }
            else
            {
                int value = Convert.ToInt32(pValues);
                var riskIds = from x in _objectModel.ProjectRisks
                              where pWildCard == 0 ? (x.Probability * x.Impact) == value : pWildCard == 1 ? (x.Probability * x.Impact) < value : (x.Probability * x.Impact) > value
                              select x.ProjID;
                return riskIds.ToList();
            }
        }
    }
}
