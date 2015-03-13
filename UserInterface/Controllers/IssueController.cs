using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.Models;
using Domain.Entities;

namespace UserInterface.Controllers
{
    public class IssueController : BaseController
    {
        private IIssueRepository _issueRepository;
        private IEmployeeRepository _empRepository;
        private IProjectRepository _projRepository;
        private ISprintRepository _sprintRepository;

        public IssueController(IIssueRepository pIssueRepository, IEmployeeRepository pEmpRepository, IProjectRepository pProjectRepository, ISprintRepository pSprintRepository)
        {
            _issueRepository = pIssueRepository;
            _empRepository = pEmpRepository;
            _projRepository = pProjectRepository;
            _sprintRepository = pSprintRepository;
        }

        public Domain.Repository.Abstract.IIssueRepository IIssueRepository
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Domain.Repository.Abstract.IEmployeeRepository IEmployeeRepository
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Domain.Repository.Abstract.IProjectRepository IProjectRepository
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Domain.Repository.Abstract.ISprintRepository ISprintRepository
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult CreateIssueTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["createIssueTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        [Authorize(Roles = "4")]
        public ActionResult IssueTabs()
        {
            return View();
        }

        [Authorize]
        public ActionResult DisplayIssue(long pIssueID)
        {
            string projectName, sprintName;

            var issue = _issueRepository.GetIssueByID(pIssueID, out projectName, out sprintName);
            ViewData["issueProjectName"] = projectName;
            ViewData["issueSprintName"] = sprintName;
            ViewData["assignedto"] = "";

            // Creating list of employees who are working on this issue.
            List<string> empList = _empRepository.GetEmployeesNames(pIssueID);

            if(empList.Count > 0)
            {
                SelectListItem[] emps = new SelectListItem[empList.Count];
                int index = 0;

                foreach (var item in empList)
                {
                    emps[index++] = new SelectListItem { Text = item, Value = "" + index };
                }

                ViewData["assignedto"] = emps;
            }

            return View(issue);
        }

        [Authorize(Roles = "3")]
        public ActionResult CreateIssue()
        {
            int index = 0;

            // Creating issue priority list.
            SelectListItem[] issuePriorities = new SelectListItem[_issueRepository.GetIssuePriorityList().Count()];

            foreach (var item in _issueRepository.GetIssuePriorityList())
            {
                issuePriorities[index++] = new SelectListItem { Text = item.PriorityName, Value = "" + item.PriorityID };
            }

            ViewData["issuePriorities"] = issuePriorities;

            // Creating issue type list.
            SelectListItem[] issueTypes = new SelectListItem[_issueRepository.GetIssueTypeList().Count()];
            index = 0;

            foreach (var item in _issueRepository.GetIssueTypeList())
            {
                issueTypes[index++] = new SelectListItem { Text = item.TypeName, Value = "" + item.TypeID };
            }

            ViewData["issueTypes"] = issueTypes;

            // Clearing the lists.
            ClearLists(1);
            return View(_issueRepository.GetIssueAttributes(CustomControlsInfo.CustomControlsID));
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public ActionResult CreateIssue(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            int[] otherInfo = new int[2];

            if (base.IsValidData)
            {
                // Collecting other info (the issue type and issue priority)
                otherInfo[0] = int.Parse(pFormData[0]);  // Issue type
                otherInfo[1] = int.Parse(pFormData[1]);  // Issue priority

                if (_issueRepository.CreateIssue(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedProjectID, otherInfo))
                {
                    // See the "CreateIssue.aspx" to understand the following.
                    TempData["askForMore"] = "Yes";
                }

                else
                {
                    base.ShowMessage("Issue did not create successfully.", true);
                }

                ClearLists(1);
                CustomControlsInfo.CustomControlsID.Clear();
            }

            else
            {
                ClearLists(1);
                TempData["createIssueTabNumber"] = 0;
            }

            return RedirectToAction("CreateIssueTabs");
        }

        [Authorize(Roles = "3")]
        public int AddIssueType(string pTypeName)
        {
            if (base.VerifyInput(pTypeName, @"^[a-zA-Z\s]*$"))
            {
                return _issueRepository.AddIssueType(pTypeName);
            }

            else
            {
                return -2;
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult EditIssue()
        {
            string projectName, sprintName;
            int index = 0;
            var issue = _issueRepository.GetIssueByID(base.SelectedIssueID, out projectName, out sprintName);
            
            // Creating issue priority list.
            SelectListItem[] priorities = new SelectListItem[_issueRepository.GetIssuePriorityList().Count()];

            foreach (var item in _issueRepository.GetIssuePriorityList())
            {
                priorities[index++] = new SelectListItem { Text = item.PriorityName, Value = "" + item.PriorityID };
            }

            ViewData["priorities"] = priorities;
            ViewData["issuePriority"] = issue.First().Issue.PriorityID; // Selected item.

            // Creating issue type list.
            SelectListItem[] types = new SelectListItem[_issueRepository.GetIssueTypeList().Count()];
            index = 0;

            foreach (var item in _issueRepository.GetIssueTypeList())
            {
                types[index++] = new SelectListItem { Text = item.TypeName, Value = "" + item.TypeID };
            }

            ViewData["types"] = types;
            ViewData["issueType"] = issue.First().Issue.TypeID; // Selected item.

            // Creating issue state list.
            SelectListItem[] states = new SelectListItem[_issueRepository.GetIssueStateList(true, (int)issue.First().Issue.StateID).Count()];
            index = 0;

            foreach (var item in _issueRepository.GetIssueStateList(true, (int)issue.First().Issue.StateID))
            {
                states[index++] = new SelectListItem { Text = item.StateName, Value = "" + item.StateID };
            }

            ViewData["states"] = states;
            ViewData["issueState"] = issue.First().Issue.StateID; // Selected item.

            // Clearing the lists.
            ClearLists(1);
            return View(issue);
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public ActionResult EditIssue(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            int[] otherInfo = new int [3];
            bool flag = false;

            if (base.IsValidData)
            {
                // Collecting other info (the issue type, issue priority and issue state)
                otherInfo[0] = int.Parse(pFormData[0]);  // Issue state
                otherInfo[1] = int.Parse(pFormData[1]);  // Issue type
                otherInfo[2] = int.Parse(pFormData[2]);  // Issue priority

                if (_issueRepository.EditIssue(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedIssueID, otherInfo))
                {
                    base.ShowMessage("Issue details updated successfully.", false);

                    // Updating the saved name.
                    base.SelectedIssueName = _issueRepository.GetIssueName(base.SelectedIssueID);
                }

                else
                {
                    base.ShowMessage("Issue details did not update successfully.", true);
                }

                flag = true;
            }

            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("MoreActionsTabs");
            }

            else
            {
                return RedirectToAction("EditIssue");
            }
        }

        [Authorize(Roles = "3")]
        public ActionResult FilterIssueListOptions()
        {
            // Creating issue priority list.
            SelectListItem[] priorities = new SelectListItem[_issueRepository.GetIssuePriorityList().Count()+1];
            int index = 0;

            foreach (var item in _issueRepository.GetIssuePriorityList())
            {
                priorities[index++] = new SelectListItem { Text = item.PriorityName, Value = "" + item.PriorityID };
            }

            priorities[index] = new SelectListItem { Text = "Any", Value = "" + 0 };

            ViewData["priorities"] = priorities;
            ViewData["filterPriority"] = (Session["filterPriority"] != null) ? Session["filterPriority"] : 1; // Select the previously selected item if available.

            // Creating issue type list.
            SelectListItem[] types = new SelectListItem[_issueRepository.GetIssueTypeList().Count()+1];
            index = 0;

            foreach (var item in _issueRepository.GetIssueTypeList())
            {
                types[index++] = new SelectListItem { Text = item.TypeName, Value = "" + item.TypeID };
            }

            types[index] = new SelectListItem { Text = "Any", Value = "" + 0 };

            ViewData["types"] = types;
            ViewData["filterType"] = (Session["filterType"] != null) ? Session["filterType"] : 1; // Select the previously selected item if available.

            // Creating issue state list.
            SelectListItem[] states = new SelectListItem[_issueRepository.GetIssueStateList().Count()+1];
            index = 0;

            foreach (var item in _issueRepository.GetIssueStateList())
            {
                states[index++] = new SelectListItem { Text = item.StateName, Value = "" + item.StateID };
            }

            states[index] = new SelectListItem { Text = "Any", Value = "" + 0 };

            ViewData["states"] = states;
            ViewData["filterState"] = (Session["filterState"] != null) ? Session["filterState"] : 1; // Select the previously selected item if available.

            // Creating issue assignment state list.
            SelectListItem[] aStates = new SelectListItem[3];

            aStates[0] = new SelectListItem { Text = "Assigned", Value = "" + 1 };
            aStates[1] = new SelectListItem { Text = "Unassigned", Value = "" + 2 };
            aStates[2] = new SelectListItem { Text = "Any", Value = "" + 0 };

            ViewData["aStates"] = aStates;
            ViewData["filterAState"] = (Session["filterAState"] != null) ? Session["filterAState"] : 1; // Select the previously selected item if available.

            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_issueRepository.GetIssueAttributes(true));
        }

        [Authorize(Roles = "3")]
        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _issueRepository.IsFieldExists(pFieldName, pFieldID);
        }

        [Authorize(Roles = "3")]
        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_issueRepository.GetFieldTypes().Count()];

            foreach (var item in _issueRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_issueRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _issueRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
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

        [Authorize(Roles = "3")]
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

                int result = _issueRepository.CreateCustomField(data);

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
                TempData["createIssueTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["createIssueTabNumber"] = 2;
            }

            return RedirectToAction("CreateIssueTabs");
        }

        [Authorize(Roles = "3")]
        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["createIssueTabNumber"] = 1;
            return View(_issueRepository.GetFieldByID(pFieldID));
        }

        [Authorize(Roles = "3")]
        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["createIssueTabNumber"] = 1;

            // Clearing lists.
            ClearLists(2);
            return View(_issueRepository.GetFieldByID(pFieldID));
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);

            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _issueRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

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
                TempData["createIssueTabNumber"] = 1;
                return RedirectToAction("CreateIssueTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        [Authorize(Roles = "3")]
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
            TempData["createIssueTabNumber"] = 0;
            return RedirectToAction("CreateIssueTabs");
        }

        // The default values will help when the data is saved and no need to pass again. 
        // Specially in redirection.
        [Authorize(Roles = "3")]
        public ActionResult MoreActionsTabs(long pIssueID = 0, string pIssueName = "", long pProjectID = 0)
        {
            if (pProjectID != 0)
            {
                pProjectID = _projRepository.GetProjectID(pIssueID);
                base.SelectedProjectName = _projRepository.GetProjectName(pProjectID);
                base.SelectedProjectID = pProjectID;
                Session["selectedMenu"] = "Issue";
                Session["issueMoreActionsTabNumber"] = 0;
            }
            if (pIssueID != 0)
            {
                // Saving the selected issue name and id for further use.
                base.SelectedIssueName = pIssueName;
                base.SelectedIssueID = pIssueID;
            }

            return View();
        }

        // The first tab of "MoreActionsTabs".
        [Authorize]
        public ActionResult IssueDetails(long pIssueID)
        {
            // In the "DisplayIssue" view, there is an "Edit" button and that should be
            // shown to user only if he/she is in "MoreActionTabs". Actually, this view
            // is called from some other places where the edit button is not required like
            // in search.
            Session["showIEditButton"] = "Yes";
            Session["issueMoreActionsTabNumber"] = 0;
            return RedirectToAction("DisplayIssue", new { pIssueID = pIssueID });
        }

        // Called in "IssueDependency" for autocomplete help.
        [Authorize(Roles = "3")]
        public JsonResult GetIssuesIDs(long pSearchID)
        {
            return Json(_issueRepository.GetIssueIDs(pSearchID, base.SelectedProjectID), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "3")]
        public ActionResult IssueDependency()
        {
            Session["showIEditButton"] = "";
            Session["issueMoreActionsTabNumber"] = 1;
            return View(_issueRepository.GetIssueDependency(base.SelectedIssueID));
        }

        // Returns the issue name of the given issue id.
        [Authorize(Roles = "3")]
        public string IssueName(long pIssueID)
        {
            string name = _issueRepository.GetIssueName(pIssueID);
            return (name == null) ? "Issue not exists" : name;
        }

        // Will return issues' list to select for dependency.
        [Authorize(Roles = "3")]
        public ActionResult ShowAllIssues()
        {
            // Getting existing issues' ids to filter out the list
            var existingIssuesIDs = _issueRepository.GetIssueDependency(base.SelectedIssueID);
            List<long> idsList = new List<long>();

            idsList.Add(base.SelectedIssueID);

            if (existingIssuesIDs != null)
            {
                idsList.AddRange(existingIssuesIDs.Select(x => x.IssueID).ToList());
            }

            return View(_issueRepository.GetIssuesforDependency(idsList, base.SelectedProjectID));
        }

        [Authorize(Roles = "3")]
        public ActionResult AddIssueDependency()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public ActionResult AddIssueDependency(FormCollection pFormData)
        {
            if (base.VerifyInput(pFormData[0], @"^\d+$"))
            {
                int result = _issueRepository.AddIssueDependency(base.SelectedIssueID, int.Parse(pFormData[0]), base.SelectedProjectID);

                switch (result)
                {
                    case 0:

                        base.ShowMessage("Invalid dependency", true);
                        break;

                    case 1:

                        base.ShowMessage("Dependency added successfully", false);
                        break;

                    case 2:

                        base.ShowMessage("Issue not exists in Project Backlog", true);
                        break;

                    case 3:

                        base.ShowMessage("Cross dependency or dependency already exists", true);
                        break;

                    case -1:

                        base.ShowMessage("Dependency did not add successfully", true);
                        break;
                }
            }

            else
            {
                base.ShowMessage("Invalid issue id", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public ActionResult AddIssueDependencyList(FormCollection pFormData)
        {
            List<long> DependsOnIDs = new List<long>();

            foreach (string str in pFormData)
            {
                if (pFormData[str] != "false")
                {
                    DependsOnIDs.Add(int.Parse(pFormData[str].Split(',')[0]));
                }
            }

            if (DependsOnIDs.Count > 0)
            {
                if (_issueRepository.AddIssueDependency(base.SelectedIssueID, DependsOnIDs))
                {
                    base.ShowMessage("Dependency added successfully", false);
                }

                else
                {
                    base.ShowMessage("Dependency did not add successfully", true);
                }
            }

            else 
            {
                base.ShowMessage("No issue was selected", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        [Authorize(Roles = "3")]
        public ActionResult RemoveIssueDependency(long pDependsOnID)
        {
            if (_issueRepository.RemoveIssueDependency(base.SelectedIssueID, pDependsOnID))
            {
                base.ShowMessage("Issue dependency removed successfully", false);
            }

            else
            {
                base.ShowMessage("Issue dependency did not remove successfully", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        [Authorize(Roles = "3")]
        public ActionResult IssueAssignee()
        {
            Session["issueMoreActionsTabNumber"] = 2;
            return View(_empRepository.GetEmployees(base.SelectedIssueID));
        }

        [Authorize(Roles = "3")]
        public ActionResult RemoveIssueAssignee(long pEmpID)
        {
            if (_issueRepository.RemoveIssueAssignee(base.SelectedIssueID, pEmpID))
            {
                base.ShowMessage("Issue assignee removed successfully", false);
            }

            else
            {
                base.ShowMessage("Issue assignee did not remove successfully", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        [Authorize(Roles = "4")]
        public ActionResult WorkFlow()
        {
            Session["issueTabNumber"] = 0;
            return View(_issueRepository.GetIssueStateList().OrderBy(x => x.StateRank));
        }

        [Authorize(Roles = "4")]
        [HttpPost]
        public ActionResult UpdateWorkFlow(FormCollection pFormData)
        {
            int size = _issueRepository.GetIssueStateList().Count(), index = 0;
            int[] statesIDs = new int[size], statesRanks = new int[size];
            bool[] active = new bool[size];
            bool flag = false;

            foreach (string key in pFormData.Keys)
            {
                statesIDs[index] = int.Parse(key.Substring(1));
                statesRanks[index] = index+1;

                // If condition is true then it means the state is active.
                if (pFormData[key].IndexOf('Y') != -1)
                {
                    active[index] = true;

                    // If no state is avtive then show error message to user.
                    // There should be at least one active state as issue can
                    // not be state less.
                    flag = true;
                }

                else
                {
                    active[index] = false;
                }

                index++;
            }

            if (flag)
            {
                flag = false;

                if (_issueRepository.UpdateWorkFlow(statesIDs, statesRanks, active))
                {
                    base.ShowMessage("Work-Flow updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Work-Flow did not update successfully.", true);
                }
            }

            else
            {
                base.ShowMessage("There should be at least one active state", true);
            }

            return View("IssueTabs");
        }

        [Authorize(Roles = "4")]
        [HttpPost]
        public ActionResult AddState(FormCollection pFormData)
        {
            if (base.VerifyInput(pFormData[0], @"^[a-zA-Z\s]*$"))
            {
                int result = _issueRepository.AddIssueState(pFormData[0]);

                if (result == 1)
                {
                    base.ShowMessage("State added successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("State did not add successfully.", true);
                }

                else
                {
                    base.ShowMessage("State '" + pFormData[0] + "' already exists.", true);
                }
            }

            else
            {
                base.ShowMessage("Invalid state name.", true);
            }

            return RedirectToAction("IssueTabs");
        }

        [Authorize(Roles = "4")]
        public ActionResult RenameState()
        {
            // Creating issue state list.
            SelectListItem[] states = new SelectListItem[_issueRepository.GetIssueStateList().Count()];
            int index = 0;

            foreach (var item in _issueRepository.GetIssueStateList())
            {
                states[index++] = new SelectListItem { Text = item.StateName, Value = "" + item.StateID };
            }

            ViewData["states"] = states;

            return View();
        }

        [Authorize(Roles = "4")]
        [HttpPost]
        public ActionResult RenameState(FormCollection pFormData)
        {
            if (base.VerifyInput(pFormData[1], @"^[a-zA-Z\s]*$"))
            {
                int result = _issueRepository.EditState(int.Parse(pFormData[0]), pFormData[1]);

                if (result == 1)
                {
                    base.ShowMessage("State renamed successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("State did not rename successfully.", true);
                }

                else
                {
                    base.ShowMessage("State '" + pFormData[1] + "' already exists.", true);
                }
            }

            else
            {
                base.ShowMessage("Invalid state name.", true);
            }

            return RedirectToAction("IssueTabs");
        }

        [Authorize]
        public ActionResult ExecuteWorkFlow(long pIssueID)
        {
            if (_issueRepository.ExecuteWorkFlow(pIssueID, base.UserID))
            {
                base.ShowMessage("Issue work-flow executed successfully.", false);
            }

            else
            {
                base.ShowMessage("Issue work-flow did not execute successfully.", false);
            }

            return RedirectToAction("HomeTabs", "Home");
        }

        [Authorize]
        public int WorkDone(long pIssueID, double pHours)
        {
            int retVal;

            if (base.VerifyInput(pHours + "", @"^\d+((.25)|(.50)|(.5)|(.75)|(.0)|(.00))?$"))
            {
                retVal  = _issueRepository.WorkDone(pIssueID, pHours);

                if (retVal == 1)
                {
                    _sprintRepository.SaveDailyEffort(_sprintRepository.GetSprintID(pIssueID), pHours);
                }

                return retVal;
            }

            else
            {
                return -2;
            }
        }

        [Authorize]
        public double GetManHours(long pIssueID)
        {
            return _issueRepository.GetIssueManHours(pIssueID);
        }

        [Authorize(Roles = "3")]
        public ActionResult SearchIssue()
        {
            // Creating field types' list.
            var attributes = _issueRepository.GetIssueAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Issue ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.IssueAttName, Value = "" + item.IssueAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            return View();
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        // To search issues in Project Backlog or Sprint Backlog the following
        // action is being used. So, the pBacklog parameter is for that purpose.
        public ActionResult SearchIssue(FormCollection pFormData, int pBacklog)
        {
            TempData["issueAttID"] = pFormData[0];
            TempData["issueAttVal"] = pFormData[1];

            if(pBacklog == 0)
                return RedirectToAction("MoreActionsTabs", "Project");
            else
                return RedirectToAction("MoreActionsTabs", "Sprint");
        }

        [Authorize(Roles = "3")]
        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Issue ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["issueIDField"] = "Yes";
                IssueAttribute attribute = new IssueAttribute
                {
                    IssueAttID = 0,
                    CanNull = false,
                    DefaultValue = "",
                    FieldType = 1,
                    IssueAttName = "Issue ID"
                };

                return View(attribute);
            }

            ViewData["issueIDField"] = null;
            return View(_issueRepository.GetFieldByID(pFieldID));
        }
    }
}


 