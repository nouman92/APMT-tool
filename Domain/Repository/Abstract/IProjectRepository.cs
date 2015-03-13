using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Project relevant data manipulation. Please see *
 * "SqlProjectRepository" class for further details.          * 
 *                                                            *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface IProjectRepository
    {
        IQueryable<ProjAttribute> GetProjectAttributes(List<int> pCustomAttributes);
        IQueryable<ProjAttribute> GetProjectAttributes(bool pOnlyCustomLevel);
        string GetProjectName(long pProjectID);
        bool CreateProject(string[] pAttValues, int[] pAttIDs, List<ProjectRisk> pRiskAssessment);
        IQueryable<ProjAttValue> GetProjects(bool pOnlyActive);
        IQueryable<ProjAttValue> GetProjectByID(long pProjectID);
        bool EditProject(string[] pAttValues, int[] pAttIDs, long pProjectID);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        long GetProjectID(long pIssueID);
        int CreateCustomField(string[] data);
        bool IsFieldExists(string pFieldName, int pFieldID);
        ProjAttribute GetFieldByID(int pFieldID);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        IQueryable<ProjAttValue> SearchProject(int pFieldID, string pFieldValue);
        List<long> ProjectBacklog(long pProjectID);
        IQueryable<ProjectRisk> GetProjectRisks(long pProjectID, short pRiskExposure);
        ProjectRisk GetProjectRiskByID(long pProjectID, int pRiskID);
        bool EditProjectRisk(ProjectRisk pRisk);
        bool AddRisks(long pProjectID, List<ProjectRisk> pRiskAssessment);
        IQueryable<ProjAttValue> GetFavoriteProjects(long pUserID);
        bool IsFavoriteProject(long pProjectID, long pUserID);
        bool RemoveFromFavorite(long pProjectID, long pUserID);
        bool AddToFavorite(long pProjectID, long pUserID);
        bool AddRisk(long pProjectID, ProjectRisk pRisk);
    }
}
