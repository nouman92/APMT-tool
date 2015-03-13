using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.Models;
using System.Text;
using Domain.Entities;
using GoogleChartSharp;

namespace UserInterface.Controllers
{
    [Authorize(Roles = "3")]
    public class SprintController : BaseController
    {
        private ISprintRepository _sprintRepository;
        private IIssueRepository _issueRepository;
        private IEmployeeRepository _empRepository;
        private IProjectRepository _projRepository;

        public SprintController(ISprintRepository pSprintRepository, IIssueRepository pIssueRepository, IEmployeeRepository pEmpRepository, IProjectRepository pProjRepository)
        {
            _projRepository = pProjRepository;
            _sprintRepository = pSprintRepository;
            _issueRepository = pIssueRepository;
            _empRepository = pEmpRepository;
        }

        public ActionResult ProjectSprints()
        {
            Session["projMoreActionsTabNumber"] = 4;
            Session["showSEditButton"] = "";
            
            if (TempData["sprintAttID"] == null)
            {
                return View(_sprintRepository.GetProjectSprints(base.SelectedProjectID));
            }

            else
            {
                return View(_sprintRepository.SearchSprint(int.Parse(TempData["sprintAttID"].ToString()), TempData["sprintAttVal"].ToString(), base.SelectedProjectID));
            }
        }

        public ActionResult CreateSprintTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["createSprintTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        // The default values will help when the data is saved and no need to pass again. Specially 
        // in redirection.
        public ActionResult MoreActionsTabs(long pSprintID = 0, string pSprintName = "", long pProjectID = 0)
        {
            // For redirection from SearchFilter to Sprint
            if (pProjectID != 0)
            {
                base.SelectedProjectID = pProjectID;
                base.SelectedProjectName = _projRepository.GetProjectName(pProjectID);
                Session["selectedMenu"] = "Project"; 
                Session["sprintMoreActionsTabNumber"] = 0;
            }

            if (pSprintID != 0)
            {
                // Saving the selected sprint name and id for further use.
                base.SelectedSprintName = pSprintName;
                base.SelectedSprintID = pSprintID;

                // Please see the "MoreActionsTabs" action in "ProjectController" (same reason is here).
                Session["showSEditButton"] = "Yes";

                Session["sFilterPriority"] = null;  // Please see the FilterIssueList(SprintController) action.
                Session["sFilterState"] = null;
                Session["sFilterType"] = null;
                Session["sFilterAState"] = null;
            }

            ViewData["valid"] = _sprintRepository.IsSprintValid(base.SelectedSprintID);
            return View();
        }

        public ActionResult CreateSprint()
        {
            ClearLists(1);
            return View(_sprintRepository.GetSprintAttributes(CustomControlsInfo.CustomControlsID));
        }

        [HttpPost]
        public ActionResult CreateSprint(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                // Verifying the start date (should be >= the current date).
                DateTime cDate = DateTime.Now;

                if (DateTime.Compare(DateTime.Parse(pFormData["_2"]), new DateTime(cDate.Year, cDate.Month, cDate.Day)) > -1)
                {
                    // Verifying the start date and end date gap (should be at least 1).
                    if (DateTime.Compare(DateTime.Parse(pFormData["_3"]), DateTime.Parse(pFormData["_2"])) == 1)
                    {
                        if (_sprintRepository.CreateSprint(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedProjectID))
                        {
                            base.ShowMessage("Sprint created successfully.", false);
                        }

                        else
                        {
                            base.ShowMessage("Sprint did not create successfully.", true);
                        }

                        ClearLists(1);
                        CustomControlsInfo.CustomControlsID.Clear();
                        return RedirectToAction("MoreActionsTabs", "Project");
                    }

                    base.ShowMessage("Invalid sprint duration", true);
                }

                else
                {
                    base.ShowMessage("The date '" + pFormData["_2"] + "' has been expired", true);
                }
            }

            ClearLists(1);
            TempData["createSprintTabNumber"] = 0;
            return RedirectToAction("CreateSprintTabs");
        }

        public ActionResult DisplaySprint(long pSprintID)
        {
            Session["sprintMoreActionsTabNumber"] = 0;

            // Sprint workload.
            double tHours = _sprintRepository.GetSprintHours(pSprintID), wHours = _sprintRepository.GetSprintWorkLoad(pSprintID), loadPercentage = 0;

            ViewData["tDays"] = _sprintRepository.GetSprintDuration(pSprintID);
            ViewData["rDays"] = _sprintRepository.GetSprintRemainingDays(pSprintID);
            ViewData["tHours"] = tHours;
            ViewData["wHours"] = wHours;

            if (tHours != 0)
                loadPercentage = (wHours / tHours) * 100;

            if (loadPercentage > 100 || (tHours == 0 && wHours > 0))
            {
                ViewData["load"] = "Overloaded";
                loadPercentage = 101; // For second condition.
            }
            else if (loadPercentage < 50 && tHours > 0)
                ViewData["load"] = "Underloaded";
            else
                ViewData["load"] = "";

            ViewData["loadPercent"] = loadPercentage;
            return View(_sprintRepository.GetSprintByID(pSprintID, false));
        }

        public ActionResult EditSprint()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_sprintRepository.GetSprintByID(base.SelectedSprintID, true));
        }

        [HttpPost]
        public ActionResult EditSprint(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_sprintRepository.EditSprint(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedSprintID))
                {
                    base.ShowMessage("Sprint details updated successfully.", false);

                    // Updating the saved name.
                    base.SelectedSprintName = _sprintRepository.GetSprintName(base.SelectedSprintID);
                }

                else
                {
                    base.ShowMessage("Sprint details did not update successfully.", true);
                }

                flag = true;
            }

            // Clearing the lists.
            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("MoreActionsTabs");
            }

            else
            {
                return RedirectToAction("EditSprint");
            }
        }

        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_sprintRepository.GetSprintAttributes(true));
        }

        [HttpPost]
        public ActionResult AddCustomFields(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            int index = 0;
            CustomControlsInfo.CustomControlsID.Clear();

            foreach (string str in data)
            {
                if (str == "Yes")
                {
                    CustomControlsInfo.CustomControlsID.Add(RenderedControlsInfo.L1ControlsID[index]);
                }

                index++;
            }

            ClearLists(1);
            TempData["createSprintTabNumber"] = 0;
            return RedirectToAction("CreateSprintTabs");
        }

        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_sprintRepository.GetFieldTypes().Count()];

            foreach (var item in _sprintRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_sprintRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _sprintRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
            {
                regularExpressions[index++] = new SelectListItem { Text = item.ExpressionName, Value = "" + item.ExpressionID };
            }

            ViewData["regularExpressions"] = regularExpressions;

            // True, False option
            ViewData["trueFalseOption"] = new[] { new SelectListItem { Text = "False", Value = "False" },
                                                  new SelectListItem { Text = "True", Value = "True" }
                                                 };

            // Clearing the lists.
            ClearLists(1);
            return View();
        }

        [HttpPost]
        public ActionResult CreateCustomField(FormCollection pFormData)
        {
            string[] data = new string[6];
            bool flag = false;

            // Collecting and verifying the "Field Name".
            data[0] = (base.CollectData(pFormData))[0];

            if (base.IsValidData)
            {
                // Collecting remaining values.
                for (int index = 1; index < pFormData.Count; index++)
                {
                    data[index] = pFormData[index];
                }

                int result = _sprintRepository.CreateCustomField(data);

                if (result == 1)
                {
                    base.ShowMessage("Custom Field created successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("Custom Field did not create successfully.", true);
                }

                else
                {
                    base.ShowMessage("Custom Field '" + data[0] + "' already exists.", true);
                }

                flag = true;
            }

            ClearLists(1);

            if (flag)
            {
                TempData["createSprintTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["createSprintTabNumber"] = 2;
            }

            return RedirectToAction("CreateSprintTabs");
        }

        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _sprintRepository.IsFieldExists(pFieldName, pFieldID);
        }

        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["createSprintTabNumber"] = 1;
            return View(_sprintRepository.GetFieldByID(pFieldID));
        }

        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["createSprintTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(2);
            return View(_sprintRepository.GetFieldByID(pFieldID));
        }

        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);

            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _sprintRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

                if (result == 1)
                {
                    base.ShowMessage("Field details updated successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("Field details did not update successfully.", true);
                }

                else
                {
                    base.ShowMessage("Custom Field '" + data[0] + "' already exists.", true);
                }

                ClearLists(2);
                // Tab number to be opened.
                TempData["createSprintTabNumber"] = 1;
                return RedirectToAction("CreateSprintTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        public ActionResult SearchSprint()
        {
            // Creating field types' list.
            var attributes = _sprintRepository.GetSprintAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Sprint ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.SprintAttName, Value = "" + item.SprintAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            return View();
        }

        [HttpPost]
        public ActionResult SearchSprint(FormCollection pFormData)
        {
            TempData["sprintAttID"] = pFormData[0];
            TempData["sprintAttVal"] = pFormData[1];
            
            return RedirectToAction("MoreActionsTabs", "Project");
        }

        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Sprint ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["sprintIDField"] = "Yes";
                SprintAttribute attribute = new SprintAttribute
                {
                    SprintAttID = 0,
                    CanNull = false,
                    DefaultValue = "",
                    FieldType = 1,
                    SprintAttName = "Sprint ID"
                };

                return View(attribute);
            }

            ViewData["sprintIDField"] = null;
            return View(_sprintRepository.GetFieldByID(pFieldID));
        }

        public ActionResult SprintBacklog()
        {
            List<long> issueIDs;
            List<bool> isIssueCompleted = new List<bool>();
            bool flag = true;
            int priority = 0, state = 0, type = 0, aState = 0;

            if (TempData["issueAttID"] == null)
            {
                issueIDs = _sprintRepository.SprintBacklog(base.SelectedSprintID);

                // If backlog is empty then do not user filter values as list is empty.
                if (issueIDs.Count == 0)
                    flag = false;
            }

            // If user has searched for the issues, then go for that (TempData has the specified search info).
            else
            {
                issueIDs = _issueRepository.SearchSprintBacklogIssue(int.Parse(TempData["issueAttID"].ToString()), TempData["issueAttVal"].ToString(), base.SelectedSprintID);
                ViewData["filtered"] = "Yes";
                flag = false;
            }

            // If user specifed filter options then use the options to filter the list.
            if (Session["sFilterPriority"] != null && flag)
            {
                priority = int.Parse(Session["sFilterPriority"].ToString());
                state = int.Parse(Session["sFilterState"].ToString());
                type = int.Parse(Session["sFilterType"].ToString());
                aState = int.Parse(Session["sFilterAState"].ToString());

                // For notification purpose (i.e. if no match is found).
                ViewData["filtered"] = "Yes";
            }

            // Sprint workload.
            double tHours = _sprintRepository.GetSprintHours(base.SelectedSprintID), wHours = _sprintRepository.GetSprintWorkLoad(base.SelectedSprintID), loadPercentage = 0;

            ViewData["tHours"] = tHours;
            ViewData["wHours"] = wHours;

            if (tHours != 0)
                loadPercentage = (wHours / tHours) * 100;

            if (loadPercentage > 100 || (tHours == 0 && wHours > 0))
            {
                ViewData["load"] = "Overloaded";
                loadPercentage = 101; // For second condition.
            }
            else if (loadPercentage < 50 && tHours > 0)
                ViewData["load"] = "Underloaded";
            else
                ViewData["load"] = "";

            ViewData["loadPercent"] = loadPercentage;
            ViewData["valid"] = _sprintRepository.IsSprintValid(base.SelectedSprintID);

            var issues = _issueRepository.GetIssues(issueIDs, state, type, priority, aState);

            if (issues != null)
            {
                // If issue is in the last state then hide the assign button as last state
                // means issue has been resolved.
                foreach (var issue in issues)
                {
                    isIssueCompleted.Add(_issueRepository.IsIssueCompleted(issue.IssueID));
                }
            }

            ViewData["isIssueCompleted"] = isIssueCompleted;
            Session["sprintMoreActionsTabNumber"] = 1;
            Session["showIEditButton"] = "";

            return View(issues);
        }

        public ActionResult FilterIssueList(FormCollection pFormData)
        {
            // Saving the use selection.
            Session["sFilterPriority"] = pFormData[0];
            Session["sFilterState"] = pFormData[1];
            Session["sFilterType"] = pFormData[2];
            Session["sFilterAState"] = pFormData[3];

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult RemoveSprintIssue(long pIssueID)
        {
            if (_issueRepository.RemoveSprintIssue(pIssueID, base.SelectedSprintID))
            {
                base.ShowMessage("Issue removed successfully", false);
            }

            else
            {
                base.ShowMessage("Issue did not remove successfully", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult AddIssue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddIssue(FormCollection pFormData)
        {
            if (base.VerifyInput(pFormData[0], @"^\d+$"))
            {
                int result = _issueRepository.AddIssueInSprint(base.SelectedSprintID, int.Parse(pFormData[0]), base.SelectedProjectID);

                switch (result)
                {
                    case 0:

                        base.ShowMessage("Issue already exists in the backlog", true);
                        break;

                    case 1:

                        base.ShowMessage("Issue added successfully", false);
                        break;

                    case 2:

                        base.ShowMessage("Issue not exists in Project Backlog", true);
                        break;

                    case 3:

                        base.ShowMessage("Issue already exists in some other sprint", true);
                        break;

                    case -1:

                        base.ShowMessage("Issue did not add successfully", true);
                        break;
                }
            }

            else
            {
                base.ShowMessage("Invalid issue id", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        // Will return issues' list to add in sprint backlog.
        [Authorize(Roles = "3")]
        public ActionResult ShowAllIssues()
        {
            // Getting existing issues' ids to filter out the list
            var existingIssuesIDs = _issueRepository.GetAllSprintsIssues(base.SelectedProjectID);

            return View(_issueRepository.GetIssuesforSprint(existingIssuesIDs, base.SelectedProjectID));
        }

        // Add the issues in the sprint backlog.
        [HttpPost]
        public ActionResult AddIssueList(FormCollection pFormData)
        {
            List<long> issuesIDs = new List<long>();

            foreach (string str in pFormData)
            {
                if (pFormData[str] != "false")
                {
                    issuesIDs.Add(int.Parse(pFormData[str].Split(',')[0]));
                }
            }

            if (issuesIDs.Count > 0)
            {
                if (_issueRepository.AddIssueInSprint(base.SelectedSprintID, issuesIDs))
                {
                    base.ShowMessage("Issue(s) added successfully", false);
                }

                else
                {
                    base.ShowMessage("Issue(s) did not add successfully", true);
                }
            }

            else
            {
                base.ShowMessage("No issue was selected", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult AssignIssue(long pIssueID)
        {
            List<string> depList = new List<string>();
            List<double> loadedHoursList = new List<double>();
            List<double> loadPercentageList = new List<double>();
            var issues = _issueRepository.CheckIssueDependency(pIssueID);
            double loadedHours, availableHours;

            // Warn the user about issue dependency if any.
            if (issues != null)
            {
                foreach (var issue in _issueRepository.CheckIssueDependency(pIssueID))
                {
                    depList.Add(issue.Value + ", ID: " + issue.IssueID);
                }
            }

            var emps = _empRepository.GetEmployeesFilteredList(pIssueID);
            availableHours = _sprintRepository.GetSprintHours(base.SelectedSprintID, 1);

            // Getting the man hours of each employee and calculating the workload.
            foreach (var emp in emps)
            {
                loadedHours = _empRepository.GetEmployeeWorkLoad(emp.EmpID, base.SelectedSprintID);
                loadedHoursList.Add(loadedHours);
                loadPercentageList.Add((loadedHours / availableHours) * 100);
            }

            ViewData["manHours"] = _issueRepository.GetIssueManHours(pIssueID);
            ViewData["depList"] = depList;
            ViewData["availableManHours"] = availableHours;
            ViewData["loadedHoursList"] = loadedHoursList;
            ViewData["loadPercentageList"] = loadPercentageList;
            ViewData["issueID"] = pIssueID;

            return View(emps);
        }

        [HttpPost]
        public ActionResult AssignIssue(FormCollection pFormData, long pIssueID)
        {
            List<long> empsIDs = new List<long>();

            foreach (string str in pFormData)
            {
                if (pFormData[str] != "false")
                {
                    empsIDs.Add(int.Parse(pFormData[str].Split(',')[0]));
                }
            }

            if (empsIDs.Count > 0)
            {
                if (_issueRepository.AssignIssue(pIssueID, empsIDs))
                {
                    base.ShowMessage("Issue assigned successfully", false);
                }

                else
                {
                    base.ShowMessage("Issue did not assign successfully", true);
                }
            }

            else
            {
                base.ShowMessage("No user was selected", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult ScrumMeetingTabs()
        {
            return View();
        }

        public ActionResult ScrumMeeting()
        { 
            // Creating users list available in current sprint.
            var emps = _empRepository.GetSprintTeam(base.SelectedSprintID, DateTime.Now.Date);

            if (emps != null)
            {
                SelectListItem[] users = new SelectListItem[emps.Count()];
                int index = 0;

                foreach (var item in emps)
                {
                    users[index++] = new SelectListItem { Text = item.Value + ", ID: " + item.EmpID, Value = "" + item.EmpID };
                }

                ViewData["users"] = users;
            }

            return View();
        }

        [HttpPost]
        public ActionResult ScrumMeeting(FormCollection pFormData)
        {
            string[] data = new string[4];

            data[0] = pFormData[1];  // Yesterday
            data[1] = pFormData[2];  // Today
            data[2] = pFormData[3];  // Tomorrow
            data[3] = pFormData[4];  // Comments

            if (_sprintRepository.SaveMeeting(base.SelectedSprintID, long.Parse(pFormData[0]), data))
            {
                base.ShowMessage("Meeting saved successfully", false);
            }

            else
            {
                base.ShowMessage("No user was selected", true);
            }

            return View("ScrumMeetingTabs");
        }

        public ActionResult SprintTeam(long pMeetingID)
        {
            TempData["meetingID"] = pMeetingID;

            // Creating users list available in current sprint.
            var emps = _empRepository.GetSprintTeam(base.SelectedSprintID, null);

            if (emps != null)
            {
                SelectListItem[] users = new SelectListItem[emps.Count()];
                int index = 0;

                foreach (var item in emps)
                {
                    users[index++] = new SelectListItem { Text = item.Value + ", ID: " + item.EmpID, Value = "" + item.EmpID };
                }

                ViewData["users"] = users;
            }

            return View();
        }

        public ActionResult MeetingsList()
        {
            Session["sprintMoreActionsTabNumber"] = 2;
            return View(_sprintRepository.ScrumMeetings(base.SelectedSprintID));
        }

        public ActionResult MeetingDetails(long pEmpID)
        {
            ViewData["userName"] = _empRepository.GetEmployeeName(pEmpID);
            return View(_sprintRepository.MeetingDetails(base.SelectedSprintID, pEmpID, long.Parse(TempData["meetingID"].ToString())));
        }

        public ActionResult EditMeetingDetails(long pEmpID)
        {
            ViewData["userName"] = _empRepository.GetEmployeeName(pEmpID);
            return View(_sprintRepository.MeetingDetails(base.SelectedSprintID, pEmpID, long.Parse(TempData["meetingID"].ToString())));
        }

        [HttpPost]
        public ActionResult EditMeetingDetail(FormCollection pFormData, long pMeetingID, long pEmpID)
        {
            string[] data = new string[4];

            data[0] = pFormData[0];  // Yesterday
            data[1] = pFormData[1];  // Today
            data[2] = pFormData[2];  // Tomorrow
            data[3] = pFormData[3];  // Tomorrow

            if (_sprintRepository.EditMeeting(pMeetingID, pEmpID, data))
            {
                base.ShowMessage("Meeting details updated successfully", false);
            }

            else
            {
                base.ShowMessage("Meeting details did not update successfully", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult BurndownChart(long pSprintID)
        {
            // Note we have to specify a MultiDataSet line chart
            LineChart lineChart = new LineChart(650, 450, LineChartType.MultiDataSet);
            //Create an axis along the bottom of the graph
            ChartAxis bottomAxis = new ChartAxis(ChartAxisType.Bottom);
            ChartAxis bottomAxis1 = new ChartAxis(ChartAxisType.Bottom);
            //Create an axis along the left of the graph
            ChartAxis leftAxis = new ChartAxis(ChartAxisType.Left);
            ChartAxis leftAxis1 = new ChartAxis(ChartAxisType.Left);
            
            
            float tHours = _sprintRepository.GetTotalSprintHours(pSprintID), twHours = (float)_sprintRepository.GetSprintWorkLoad(pSprintID);
            int tDays = _sprintRepository.GetSprintDuration(pSprintID);
            int rDays = _sprintRepository.GetSprintRemainingDays(pSprintID);
            float[] daliyWorkDone = _sprintRepository.GetDailyEffort(pSprintID);
            float[] xaxis_expectedWorkflow = new float[tDays + 1];
            float[] yaxis_expectedWorkflow = new float[tDays + 1];
            float[] xaxis_actualWorkflow = new float[daliyWorkDone.Length];
            float[] yaxis_actualWorkflow = new float[daliyWorkDone.Length];
            for (int i = 0; i < daliyWorkDone.Length; i++)
            {
                twHours+=daliyWorkDone[i];
            }
            float expectedWorkDone = twHours / tDays;
            float remainingEffort = twHours;
            float xaxisMaxLimit = tDays, yaxisMaxLimit = tHours > twHours ? tHours : twHours;
            
            for (int i = 0; i <= tDays; i++)
            {
                float temp = i;
                xaxis_expectedWorkflow[i] = (temp / tDays) * 100;
                yaxis_expectedWorkflow[i] = ((twHours - (temp * expectedWorkDone)) / yaxisMaxLimit )*100;
                if (i%(tDays/21+1)==0)
                {
                    bottomAxis.AddLabel(new ChartAxisLabel("" + i, xaxis_expectedWorkflow[i]));
                }
            }
            for (int i = 0; i < daliyWorkDone.Length; i++)
            {
                float temp = i;
                xaxis_actualWorkflow[i] = (temp / tDays) * 100;
                yaxis_actualWorkflow[i] = ((remainingEffort-daliyWorkDone[i]) / yaxisMaxLimit) * 100;
                remainingEffort = remainingEffort - daliyWorkDone[i];
                lineChart.AddShapeMarker(new ShapeMarker(ShapeMarkerType.Circle, "FF6600", 1, i, 10));
            }
            List<float[]> datasets = new List<float[]>();
            datasets.Add(xaxis_expectedWorkflow);
            datasets.Add(yaxis_expectedWorkflow);
            datasets.Add(xaxis_actualWorkflow);
            datasets.Add(yaxis_actualWorkflow);

            lineChart.SetData(datasets);

            //bottomAxis.SetRange(0, (int)xaxisMaxLimit);
            bottomAxis.Color = "FFFFFF";
            bottomAxis.FontSize = 12;
            
            
            bottomAxis1.SetRange(0, (int)xaxisMaxLimit);
            bottomAxis1.Color = "FFFFFF";
            bottomAxis1.FontSize = 16;
            ChartAxisLabel xchartAxisLabel = new ChartAxisLabel("Days", tDays / 2);
            bottomAxis1.AddLabel(xchartAxisLabel);

            leftAxis.SetRange(0, (int)yaxisMaxLimit);
            leftAxis.Color = "FFFFFF";
            leftAxis.FontSize = 12;

            leftAxis1.SetRange(0, (int)yaxisMaxLimit);
            leftAxis1.Color = "FFFFFF";
            leftAxis1.FontSize = 16;
            ChartAxisLabel ychartAxisLabel = new ChartAxisLabel("Effort in Hours", yaxisMaxLimit / 2);
            leftAxis1.AddLabel(ychartAxisLabel);

            lineChart.AddAxis(bottomAxis);
            lineChart.AddAxis(bottomAxis1);
            lineChart.AddAxis(leftAxis);
            lineChart.AddAxis(leftAxis1);
            
            // Assign a color to each dataset in the same order the datasets were added
            lineChart.SetDatasetColors(new string[] { "ADFF2F", "FF6600" });

            // Specify legend labels in the same way
            // The legend will use the dataset color
            lineChart.SetLegend(new string[] { "Ideal Burndown", "Actual Burndown" });
            
            LineStyle lineStyle = new LineStyle(3,0,0);
            lineChart.AddLineStyle(lineStyle);
            lineChart.AddLineStyle(lineStyle);
            lineChart.SetGrid(0, 20,5,5);
            // Specify the area to fill (background or chart area)
            // and the fill color
            SolidFill bgFill = new SolidFill(ChartFillTarget.Background, "6A6868");
            SolidFill chartAreaFill = new SolidFill(ChartFillTarget.ChartArea, "303030");

            // In this case we are applying solid fills to both the background and the
            // chart area
            lineChart.AddSolidFill(bgFill);
            lineChart.AddSolidFill(chartAreaFill);



            TempData["Link"] = lineChart.GetUrl() + "&chdls=FFFFFF,16";
            return View();
        }
    }
}
