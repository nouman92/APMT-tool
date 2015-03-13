using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Repository.Abstract;
using Domain.Entities;
using System.Configuration;

/*-------------------- About this Class ---------------------*
 *                                                           *
 * This class is interface for the interaction with Database *
 * in contex with Sprints' relevant data manipulation. Like: *
 * Creating Sprint for Project, Editing Sprint, Creating     *
 * Sprint backlog etc.                                       *
 *                                                           *
 *---------------------------------------------------------- */

namespace Domain.Repository.Concrete
{
    public class SqlSprintRepository : ISprintRepository
    {
        private APMTObjectModelDataContext _objectModel;

        public SqlSprintRepository()
        {
            // Getting connection string from app.config file.
            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["CString"];
            _objectModel = new APMTObjectModelDataContext(cString.ConnectionString);
        }

        // Get the project's sprints.
        public IQueryable<SprintAttValue> GetProjectSprints(long pProjectID)
        {
            var sprintsIDs = (from x in _objectModel.Sprints
                              where x.ProjID == pProjectID
                              select x.SprintID).ToList();

            if (sprintsIDs.Count() > 0)
            {
                var sprintsNames = from x in _objectModel.SprintAttributes
                                   where x.SprintAttName == "Sprint Name"
                                   from y in _objectModel.SprintAttValues
                                   where sprintsIDs.Contains(y.SprintID) && y.SprintAttID == x.SprintAttID
                                   select y;

                return sprintsNames;
            }

            return null;
        }

        public IQueryable<SprintAttValue> GetSprintByID(long pSprintID, bool pIsForEdit)
        {
            IQueryable<SprintAttValue> sprint;

            // Sprint start and end date cannot be changed. So,
            // filtering these attributes.
            if (pIsForEdit)
            {
                sprint = (from x in _objectModel.SprintAttValues
                          where x.SprintID == pSprintID && (x.SprintAttID != 2 && x.SprintAttID != 3)
                          select x).OrderBy(x => x.SprintAttribute.FieldType);
            }

            else
            {
                sprint = (from x in _objectModel.SprintAttValues
                          where x.SprintID == pSprintID
                          select x).OrderBy(x => x.SprintAttribute.FieldType);
            }

            return sprint;
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
        public IQueryable<SprintAttribute> GetSprintAttributes(List<int> pCustomAttributes)
        {
            if (pCustomAttributes != null && pCustomAttributes.Count > 0)
            {
                var attributes = (from x in _objectModel.SprintAttributes
                                  where (x.IsSystemLevel == true) || pCustomAttributes.Contains(x.SprintAttID)
                                  select x).OrderBy(x => x.FieldType);

                return attributes;
            }

            else
            {
                return _objectModel.SprintAttributes.Where(x => x.IsSystemLevel == true).OrderBy(x => x.FieldType);
            }
        }

        public IQueryable<SprintAttribute> GetSprintAttributes(bool pOnlyCustomLevel)
        {
            if (pOnlyCustomLevel)
            {
                return _objectModel.SprintAttributes.Where(x => x.IsSystemLevel == false);
            }

            else
            {
                return _objectModel.SprintAttributes;
            }
        }

        public string GetSprintName(long pSprintID)
        {
            var sprintName = (from x in _objectModel.SprintAttributes
                              where x.SprintAttName == "Sprint Name"
                              from y in _objectModel.SprintAttValues
                              where y.SprintAttID == x.SprintAttID && y.SprintID == pSprintID
                              select y.Value).First();

            return sprintName;
        }

        public bool CreateSprint(string[] pAttValues, int[] pAttIDs, long pProjectID)
        {
            int index = 0;
            bool result = false;

            _objectModel.Sprints.InsertOnSubmit(new Sprint
            {
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

            if (result)
            {
                result = false;
                long sprintID = _objectModel.Sprints.Select(x => x.SprintID).Max();

                foreach (int id in pAttIDs)
                {
                    _objectModel.SprintAttValues.InsertOnSubmit(new SprintAttValue
                    {
                        SprintAttID = id,
                        SprintID = sprintID,
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
            }

            return result;
        }

        public bool EditSprint(string[] pAttValues, int[] pAttIDs, long pSprintID)
        {
            int index = 0;
            bool result = false;

            var sprint = from x in _objectModel.SprintAttValues
                         where x.SprintID == pSprintID
                         select x;

            // Editing in SprintAttValues table.
            foreach (int id in pAttIDs)
            {
                sprint.Where(x => x.SprintAttID == id).First().Value = pAttValues[index++];
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
            IQueryable<SprintAttribute> field;

            if (pFieldID == 0)
            {
                field = from x in _objectModel.SprintAttributes
                        where x.SprintAttName == pFieldName
                        select x;
            }

            else
            {
                field = from x in _objectModel.SprintAttributes
                        where x.SprintAttName == pFieldName && x.SprintAttID != pFieldID
                        select x;
            }

            return (field.Count() != 0);
        }

        public SprintAttribute GetFieldByID(int pFieldID)
        {
            return _objectModel.SprintAttributes.Where(x => x.SprintAttID == pFieldID).First();
        }

        public int CreateCustomField(string[] data)
        {
            int result = -1;

            var attribute = from x in _objectModel.SprintAttributes
                            where x.SprintAttName == data[0]
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
                    _objectModel.SprintAttributes.InsertOnSubmit(new SprintAttribute
                    {
                        SprintAttName = data[0],
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
                    _objectModel.SprintAttributes.InsertOnSubmit(new SprintAttribute
                    {
                        SprintAttName = data[0],
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

            var attribute = from x in _objectModel.SprintAttributes
                            where x.SprintAttName == pFieldName && x.SprintAttID != pFieldID
                            select x;

            // If attribute already exists then do not create again.
            if (attribute.Count() == 0)
            {
                var sprintAtt = (from x in _objectModel.SprintAttributes
                                 where x.SprintAttID == pFieldID
                                 select x).First();

                sprintAtt.SprintAttName = pFieldName;

                if (pListOptions != null && pListOptions != "")
                {
                    sprintAtt.DefaultValue = sprintAtt.DefaultValue + ";" + pListOptions;
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

        public IQueryable<SprintAttValue> SearchSprint(int pFieldID, string pFieldValue, long pPorjectID)
        {
            int fieldType = 0;
            IQueryable<long> sprintsIDs;

            // The value 0 is for search by Component ID.
            // For this, we have to query with Sprints' table
            // rather than with SprintAttributes' table.
            if (pFieldID != 0)
            {
                fieldType = (from f in _objectModel.SprintAttributes
                             where f.SprintAttID == pFieldID
                             select f.FieldType).First();
            }

            // For check box with unchecked (need to search against "null").
            if (fieldType == 5 && pFieldValue != "Yes")
            {
                sprintsIDs = from x in _objectModel.SprintAttValues
                             where x.Sprint.ProjID == pPorjectID && x.SprintAttID == pFieldID && x.Value == null
                             select x.SprintID;
            }

            else
            {
                // If pFieldID = 0 then this means the search is on Component ID.
                if (pFieldID == 0)
                {
                    int id = int.Parse(pFieldValue);

                    sprintsIDs = from x in _objectModel.Sprints
                                 where x.ProjID == pPorjectID && x.SprintID == id
                                 select x.SprintID;
                }

                else
                {
                    sprintsIDs = from x in _objectModel.SprintAttValues
                                 where x.Sprint.ProjID == pPorjectID && x.SprintAttID == pFieldID && x.Value == pFieldValue
                                 select x.SprintID;
                }
            }

            if (sprintsIDs.Count() > 0)
            {
                var sprintssNames = from x in _objectModel.SprintAttributes
                                    where x.SprintAttName == "Sprint Name"
                                    from y in _objectModel.SprintAttValues
                                    where y.SprintAttID == x.SprintAttID && sprintsIDs.Contains(y.SprintID)
                                    select y;

                return sprintssNames;
            }

            return null;
        }

        public List<long> SprintBacklog(long pSprintID)
        {
            var issueIDs = from x in _objectModel.SprintsBacklogs
                           where x.SprintID == pSprintID
                           select x.IssueID;

            return issueIDs.ToList();
        }

        // If the "pIssueID" in greater than 0 then its mean
        // first find the sprint id then verify it.
        public bool IsSprintValid(long pSprintID, long pIssueID = 0)
        {
            if (pIssueID != 0)
            {
                pSprintID = (from x in _objectModel.SprintsBacklogs
                             where x.IssueID == pIssueID
                             select x.SprintID).First();
            }

            string endDate = (from x in _objectModel.SprintAttributes
                             where x.SprintAttName == "End Date"
                             from y in _objectModel.SprintAttValues
                             where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                             select y.Value).First();

            DateTime cDate = DateTime.Now;
            return (DateTime.Compare(DateTime.Parse(endDate), new DateTime(cDate.Year, cDate.Month, cDate.Day)) > -1);
        }

        // Returns the total working days (excluding holidays).
        public int GetSprintDuration(long pSprintID)
        {
            string endDate = (from x in _objectModel.SprintAttributes
                              where x.SprintAttName == "End Date"
                              from y in _objectModel.SprintAttValues
                              where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                              select y.Value).First();

            string staDate = (from x in _objectModel.SprintAttributes
                              where x.SprintAttName == "Start Date"
                              from y in _objectModel.SprintAttValues
                              where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                              select y.Value).First();

            TimeSpan timeSpanObj = DateTime.Parse(endDate) - DateTime.Parse(staDate);
            int workingDays = new SqlOrganizationRepository().GetWorkingDays(), duration = 0, days;
            // The difference between two dates e.g. 8 May 2011 - 10 May 2011 is 2. But this does not
            // include the 8th May. So, adding 1 for this purpose.
            days = timeSpanObj.Days + 1;

            if((days/7) < 1)
            {
                if (days <= workingDays)
                    duration = days;
                else
                    duration = workingDays;
            }

            else
            {
                int weeks = days / 7, holidays, remainder = days % 7;
                holidays = weeks * (7 - workingDays);

                if (remainder != 0 && remainder > workingDays)
                {
                    holidays += remainder - workingDays;
                }

                duration = days - holidays;
            }

            return duration;
        }

        public int GetSprintRemainingDays(long pSprintID)
        {
            DateTime cDate = DateTime.Now;
            string staDate = (from x in _objectModel.SprintAttributes
                              where x.SprintAttName == "Start Date"
                              from y in _objectModel.SprintAttValues
                              where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                              select y.Value).First();

            // If sprint not started yet, then the remaining days are equal to sprint duration.
            if (DateTime.Compare(DateTime.Parse(staDate), new DateTime(cDate.Year, cDate.Month, cDate.Day)) < 1)
            {
                string endDate = (from x in _objectModel.SprintAttributes
                                  where x.SprintAttName == "End Date"
                                  from y in _objectModel.SprintAttValues
                                  where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                                  select y.Value).First();

                TimeSpan timeSpanObj = DateTime.Parse(endDate) - new DateTime(cDate.Year, cDate.Month, cDate.Day);
                int workingDays = new SqlOrganizationRepository().GetWorkingDays();
                int duration = 0;

                if (timeSpanObj.Days < 0)
                {
                    duration = 0;
                }

                else if ((timeSpanObj.Days / 7) < 1)
                {
                    if (timeSpanObj.Days <= workingDays)
                        duration = timeSpanObj.Days;
                    else
                        duration = workingDays;
                }

                else
                {
                    int weeks = timeSpanObj.Days / 7, holidays, remainder = timeSpanObj.Days % 7;
                    holidays = weeks * (7 - workingDays);

                    if (remainder != 0 && remainder > workingDays)
                    {
                        holidays += remainder - workingDays;
                    }

                    duration = timeSpanObj.Days - holidays;
                }

                return duration;
            }

            return GetSprintDuration(pSprintID);
        }

        // Returns the day number of sprint (e.g. 1st day, 2nd day etc).
        public int GetSprintDay(long pSprintID)
        {
            return GetSprintDuration(pSprintID) - GetSprintRemainingDays(pSprintID); 
        }

        // Returns the total available sprint hours. The sprint hours will be equal 
        // to the sum of man hours of all human resources(which will have issues) available in that sprint.
        // And each user will have the man hours according to the sprint duration.
        // Example:
        // ----------------------------------------------------
        // Sprint duration = SD = 1 week (Only working days)
        // Man hours in a week = MHW = working days per week * man hours per day (according to the organization criteria)
        // Each user man hours in sprint = SD * MHW
        // Sprint hours = MHE * Number of users in the sprint
        // ----------------------------------------------------
        // This function also used to get the user man hours when "pUsersCount" is not 0.
        public int GetSprintHours(long pSprintID, int pUsersCount = 0)
        {
            if (pUsersCount == 0)
            {
                // Getting the users available in the sprint.
                pUsersCount = (from x in _objectModel.SprintsBacklogs
                               where x.SprintID == pSprintID
                               from y in _objectModel.AssignedIssues
                               where y.IssueID == x.IssueID
                               select y.EmpID).Distinct().Count();
            }

            int remainingDays = GetSprintRemainingDays(pSprintID);

            // If no remaining day then check if today is the last day then return the remaining hours.
            if (remainingDays == 0)
            {
                string endDate = (from x in _objectModel.SprintAttributes
                                  where x.SprintAttName == "End Date"
                                  from y in _objectModel.SprintAttValues
                                  where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                                  select y.Value).First();

                DateTime cDate = DateTime.Now;
                int orgWorkingHours = new SqlOrganizationRepository().GetDailyHours();
                TimeSpan timeSpanObj = DateTime.Parse(endDate) - new DateTime(cDate.Year, cDate.Month, cDate.Day);

                if (timeSpanObj.Days == 0)
                {
                    // If remaining hours are greater than the organization daily hours then
                    // use organization daily hours to multiply with users count else
                    // use remaining hours.
                    if ((24 - cDate.Hour) < orgWorkingHours)
                        return ((24 - cDate.Hour) * pUsersCount);
                    else
                        return (orgWorkingHours * pUsersCount);
                }
            }

            return (remainingDays * (new SqlOrganizationRepository().GetDailyHours()) * pUsersCount);
        }

        public int GetTotalSprintHours(long pSprintID, int pUsersCount = 0)
        {
            if (pUsersCount == 0)
            {
                // Getting the users available in the sprint.
                pUsersCount = (from x in _objectModel.SprintsBacklogs
                               where x.SprintID == pSprintID
                               from y in _objectModel.AssignedIssues
                               where y.IssueID == x.IssueID
                               select y.EmpID).Distinct().Count();
            }

            int totalDays = GetSprintDuration(pSprintID);

            return (totalDays * (new SqlOrganizationRepository().GetDailyHours()) * pUsersCount);
        }

        public double GetSprintWorkLoad(long pSprintID)
        {
            var issueIDs = from x in _objectModel.SprintsBacklogs
                           where x.SprintID == pSprintID
                           select x.IssueID;

            var manHours = from x in _objectModel.IssueAttributes
                           where x.IssueAttName == "Man Hours"
                           from y in _objectModel.IssueAttValues
                           where issueIDs.Contains(y.IssueID) && y.IssueAttID == x.IssueAttID
                           select y.Value;

            double totalManHours = 0;

            foreach (string hours in manHours)
            {
                totalManHours += double.Parse(hours);
            }

            return totalManHours;
        }

        // Returns the sprint id of the given issue.
        public long GetSprintID(long pIssueID)
        {
            return _objectModel.SprintsBacklogs.Where(x => x.IssueID == pIssueID).Select(y => y.SprintID).First();
        }

        // If the "pIssueID" in greater than 0 then its mean
        // first find the sprint id then check it.
        public bool IsSprintStarted(long pSprintID, long pIssueID = 0)
        {
            if (pIssueID != 0)
            {
                pSprintID = (from x in _objectModel.SprintsBacklogs
                             where x.IssueID == pIssueID
                             select x.SprintID).First();
            }

            string startDate = (from x in _objectModel.SprintAttributes
                                where x.SprintAttName == "Start Date"
                                from y in _objectModel.SprintAttValues
                                where y.SprintID == pSprintID && y.SprintAttID == x.SprintAttID
                                select y.Value).First();

            DateTime cDate = DateTime.Now;
            return (DateTime.Compare(DateTime.Parse(startDate), new DateTime(cDate.Year, cDate.Month, cDate.Day)) < 1);
        }

        public bool SaveMeeting(long pSprintID, long pEmpID, string[] pData)
        {
            bool result = false;
            long meetingID = 0;
            
            // If already meeting has been done on the current date then take the meeting id. 
            var meeting = from x in _objectModel.ScrumMeetings
                          where x.SprintID == pSprintID && (DateTime.Compare(x.MeetingDate, DateTime.Now.Date) == 0)
                          select x.MeetingID;

            if (meeting.Count() > 0)
            {
                meetingID = meeting.First();
                result = true;
            }

            else
            {
                _objectModel.ScrumMeetings.InsertOnSubmit(new ScrumMeeting
                    {
                        SprintID = pSprintID,
                        MeetingDate = DateTime.Now
                    }
                );

                try
                {
                    _objectModel.SubmitChanges();
                    meetingID = _objectModel.ScrumMeetings.Select(x => x.MeetingID).Max();
                    result = true;
                }

                catch (Exception ex)
                {
                }
            }

            if (result)
            {
                result = false;

                _objectModel.MeetingDetails.InsertOnSubmit(new MeetingDetail
                    {
                        EmpID = pEmpID,
                        MeetingID = meetingID,
                        Yesterday = pData[0],
                        Today = pData[1],
                        Tomorrow = pData[2],
                        Comments = pData[3]
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

        public MeetingDetail MeetingDetails(long pSprintID, long pEmpID, long pMeetingID)
        {
            var meetingDetails = from y in _objectModel.MeetingDetails
                                 where y.MeetingID == pMeetingID && y.ScrumMeeting.SprintID == pSprintID && y.EmpID == pEmpID
                                 select y;

            return meetingDetails.FirstOrDefault();
        }

        public bool EditMeeting(long pMeetingID, long pEmpID, string[] pData)
        {
            bool result = false;
            var meetingDetails = (from x in _objectModel.MeetingDetails
                                 where x.MeetingID == pMeetingID && x.EmpID == pEmpID
                                 select x).First();

            meetingDetails.Yesterday = pData[0];
            meetingDetails.Today = pData[1];
            meetingDetails.Tomorrow = pData[2];
            meetingDetails.Comments = pData[3];

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

        public IQueryable<ScrumMeeting> ScrumMeetings(long pSprintID)
        {
            return _objectModel.ScrumMeetings.Where(x => x.SprintID == pSprintID).Select(y =>  y);
        }

        public bool SaveDailyEffort(long pSprintID, double pHours)
        {
            bool result = false;
            int day = GetSprintDay(pSprintID);
            var obj = _objectModel.SprintDailyEfforts.Where(x => x.SprintID == pSprintID && x.Day == day).FirstOrDefault();

            if (obj != null)
            {
                obj.WorkDone = obj.WorkDone + pHours;
            }

            else
            {
                _objectModel.SprintDailyEfforts.InsertOnSubmit(new SprintDailyEffort
                    {
                        SprintID = pSprintID,
                        Day = day,
                        WorkDone = pHours
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

        public float[] GetDailyEffort(long pSprintID)
        {
            int size=GetSprintDuration(pSprintID)-GetSprintRemainingDays(pSprintID);
            float[] dailyWorkDone = new float[size+1];
            var dailyEffort = from x in _objectModel.SprintDailyEfforts
                              where x.SprintID == pSprintID
                              select x;
            foreach (SprintDailyEffort x in dailyEffort)
            {
                dailyWorkDone[x.Day] = (float)x.WorkDone;
            }
            return dailyWorkDone;
        }
    }
}
