using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

/*------------------ About this Interface --------------------*
 *                                                            *
 * This interface is for the interaction with Database in     *
 * contex with Project's Sprints relevant data manipulation.  *
 * Please see "SqlSprintRepository" class for further details.* 
 *                                                            *
 *------------------------------------------------------------*/

namespace Domain.Repository.Abstract
{
    public interface ISprintRepository
    {
        IQueryable<SprintAttValue> GetProjectSprints(long pProjectID);
        IQueryable<SprintAttValue> GetSprintByID(long pSprintID, bool pIsForEdit);
        IQueryable<FieldType> GetFieldTypes();
        IQueryable<RegularExpression> GetRegularExpressions();
        IQueryable<SprintAttribute> GetSprintAttributes(List<int> pCustomAttributes);
        IQueryable<SprintAttribute> GetSprintAttributes(bool pOnlyCustomLevel);
        string GetSprintName(long pSprintID);
        bool CreateSprint(string[] pAttValues, int[] pAttIDs, long pProjectID);
        bool EditSprint(string[] pAttValues, int[] pAttIDs, long pSprintID);
        bool IsFieldExists(string pFieldName, int pFieldID);
        SprintAttribute GetFieldByID(int pFieldID);
        int CreateCustomField(string[] data);
        int EditField(int pFieldID, string pFieldName, string pListOptions);
        IQueryable<SprintAttValue> SearchSprint(int pFieldID, string pFieldValue, long pProjectID);
        List<long> SprintBacklog(long pSprintID);
        bool IsSprintValid(long pSprintID, long pIssueID = 0);
        bool IsSprintStarted(long pSprintID, long pIssueID = 0);
        int GetSprintDuration(long pSprintID);
        int GetSprintRemainingDays(long pSprintID);
        double GetSprintWorkLoad(long pSprintID);
        int GetSprintHours(long pSprintID, int pUsersCount = 0);
        bool SaveMeeting(long pSprintID, long pEmpID, string[] pData);
        MeetingDetail MeetingDetails(long pSprintID, long pEmpID, long pMeetingID);
        bool EditMeeting(long pMeetingID, long pEmpID, string[] pData);
        IQueryable<ScrumMeeting> ScrumMeetings(long pSprintID);
        long GetSprintID(long pIssueID);
        bool SaveDailyEffort(long pSprintID, double pHours);
        int GetSprintDay(long pSprintID);
        float[] GetDailyEffort(long pSprintID);
        int GetTotalSprintHours(long pSprintID, int pUsersCount = 0);
    }
}
