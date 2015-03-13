using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Repository.Abstract;
using Domain.Entities;
using System.Data.Linq.SqlClient;
using System.Configuration;

/*-------------------- About this Class ---------------------*
 *                                                           *
 * This class is interface for the interaction with Database *
 * in contex with Issue relevant data manipulation. Like:    *
 * Creating Issue, Editing Issue, Assigning Task to employees*
 * setting Issue Dependency etc.                             *
 *                                                           *
 *---------------------------------------------------------- */

namespace Domain.Repository.Concrete
{
    public class SqlIssueRepository : IIssueRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlIssueRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public IQueryable<IssueAttValue> GetAssignedIssues(long pEmpID)
        {
            var empAssignedIssuesIDs = from x in _objectModel.AssignedIssues
                                       where x.EmpID == pEmpID
                                       select x.IssueID;

            var assignedIssues = from y in _objectModel.IssueAttributes
                                 where y.IssueAttName == "Issue Name"
                                 from z in _objectModel.IssueAttValues
                                 where (z.IssueAttID == y.IssueAttID && empAssignedIssuesIDs.Contains(z.IssueID))
                                 select z;

            return assignedIssues;
        }

        public IQueryable<IssueAttValue> GetIssueByID(long pIssueID, out string projectName, out string sprintName)
        {
            sprintName = null;

            var issue = from x in _objectModel.IssueAttValues
                        where x.IssueID == pIssueID
                        select x;

            var projectID = (from x in _objectModel.ProjectsBacklogs
                            where x.IssueID == issue.First().IssueID
                            select x.ProjID).First();

            projectName = (from x in _objectModel.ProjAttributes
                          where x.ProjAttName == "Project Name"
                          from y in _objectModel.ProjAttValues
                          where y.ProjAttID == x.ProjAttID && y.ProjID == projectID
                          select y.Value).First();

            var sprintID = from x in _objectModel.SprintsBacklogs
                             where x.IssueID == issue.First().IssueID
                             select x.SprintID;

            if (sprintID.Count() > 0)
            {
                sprintName = (from x in _objectModel.SprintAttributes
                               where x.SprintAttName == "Sprint Name"
                               from y in _objectModel.SprintAttValues
                              where y.SprintAttID == x.SprintAttID && y.SprintID == sprintID.First()
                               select y.Value).First();

            }

            return issue;
        }

        // The last three parameters are for filtration. 0 means any, no filtration on that attribute.
        public IQueryable<IssueAttValue> GetIssues(List<long> pIssuesIDs, int pIssueState = 0, int pIssueType = 0, int pIssuePriority = 0, int pAssigned = 0)
        {
            if (pIssuesIDs.Count > 0)
            {
                // Getting the "Issue Name" attribute id.
                var issueNameAttID = (from x in _objectModel.IssueAttributes
                                      where x.IssueAttName == "Issue Name"
                                     select x.IssueAttID).First();

                // Select only those issues which are assigned.
                if (pAssigned == 1)
                {
                    pIssuesIDs = (from x in _objectModel.AssignedIssues
                                 where pIssuesIDs.Contains(x.IssueID)
                                 select x.IssueID).ToList();
                }

                // Select only those issues which are unassigned.
                else if (pAssigned == 2)
                {
                    var assigned = (from x in _objectModel.AssignedIssues
                                    where pIssuesIDs.Contains(x.IssueID)
                                    select x.IssueID).ToList();

                    pIssuesIDs = (from x in pIssuesIDs
                                  where !(assigned.Contains(x))
                                  select x).ToList();
                }

                // Filtering the list according to the given values.
                var issuesIDs = (from x in _objectModel.Issues
                             where pIssuesIDs.Contains(x.IssueID) && ((pIssueState == 0) ? x.StateID > pIssueState : x.StateID == pIssueState) &&
                                   ((pIssueType == 0) ? x.TypeID > pIssueType : x.TypeID == pIssueType) &&
                                   ((pIssuePriority == 0) ? x.PriorityID > pIssuePriority : x.PriorityID == pIssuePriority)
                            select x.IssueID).ToList();

                // Getting issues' names.
                var issuesDetails =  (from x in _objectModel.IssueAttValues
                                      where issuesIDs.Contains(x.IssueID) && x.IssueAttID == issueNameAttID
                                      select x).OrderBy(y => y.Issue.PriorityID);

                if(issuesDetails.Count() > 0)
                {
                    return issuesDetails;
                }
    
                return null;
            }

            return null;
        }

        // Used in adding issue dependency to show the list for selection.
        public IQueryable<IssueAttValue> GetIssuesforDependency(List<long> pExceptIDs, long pProjectID)
        {
            if (pExceptIDs.Count() > 0)
            {
                // Getting the "Issue Name" attribute id.
                var issueNameAttID = (from x in _objectModel.IssueAttributes
                                      where x.IssueAttName == "Issue Name"
                                      select x.IssueAttID).First();

                // Filtering list.
                var issuesIDs = (from x in _objectModel.ProjectsBacklogs
                                 where x.ProjID == pProjectID && !(pExceptIDs.Contains(x.IssueID)) &&
                                 !((from y in _objectModel.IssuesDependencies  // filtering out cross dependency
                                    where y.IssueID == x.IssueID
                                    select y.DependsOn).Contains(pExceptIDs[0]))
                                 select x.IssueID).ToList();

                if (issuesIDs.Count() > 0)
                {
                    // Getting issues' names.
                    var issuesDetails = from y in _objectModel.IssueAttValues
                                        where issuesIDs.Contains(y.IssueID) && y.IssueAttID == issueNameAttID
                                        select y;

                    return issuesDetails;
                }
            }

            return null;
        }

        // Used in adding issues in Sprint Backlog to show the list for selection.
        public IQueryable<IssueAttValue> GetIssuesforSprint(List<long> pExceptIDs, long pProjectID)
        {
            // Getting the "Issue Name" attribute id.
            var issueNameAttID = (from x in _objectModel.IssueAttributes
                                    where x.IssueAttName == "Issue Name"
                                    select x.IssueAttID).First();

            List<long> issuesIDs;
            // Filtering list.
            if (pExceptIDs.Count() > 0)
            {
                issuesIDs = (from x in _objectModel.ProjectsBacklogs
                            where x.ProjID == pProjectID && !(pExceptIDs.Contains(x.IssueID))
                            select x.IssueID).ToList();
            }

            else 
            {
                issuesIDs = (from x in _objectModel.ProjectsBacklogs
                            where x.ProjID == pProjectID
                            select x.IssueID).ToList();
            }

            if (issuesIDs.Count() > 0)
            {
                // Getting issues' names.
                var issuesDetails = from y in _objectModel.IssueAttValues
                                    where issuesIDs.Contains(y.IssueID) && y.IssueAttID == issueNameAttID
                                    select y;

                return issuesDetails;
            }

            return null;
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
        public IQueryable<IssueAttribute> GetIssueAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes != null && pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.IssueAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.IssueAttID)
                                  select x).OrderBy(x => x.FieldType);

                return attributes;
            }

            else
            {
                return _objectModel.IssueAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType);
            }
        }

        public IQueryable<IssuePriority> GetIssuePriorityList()
        {
            return _objectModel.IssuePriorities;
        }

        // Will return the list of states which an issue can have.
        // The second parameter is for inactive state that should be
        // included in case of ActiveOnly.
        public IQueryable<IssueWorkFlow> GetIssueStateList(bool pActiveOnly = false, int pIncludeID = 0)
        {
            if (pActiveOnly)
            {
                return _objectModel.IssueWorkFlows.Where(x => x.Active == true || x.StateID == pIncludeID);
            }

            else
            {
                return _objectModel.IssueWorkFlows;
            }
        }

        public IQueryable<IssueType> GetIssueTypeList()
        {
            return _objectModel.IssueTypes;
        }

        public IQueryable<IssueAttribute> GetIssueAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.IssueAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                return _objectModel.IssueAttributes;
            }
        }

        // The last attribute "pOtherInfo" contains the issue type and priority ids.
        public bool CreateIssue(string[] pAttValues, int[] pAttIDs, long pProjectID, int[] pOtherInfo)
        {
            int index = 0, stateRank;
            long nextID = 1;
            bool result = false;

            if (_objectModel.Issues.FirstOrDefault() != null)
            {
                var maxID = (from x in _objectModel.Issues
                             select x).Max(y => y.IssueID);

                nextID = maxID + 1;
            }

            // Getting the minimum active state rank.
            stateRank = _objectModel.IssueWorkFlows.Where(x => x.Active == true).Select(y => y.StateRank).Min();

            _objectModel.Issues.InsertOnSubmit(new Issue
            {
                IssueID = nextID,
                // Setting issue state. It will be in active state with the minimum  rank.
                StateID = _objectModel.IssueWorkFlows.Where(x => x.StateRank == stateRank).Select( y => y.StateID).First(),
                TypeID = pOtherInfo[0],
                PriorityID = pOtherInfo[1]
            }
            );

            foreach (int id in pAttIDs)
            {
                _objectModel.IssueAttValues.InsertOnSubmit(new IssueAttValue
                {
                    IssueAttID = id,
                    IssueID = nextID,
                    Value = pAttValues[index++]
                }
                );
            }

            // Adding the issue in relevant project backlog.
            _objectModel.ProjectsBacklogs.InsertOnSubmit(new ProjectsBacklog
            {
                IssueID = nextID,
                ProjID = pProjectID
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

        public int AddIssueType(string pTypeName)
        {
            // If transaction fails then return -1.
            int result = -1;

            if (_objectModel.IssueTypes.Where(x => x.TypeName == pTypeName).Count() == 0)
            {
                _objectModel.IssueTypes.InsertOnSubmit(new IssueType
                {
                    TypeName = pTypeName
                }
                );

                try
                {
                    _objectModel.SubmitChanges();

                    // Returning the newly added type id to be used in list.
                    result = _objectModel.IssueTypes.Select(x => x.TypeID).Max();
                }

                catch (Exception ex)
                {
                }
            }

            else
            {
                // 0 means type already exists.
                result = 0;
            }

            return result;
        }

        // The last attribute "pOtherInfo" contains the issue state, type and priority ids.
        public bool EditIssue(string[] pAttValues, int[] pAttIDs, long pIssueID, int[] pOtherInfo)
        {
            int index = 0;
            bool result = false;

            var issue = from x in _objectModel.IssueAttValues
                        where x.IssueID == pIssueID
                        select x;

            // Editing in Issue table.
            issue.First().Issue.StateID = pOtherInfo[0];
            issue.First().Issue.TypeID = pOtherInfo[1];
            issue.First().Issue.PriorityID = pOtherInfo[2];

            // Editing in IssueAttValues table.
            foreach (int id in pAttIDs)
            {
                issue.Where(x => x.IssueAttID == id).First().Value = pAttValues[index++];
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

        // The "pFieldID" parameter may contains the field id.
        // This is required in "EditFieldInfo" when user is editing
        // the same field.
        public bool IsFieldExists(string pFieldName, int pFieldID)
        {
            IQueryable<IssueAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.IssueAttributes
                        where x.IssueAttName == pFieldName
                        select x;
            }

            else
            {
                field = from x in _objectModel.IssueAttributes
                        where x.IssueAttName == pFieldName && x.IssueAttID != pFieldID
                        select x;
            }

            return (field.Count() != 0);
        }

        public IssueAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.IssueAttributes.Where(x => x.IssueAttID == pFieldID).First();
        }

        public int CreateCustomField(string[] data)
        {
            int result = -1;

            var attribute = from x in _objectModel.IssueAttributes
                            where x.IssueAttName == data[0]
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
                    _objectModel.IssueAttributes.InsertOnSubmit(new IssueAttribute
                    {
                        IssueAttName = data[0],
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
                    _objectModel.IssueAttributes.InsertOnSubmit(new IssueAttribute
                    {
                        IssueAttName = data[0],
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

            var attribute = from x in _objectModel.IssueAttributes
                            where x.IssueAttName == pFieldName && x.IssueAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                var issueAtt = (from x in _objectModel.IssueAttributes
                                where x.IssueAttID == pFieldID
                                select x).First();

                issueAtt.IssueAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    issueAtt.DefaultValue = issueAtt.DefaultValue + ";" + pListOptions;
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

        //  Return only those issues which are in the given project backlog.
        public List<string> GetIssueIDs(long pSearchID, long pProjectID)
        {
            string pattern = "%" + pSearchID + "%";
            List<string> issueIDs;

            issueIDs = (from x in _objectModel.ProjectsBacklogs
                        where x.ProjID == pProjectID && SqlMethods.Like(x.IssueID.ToString(), pattern)
                        select x.IssueID.ToString()).ToList();

            return issueIDs;
        }

        public string GetIssueName(long pIssueID)
        {
            var issue = from x in _objectModel.IssueAttributes
                        where x.IssueAttName == "Issue Name"
                        from y in _objectModel.IssueAttValues
                        where (y.IssueAttID == x.IssueAttID && y.IssueID == pIssueID)
                        select y.Value;

            return issue.FirstOrDefault();
        }

        public IQueryable<IssueAttValue> GetIssueDependency(long pIssueID)
        {
            var issueIDs = from x in _objectModel.IssuesDependencies
                           where x.IssueID == pIssueID
                           select x.DependsOn;

            var dependency = from y in _objectModel.IssueAttributes
                             where y.IssueAttName == "Issue Name"
                                 from z in _objectModel.IssueAttValues
                                 where (z.IssueAttID == y.IssueAttID && issueIDs.Contains(z.IssueID))
                             select z;

            if (dependency.Count() > 0)
            {
                return dependency.OrderBy(x => x.Issue.PriorityID);
            }

            return null;
        }

        public int AddIssueDependency(long pIssueID, long pDependsOnID, long pProjectID)
        {
            int result = -1;

            // If both are same then do not process.
            if (pIssueID != pDependsOnID)
            {
                // Verifying is issue in the current project backlog.
                var isInBacklog = from x in _objectModel.ProjectsBacklogs
                                  where x.ProjID == pProjectID && x.IssueID == pDependsOnID
                                  select x;

                if (isInBacklog.Count() > 0)
                {
                    // Verifying cross dependency and already existence.
                    var temp = from x in _objectModel.IssuesDependencies
                               where (x.IssueID == pIssueID && x.DependsOn == pDependsOnID) ||
                                     (x.IssueID == pDependsOnID && x.DependsOn == pIssueID)
                               select x;

                    if (temp.Count() == 0)
                    {
                        _objectModel.IssuesDependencies.InsertOnSubmit(new IssuesDependency
                            {
                                IssueID = pIssueID,
                                DependsOn = pDependsOnID
                            }
                        );

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
                        result = 3;
                    }
                }

                else
                {
                    result = 2;
                }
            }

            else
            {
                result = 0;
            }

            return result;
        }

        public bool AddIssueDependency(long pIssueID, List<long> pDependsOnIDs)
        {
            bool result = false;

            // No need to verify the cross dependency, already exitence etc. Because,
            // the list provided to user for selection is already filtered. Please see
            // "GetIssues(List<int> pExceptIDs, int pProjectID)" definition.
            foreach (int id in pDependsOnIDs)
            {
                _objectModel.IssuesDependencies.InsertOnSubmit(new IssuesDependency
                {
                    IssueID = pIssueID,
                    DependsOn = id
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

        public int AddIssueInSprint(long pSprintID, long pIssueID, long pProjectID)
        {
            int result = -1;

            // If already exists then do not go ahead.
            var issue = from x in _objectModel.SprintsBacklogs
                        where x.SprintID == pSprintID && x.IssueID == pIssueID
                        select x;

            if (issue.Count() == 0)
            {
                // Verifying is issue in the current project backlog.
                var isInBacklog = from x in _objectModel.ProjectsBacklogs
                                  where x.ProjID == pProjectID && x.IssueID == pIssueID
                                  select x;

                if (isInBacklog.Count() > 0)
                {
                    // Verifying already existence in other sprint.
                    var temp = from x in _objectModel.SprintsBacklogs
                               where x.IssueID == pIssueID
                               select x;

                    if (temp.Count() == 0)
                    {
                        _objectModel.SprintsBacklogs.InsertOnSubmit(new SprintsBacklog
                        {
                            IssueID = pIssueID,
                            SprintID = pSprintID
                        }
                        );

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
                        result = 3;
                    }
                }

                else
                {
                    result = 2;
                }
            }

            else
            {
                result = 0;
            }

            return result;
        }

        public bool AddIssueInSprint(long pSprintID, List<long> pIssuesIDs)
        {
            bool result = false;

            // No need to verify the already exitence etc. Because,
            // the list provided to user for selection is already filtered.
            foreach (int id in pIssuesIDs)
            {
                _objectModel.SprintsBacklogs.InsertOnSubmit(new SprintsBacklog
                    {
                        IssueID = id,
                        SprintID = pSprintID
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

        public bool AssignIssue(long pIssueID, List<long> pEmpsIDs)
        {
            bool result = false;

            // No need to verify the already exitence etc. Because,
            // the list provided to user for selection is already filtered.
            foreach (int id in pEmpsIDs)
            {
                _objectModel.AssignedIssues.InsertOnSubmit(new AssignedIssue
                {
                    EmpID = id,
                    IssueID = pIssueID
                }
                );
            }

            // Getting the current state rank.
            var currentRank = _objectModel.Issues.Where(x => x.IssueID == pIssueID).Select(y => y.IssueWorkFlow.StateRank).First();

            // Getting the next active state id.
            var stateRank = _objectModel.IssueWorkFlows.Where(x => x.Active == true && x.StateRank > currentRank).Select(y => y.StateRank);

            // May be there is no next active state.
            if (stateRank.Count() > 0)
            {
                var chIssue = (from x in _objectModel.Issues
                               where x.IssueID == pIssueID
                               select x).First();

                chIssue.StateID = _objectModel.IssueWorkFlows.Where(x => x.StateRank == stateRank.Min()).Select(y => y.StateID).First();
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

        public bool RemoveIssueDependency(long pIssueID, long pDependnOnID)
        {
            bool result = true;

            var dependency = (from x in _objectModel.IssuesDependencies
                              where x.IssueID == pIssueID && x.DependsOn == pDependnOnID
                              select x).First();

            _objectModel.IssuesDependencies.DeleteOnSubmit(dependency);

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

        public bool RemoveIssueAssignee(long pIssueID, long pEmpID)
        {
            bool result = true;

            var assignee = (from x in _objectModel.AssignedIssues
                            where x.IssueID == pIssueID && x.EmpID == pEmpID
                            select x).First();

            _objectModel.AssignedIssues.DeleteOnSubmit(assignee);

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

        public bool UpdateWorkFlow(int[] pStatesIDs, int[] pStatesRanks, bool[] pActive)
        {
            bool result = false;
            int index = 0;

             var states = from x in _objectModel.IssueWorkFlows
                          select x;

            foreach (int id in pStatesIDs)
            {
                var item = states.Where(x => x.StateID == id).First();
                item.Active = pActive[index];
                item.StateRank = pStatesRanks[index++];
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

        public int AddIssueState(string pStateName)
        {
            int result = -1;

            var state = from x in _objectModel.IssueWorkFlows
                        where x.StateName == pStateName
                        select x;

            // If state already exists then do not create again.
            if (state.Count() == 0)
            {
                _objectModel.IssueWorkFlows.InsertOnSubmit(new IssueWorkFlow
                    {
                         StateName = pStateName,
                         Active  = true,
                         StateRank = _objectModel.IssueWorkFlows.Select(x => x.StateRank).Max() + 1
                    }
                );

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

        public int EditState(int pStateID, string pStateName)
        {
            int result = -1;

            var states = from x in _objectModel.IssueWorkFlows
                         where x.StateName == pStateName && x.StateID != pStateID
                         select x;

            // If state already exists then do not create again.
            if (states.Count() == 0)
            {
                var state = (from x in _objectModel.IssueWorkFlows
                                where x.StateID == pStateID
                                select x).First();

                state.StateName = pStateName;

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

        public bool ExecuteWorkFlow(long pIssueID, long pEmpID)
        {
            bool result = false;

            // An issue can be assigned to more than one user.
            // If the current issue is assigned to more than one user
            // then just remove it from the current user assigned list 
            // and do not change the issue state.
            var users = from x in _objectModel.AssignedIssues
                        where x.IssueID == pIssueID
                        select x.EmpID;

            if (users.Count() == 1)
            {
                // Getting the current state rank.
                var currentRank = _objectModel.Issues.Where(x => x.IssueID == pIssueID).Select(y => y.IssueWorkFlow.StateRank).First();

                // Getting the next active state id.
                var stateRank = _objectModel.IssueWorkFlows.Where(x => x.Active == true && x.StateRank > currentRank).Select(y => y.StateRank);

                // May be there is no next active state.
                if (stateRank.Count() > 0)
                {
                    var chIssue = (from x in _objectModel.Issues
                                   where x.IssueID == pIssueID
                                   select x).First();

                    chIssue.StateID = _objectModel.IssueWorkFlows.Where(x => x.StateRank == stateRank.Min()).Select(y => y.StateID).First();
                }
            }

            // Removing the issue from the current user assigned issues' list.
            var issue = (from x in _objectModel.AssignedIssues
                         where x.IssueID == pIssueID && x.EmpID == pEmpID
                         select x).First();

            _objectModel.AssignedIssues.DeleteOnSubmit(issue);
            
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

        public int WorkDone(long pIssueID, double pHours)
        {
            int result = -1;

            var issue = (from x in _objectModel.IssueAttributes
                        where x.IssueAttName == "Man Hours"
                        from y in _objectModel.IssueAttValues
                        where y.IssueID == pIssueID && y.IssueAttID == x.IssueAttID
                        select y).First();

            double currentHours = double.Parse(issue.Value);

            if (currentHours > 0 && pHours <= currentHours)
            {
                currentHours -= pHours;
                issue.Value = currentHours + "";

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

        public double GetIssueManHours(long pIssueID)
        {
            var issue = (from x in _objectModel.IssueAttributes
                         where x.IssueAttName == "Man Hours"
                         from y in _objectModel.IssueAttValues
                         where y.IssueID == pIssueID && y.IssueAttID == x.IssueAttID
                         select y).First();

            return double.Parse(issue.Value);
        }

        public List<long> SearchProjBacklogIssue(int pFieldID, string pFieldValue, long pProjectID)
        {
            int fieldType = 0;
            IQueryable<long> issuesIDs;
            IQueryable<long> issuesFromBacklog = null;

            // The value 0 is for search by issue ID.
            // For this, we have to query with ProjectsBacklogs' table
            // rather than with IssueAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.IssueAttributes
                             where f.IssueAttID == pFieldID
                             select f.FieldType).First();

                issuesFromBacklog = from x in _objectModel.ProjectsBacklogs
                                    where x.ProjID == pProjectID
                                    select x.IssueID;
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                issuesIDs = from x in _objectModel.IssueAttValues
                            where issuesFromBacklog.Contains(x.IssueID) && x.IssueAttID == pFieldID && x.Value == null
                            select x.IssueID;
            }

            else
            {
                // If pFieldID = 0 then this means the search is on Issue ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    issuesIDs = from x in _objectModel.ProjectsBacklogs
                                where x.ProjID == pProjectID && x.IssueID == id
                                select x.IssueID;
                }

                else
                {
                    issuesIDs = from x in _objectModel.IssueAttValues
                                where issuesFromBacklog.Contains(x.IssueID) && x.IssueAttID == pFieldID && x.Value == pFieldValue
                                select x.IssueID;
                }
            }

            return issuesIDs.ToList();
        }
        
        public List<long> SearchSprintBacklogIssue(int pFieldID, string pFieldValue, long pSprintID)
        {
            int fieldType = 0;
            IQueryable<long> issuesIDs;
            IQueryable<long> issuesFromBacklog = null;

            // The value 0 is for search by issue ID.
            // For this, we have to query with SprintsBacklogs' table
            // rather than with IssueAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.IssueAttributes
                             where f.IssueAttID == pFieldID
                             select f.FieldType).First();

                issuesFromBacklog = from x in _objectModel.SprintsBacklogs
                                    where x.SprintID == pSprintID
                                    select x.IssueID;
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                issuesIDs = from x in _objectModel.IssueAttValues
                            where issuesFromBacklog.Contains(x.IssueID) && x.IssueAttID == pFieldID && x.Value == null
                            select x.IssueID;
            }

            else
            {
                // If pFieldID = 0 then this means the search is on Issue ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    issuesIDs = from x in _objectModel.SprintsBacklogs
                                where x.SprintID == pSprintID && x.IssueID == id
                                select x.IssueID;
                }

                else
                {
                    issuesIDs = from x in _objectModel.IssueAttValues
                                where issuesFromBacklog.Contains(x.IssueID) && x.IssueAttID == pFieldID && x.Value == pFieldValue
                                select x.IssueID;
                }
            }

            return issuesIDs.ToList();
        }

        public bool RemoveSprintIssue(long pIssueID, long pSprintID)
        {
            bool result = false;

            // Removing the issue from the sprint backlog.
            var issue = (from x in _objectModel.SprintsBacklogs
                         where x.SprintID == pSprintID && x.IssueID == pIssueID
                         select x).First();

            _objectModel.SprintsBacklogs.DeleteOnSubmit(issue);
          
            // Removing from the assigned list as an issue cannot be assigned 
            // without sprint.
            var asignees = from x in _objectModel.AssignedIssues
                           where x.IssueID == pIssueID
                           select x;

            if(asignees.Count() > 0)
                _objectModel.AssignedIssues.DeleteAllOnSubmit(asignees);

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

        // Will return all the issues' ids of the given project
        // which are added in any sprint.
        public List<long> GetAllSprintsIssues(long pProjectID)
        {
            var isssuesIDs = from x in _objectModel.SprintsBacklogs
                             where x.Sprint.ProjID == pProjectID
                             select x.IssueID;

            return isssuesIDs.ToList();
        }

        // Returns those issues on which the current issue depends and are unresolved.
        public IQueryable<IssueAttValue> CheckIssueDependency(long pIssueID)
        {
            var issueIDs = (from x in _objectModel.IssuesDependencies
                           where x.IssueID == pIssueID
                           select x.DependsOn).ToList();

            if (issueIDs.Count() > 0)
            {
                int lastStateRank = (from x in _objectModel.IssueWorkFlows
                                     where x.Active == true
                                     select x.StateRank).Max();

                var unResolvedIssuesIDs = (from x in _objectModel.Issues
                                          where issueIDs.Contains(x.IssueID) && x.IssueWorkFlow.StateRank != lastStateRank
                                          select x.IssueID).ToList();

                var issuesNames = from y in _objectModel.IssueAttributes
                                  where y.IssueAttName == "Issue Name"
                                  from z in _objectModel.IssueAttValues
                                  where unResolvedIssuesIDs.Contains(z.IssueID) && z.IssueAttID == y.IssueAttID
                                  select z;

                return issuesNames;
            }

            return null;
        }

        // The last active state means the final state.
        // So, if issue is in that state then means completed.
        public bool IsIssueCompleted(long pIssueID)
        {
            int lastState = (from x in _objectModel.IssueWorkFlows
                             where x.Active == true
                             select x.StateRank).Max();

            int issueState = (from x in _objectModel.Issues
                             where x.IssueID == pIssueID
                             select x.IssueWorkFlow.StateRank).First();

            return (lastState == issueState);
        }
    }
}
