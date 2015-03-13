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
 * in contex with Project relevant data manipulation. Like:  *
 * Creating and Updating the project, Creating Sprint,       *
 * Creating Product Backlog and likewise some other operati- *
 * ons.                                                      *
 *                                                           *
 *-----------------------------------------------------------*/

namespace Domain.Repository.Concrete
{
    public class SqlProjectRepository : IProjectRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlProjectRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        public IQueryable<ProjAttValue> GetProjects(bool pOnlyActive)
        {
            if (pOnlyActive)
            {
                var activeProjectIDs = from x in _objectModel.ProjAttributes
                                       where x.ProjAttName == "Active"
                                       from y in _objectModel.ProjAttValues
                                       where y.ProjAttID == x.ProjAttID && y.Value == "Yes"
                                       select y.ProjID;

                if (activeProjectIDs.Count() > 0)
                {
                    var activeProjectNames = from x in _objectModel.ProjAttributes
                                             where x.ProjAttName == "Project Name"
                                             from y in _objectModel.ProjAttValues
                                             where y.ProjAttID == x.ProjAttID && activeProjectIDs.Contains(y.ProjID)
                                             select y;

                    return activeProjectNames;
                }

                return null;
            }

            else
            {
                var projects = from x in _objectModel.ProjAttributes
                               where x.ProjAttName == "Project Name"
                               from y in _objectModel.ProjAttValues
                               where y.ProjAttID == x.ProjAttID
                               select y;

                return projects;
            }
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
        public IQueryable<ProjAttribute> GetProjectAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.ProjAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.ProjAttID)
                                  select x).OrderBy(x => x.FieldType);

                return attributes;
            }

            else
            {
                return _objectModel.ProjAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType);
            }
        }

        public IQueryable<ProjAttribute> GetProjectAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.ProjAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                return _objectModel.ProjAttributes;
            }
        }

        public long GetProjectID(long pIssueID)
        {
            var ID = from x in _objectModel.ProjectsBacklogs
                     where x.IssueID == pIssueID
                     select x.ProjID;

            return ID.First();
        }

        public bool CreateProject(string[] pAttValues, int[] pAttIDs, List<ProjectRisk> pRiskAssessment)
        {
            int index = 0;
            long nextID = 1;
            bool result = false;

            if (_objectModel.Projects.FirstOrDefault() != null)
            {
                var maxID = (from x in _objectModel.Projects
                             select x).Max(y => y.ProjID);

                nextID = maxID + 1;
            }

            _objectModel.Projects.InsertOnSubmit(new Project
            {
                ProjID = nextID
            }
                                                );

            foreach (int id in pAttIDs)
            {
                _objectModel.ProjAttValues.InsertOnSubmit(new ProjAttValue
                {
                    ProjAttID = id,
                    ProjID = nextID,
                    Value = pAttValues[index++]
                }
                                                         );
            }

            foreach (ProjectRisk risk in pRiskAssessment)
            {
                _objectModel.ProjectRisks.InsertOnSubmit(new ProjectRisk
                {
                    ProjID = nextID,
                    RiskID = risk.RiskID,
                    Probability = risk.Probability,
                    Impact = risk.Impact,
                    Mitigation = risk.Mitigation
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

        public IQueryable<ProjAttValue> GetProjectByID(long pProjectID)
        {
            var project = (from x in _objectModel.ProjAttValues
                           where x.ProjID == pProjectID
                           select x).OrderBy(x => x.ProjAttribute.FieldType);

            return project;
        }

        public string GetProjectName(long pProjectID)
        {
            var projectName = (from x in _objectModel.ProjAttributes
                              where x.ProjAttName == "Project Name"
                                  from y in _objectModel.ProjAttValues
                                  where y.ProjAttID == x.ProjAttID && y.ProjID == pProjectID
                              select y.Value).First();

            return projectName;
        }

        public bool EditProject(string[] pAttValues, int[] pAttIDs, long pProjectID)
        {
            int index = 0;
            bool result = false;

            var project = from x in _objectModel.ProjAttValues
                          where x.ProjID == pProjectID
                          select x;

            foreach (int id in pAttIDs)
            {
                project.Where(x => x.ProjAttID == id).First().Value = pAttValues[index++];
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

        public int CreateCustomField(string[] data)
        {
            int result = -1;

            var attribute = from x in _objectModel.ProjAttributes
                            where x.ProjAttName == data[0]
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
                    _objectModel.ProjAttributes.InsertOnSubmit(new ProjAttribute
                    {
                        ProjAttName = data[0],
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
                    _objectModel.ProjAttributes.InsertOnSubmit(new ProjAttribute
                    {
                        ProjAttName = data[0],
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

        public ProjAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.ProjAttributes.Where(x => x.ProjAttID == pFieldID).First();
        }

        // User can only change the Field Name property. And, if the field is of
        // type list then user can specify new option(s) for list which will be added
        // in existing list. So, third parameter is for that purpose.
        public int EditField(int pFieldID, string pFieldName, string pListOptions)
        {
            int result = -1;

            var attribute = from x in _objectModel.ProjAttributes
                            where x.ProjAttName == pFieldName && x.ProjAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                var projectAtt = (from x in _objectModel.ProjAttributes
                                  where x.ProjAttID == pFieldID
                                  select x).First();

                projectAtt.ProjAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    projectAtt.DefaultValue = projectAtt.DefaultValue + ";" + pListOptions;
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

        // The "pFieldID" parameter may contains the field id.
        // This is required in "EditFieldInfo" when user is editing
        // the same field.
        public bool IsFieldExists(string pFieldName, int pFieldID)
        {
            IQueryable<ProjAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.ProjAttributes
                            where x.ProjAttName == pFieldName
                            select x;
            }

            else
            {
                field = from x in _objectModel.ProjAttributes
                            where x.ProjAttName == pFieldName && x.ProjAttID != pFieldID
                            select x;
            }

            return (field.Count() != 0);
        }

        public IQueryable<ProjAttValue> SearchProject(int pFieldID, string pFieldValue) 
        {
            int fieldType = 0;

            // The value 0 is for search by Project ID.
            // For this, we have to query with Projects' table
            // rather than with ProjAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.ProjAttributes
                                 where f.ProjAttID == pFieldID
                                 select f.FieldType).First();
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                var projectIDs = from x in _objectModel.ProjAttValues
                                 where x.ProjAttID == pFieldID && x.Value == null
                                 select x.ProjID;

                if (projectIDs.Count() > 0)
                {
                    var projectsNames = from x in _objectModel.ProjAttributes
                                             where x.ProjAttName == "Project Name"
                                             from y in _objectModel.ProjAttValues
                                             where y.ProjAttID == x.ProjAttID && projectIDs.Contains(y.ProjID)
                                             select y;
                    return projectsNames;
                }
            }

            else
            {
                IQueryable<long> projectIDs;

                // If pFieldID = 0 then this means the search is on Project ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    projectIDs = from x in _objectModel.Projects
                                 where x.ProjID == id
                                 select x.ProjID;
                }

                else
                {
                     projectIDs = from x in _objectModel.ProjAttValues
                                     where x.ProjAttID == pFieldID && x.Value == pFieldValue
                                     select x.ProjID;
                }

                if (projectIDs.Count() > 0)
                {
                    var projectsNames = from x in _objectModel.ProjAttributes
                                             where x.ProjAttName == "Project Name"
                                             from y in _objectModel.ProjAttValues
                                             where y.ProjAttID == x.ProjAttID && projectIDs.Contains(y.ProjID)
                                             select y;

                    return projectsNames;
                }
            }

            return null;
        }

        public List<long> ProjectBacklog(long pProjectID)
        {
            var issueIDs = from x in _objectModel.ProjectsBacklogs
                           where x.ProjID == pProjectID
                           select x.IssueID;

            return issueIDs.ToList();
        }

        public IQueryable<ProjectRisk> GetProjectRisks(long pProjectID, short pRiskExposure)
        {
            return _objectModel.ProjectRisks.Where(x => x.ProjID == pProjectID && (x.Probability * x.Impact) >= pRiskExposure).OrderByDescending(x => (x.Probability * x.Impact));
        }

        public ProjectRisk GetProjectRiskByID(long pProjectID, int pRiskID)
        {
            return _objectModel.ProjectRisks.Where(x => x.ProjID == pProjectID && x.RiskID == pRiskID).First();
        }

        public bool EditProjectRisk(ProjectRisk pRisk)
        {
            bool result = false;

            var risk = (from x in _objectModel.ProjectRisks
                        where x.ProjID == pRisk.ProjID && x.RiskID == pRisk.RiskID
                           select x).First();

            risk.Probability = pRisk.Probability;
            risk.Impact = pRisk.Impact;
            risk.Mitigation = pRisk.Mitigation;

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

        public bool AddRisks(long pProjectID, List<ProjectRisk> pRiskAssessment)
        {
            bool result = false;

            foreach (ProjectRisk risk in pRiskAssessment)
            {
                _objectModel.ProjectRisks.InsertOnSubmit(new ProjectRisk
                {
                    ProjID = pProjectID,
                    RiskID = risk.RiskID,
                    Probability = risk.Probability,
                    Impact = risk.Impact,
                    Mitigation = risk.Mitigation
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

        public bool AddRisk(long pProjectID, ProjectRisk pRisk)
        {
            bool result = false;

            _objectModel.ProjectRisks.InsertOnSubmit(new ProjectRisk
                {
                    ProjID = pProjectID,
                    RiskID = pRisk.RiskID,
                    Probability = pRisk.Probability,
                    Impact = pRisk.Impact,
                    Mitigation = pRisk.Mitigation
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

        public IQueryable<ProjAttValue> GetFavoriteProjects(long pUserID)
        {
            var projectIDs = from x in _objectModel.FavoriteProjects
                             where x.EmpID == pUserID
                             select x.ProjID;


            if (projectIDs.Count() > 0)
            {
                var projectNames = from x in _objectModel.ProjAttributes
                                   where x.ProjAttName == "Project Name"
                                         from y in _objectModel.ProjAttValues
                                         where y.ProjAttID == x.ProjAttID && projectIDs.Contains(y.ProjID)
                                   select y;

                return projectNames;
            }

            return null;
        }

        public bool IsFavoriteProject(long pProjectID, long pUserID)
        {
            return (_objectModel.FavoriteProjects.Where(x => x.EmpID == pUserID && x.ProjID == pProjectID).Count() > 0);
        }

        public bool AddToFavorite(long pProjectID, long pUserID)
        {
            bool result = false;

            var temp = from x in _objectModel.FavoriteProjects
                       where x.ProjID == pProjectID && x.EmpID == pUserID
                       select x;

            // If already exists then do not insert again.
            if (temp.Count() == 0)
            {
                _objectModel.FavoriteProjects.InsertOnSubmit(new FavoriteProject
                    {
                        ProjID = pProjectID,
                        EmpID = pUserID
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
            }

            return result;
        }

        public bool RemoveFromFavorite(long pProjectID, long pUserID)
        {
            bool result = false;

            var favoriteProject = (from x in _objectModel.FavoriteProjects
                                  where x.ProjID == pProjectID && x.EmpID == pUserID
                                  select x).First();

            _objectModel.FavoriteProjects.DeleteOnSubmit(favoriteProject);
            
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
    }
}
