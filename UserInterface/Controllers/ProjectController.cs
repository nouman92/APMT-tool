using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.HtmlHelpers;
using UserInterface.Models;
using System.Text;
using Domain.Entities;

namespace UserInterface.Controllers
{
    [Authorize(Roles = "3")]
    public class ProjectController : BaseController
    {
        private IProjectRepository _projRepository;
        private IIssueRepository _issueRepository;
        private IOrganizationRepository _orgRepository;

        public ProjectController(IProjectRepository pProjRepository, IIssueRepository pIssueRepository, IOrganizationRepository pOrgRepository)
        {
            _projRepository = pProjRepository;
            _issueRepository = pIssueRepository;
            _orgRepository = pOrgRepository;
        }

        public ActionResult ProjectTabs(int pTabNumber = -1)
        {
            if (pTabNumber != -1)
            {
                Session["projectTabNumber"] = pTabNumber;
            }

            // The "DisplayProject" view is being used in different places
            // like in search, showall etc. It has "Edit" button that should
            // be shown only when it is called in "MoreActionsTabs".
            Session["showPEditButton"] = "";
            return View();
        }

        public ActionResult CreateProjectTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["createProjectTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        // The default values will help when the data is saved and no need to pass again. Specially 
        // in redirection.
        public ActionResult MoreActionsTabs(long pProjectID = 0, string pProjectName = "", int pIsFilterCall = 0)
        {
            if (pIsFilterCall != 0)
            {
                Session["selectedMenu"] = "Project";
                Session["projMoreActionsTabNumber"] = 0;
            }

            if (pProjectID != 0)
            {
                // Saving the selected project name and id for further use.
                base.SelectedProjectName = pProjectName;
                base.SelectedProjectID = pProjectID;

                // In the "DisplayProject" view, there is an "Edit" button and that should be
                // shown to user only if he/she is in "MoreActionTabs". Actually, this view
                // is called from some other places where the edit button is not required like
                // in search (same in other views).
                Session["showPEditButton"] = "Yes";

                // Resetting some other values.
                Session["riskFilterValue"] = null; // Please see the FilterRiskList(ProjectController) action.
                Session["filterPriority"] = null;  // Please see the FilterIssueList(ProjectController) action.
                Session["filterState"] = null;
                Session["filterType"] = null;
                Session["filterAState"] = null;
                Session["selectedMenu"] = "Project";
            }

            return View();
        }

        public ActionResult ShowActiveProjects()
        {
            Session["showPEditButton"] = "";
            Session["projectTabNumber"] = 0;
            return View(_projRepository.GetProjects(true));
        }

        public ActionResult ShowAllProjects()
        {
            Session["showPEditButton"] = "";
            Session["projectTabNumber"] = 1;
            return View(_projRepository.GetProjects(false));
        }

        public ActionResult DisplayProject(long pProjectID)
        {
            Session["projMoreActionsTabNumber"] = 0;
            ViewData["isFavorite"] = (_projRepository.IsFavoriteProject(pProjectID, base.UserID))? "Yes": null;

            return View(_projRepository.GetProjectByID(pProjectID));
        }

        public ActionResult CreateProject()
        {
            ClearLists(1);
            return View(_projRepository.GetProjectAttributes(CustomControlsInfo.CustomControlsID));
        }

        [HttpPost]
        public ActionResult CreateProject(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            List<ProjectRisk> riskAssessmentData = new List<ProjectRisk>();

            if (base.IsValidData)
            {
                // Collecting the Risk Assessment data.
                foreach (var item in _orgRepository.RiskList())
                {
                    if (pFormData["risk_" + item.RiskID] != "false")
                    {
                        riskAssessmentData.Add(new ProjectRisk
                                        {
                                            RiskID = item.RiskID,
                                            Probability = short.Parse(pFormData["prob_" + item.RiskID]),
                                            Impact = short.Parse(pFormData["imp_" + item.RiskID]),
                                            Mitigation = pFormData["mit_" + item.RiskID]
                                        }
                                    );
                    }
                }

                if (_projRepository.CreateProject(data, RenderedControlsInfo.L1ControlsID.ToArray(), riskAssessmentData))
                {
                    base.ShowMessage("Project created successfully.", false);
                }

                else
                {
                    base.ShowMessage("Project did not create successfully.", true);
                }

                ClearLists(1);
                CustomControlsInfo.CustomControlsID.Clear();
                return RedirectToAction("ProjectTabs", new { pTabNumber = 1 });
            }
            else
            {
                ClearLists(1);
                TempData["createProjectTabNumber"] = 0;
                return RedirectToAction("CreateProjectTabs");
            }
        }

        public ActionResult EditProject()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_projRepository.GetProjectByID(base.SelectedProjectID));
        }

        [HttpPost]
        public ActionResult EditProject(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_projRepository.EditProject(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedProjectID))
                {
                    base.ShowMessage("Project details updated successfully.", false);

                    // Updating the saved name.
                    base.SelectedProjectName = _projRepository.GetProjectName(base.SelectedProjectID);
                }

                else
                {
                    base.ShowMessage("Project details did not update successfully.", true);
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
                return RedirectToAction("EditProject");
            }
        }

        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_projRepository.GetProjectAttributes(true));
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
            TempData["createProjectTabNumber"] = 0;
            return RedirectToAction("CreateProjectTabs");
        }

        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_projRepository.GetFieldTypes().Count()];

            foreach (var item in _projRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_projRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _projRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
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

                int result = _projRepository.CreateCustomField(data);

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
                TempData["createProjectTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["createProjectTabNumber"] = 2;
            }

            return RedirectToAction("CreateProjectTabs");
        }

        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _projRepository.IsFieldExists(pFieldName, pFieldID);
        }

        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["createProjectTabNumber"] = 1;
            return View(_projRepository.GetFieldByID(pFieldID));
        }

        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["createProjectTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(2);
            return View(_projRepository.GetFieldByID(pFieldID));
        }

        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);

            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _projRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

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
                TempData["createProjectTabNumber"] = 1;
                return RedirectToAction("CreateProjectTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        public ActionResult SearchProject()
        {
            // Creating field types' list.
            var attributes = _projRepository.GetProjectAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Project ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.ProjAttName, Value = "" + item.ProjAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            Session["projectTabNumber"] = 2;
            Session["showPEditButton"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult SearchProject(int pFieldID, string pSearchValue)
        {
            return View("SearchResult", _projRepository.SearchProject(pFieldID, pSearchValue));
        }

        // Project search can be on any attribute. The user will select the attribute from
        // the list and this action will render the field according to attribute field type.
        // e.g. if the attribute is of type "Checkbox" then this action will render a checkbox
        // and user can search checked or unchecked project on this attribute. Please run the
        // demo for further understanding.
        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Project ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["projectIDField"] = "Yes";
                ProjAttribute attribute =  new ProjAttribute
                                                {
                                                    ProjAttID = 0,
                                                    CanNull = false,
                                                    DefaultValue = "",
                                                    FieldType = 1,
                                                    ProjAttName = "Project ID"
                                                };
                return View(attribute);
            }

            ViewData["projectIDField"] = null;
            return View(_projRepository.GetFieldByID(pFieldID));
        }

        public ActionResult ProjectBacklog()
        {
            List<long> issueIDs;
            bool flag = true;
            int priority = 0, state = 0, type = 0, aState = 0;

            if (TempData["issueAttID"] == null)
            {
                issueIDs = _projRepository.ProjectBacklog(base.SelectedProjectID);
            }

            // If user has searched for the issues, then go for that (TempData has the specified search info).
            else
            {
                issueIDs = _issueRepository.SearchProjBacklogIssue(int.Parse(TempData["issueAttID"].ToString()), TempData["issueAttVal"].ToString(), base.SelectedProjectID);
                ViewData["filtered"] = "Yes";
                flag = false;
            }

            // If user specifed filter options then use the options to filter the list.
            if (Session["filterPriority"] != null && flag)
            {
                priority = int.Parse(Session["filterPriority"].ToString());
                state = int.Parse(Session["filterState"].ToString());
                type = int.Parse(Session["filterType"].ToString());
                aState = int.Parse(Session["filterAState"].ToString());

                // For notification purpose (i.e. if no match is found).
                ViewData["filtered"] = "Yes";
            }

            Session["projMoreActionsTabNumber"] = 1;
            Session["showIEditButton"] = "";
            return View(_issueRepository.GetIssues(issueIDs, state, type, priority, aState));
        }

        public ActionResult FilterIssueList(FormCollection pFormData)
        {
            // Saving the use selection.
            Session["filterPriority"] = pFormData[0];
            Session["filterState"] = pFormData[1];
            Session["filterType"] = pFormData[2];
            Session["filterAState"] = pFormData[3];

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult ProjectRisks()
        {
            short riskExposure = 0;

            // If user specifed filter value (i.e. Risk Exposure) then use the option to filter the list.
            if (Session["riskFilterValue"] != null)
            {
                riskExposure = short.Parse(Session["riskFilterValue"].ToString());

                // For notification purpose (i.e. if no match is found).
                ViewData["filtered"] = "Yes";
            }

            Session["projMoreActionsTabNumber"] = 2;
            return View(_projRepository.GetProjectRisks(base.SelectedProjectID, riskExposure));
        }

        public ActionResult FilterRiskList(FormCollection pFormData)
        {
            if (base.VerifyInput(pFormData[0], @"^[1-9]{1}$|^[1-9]{1}[0-9]{1}$|^100$"))
            {
                Session["riskFilterValue"] = pFormData[0];
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult DisplayProjectRisk(int pRiskID)
        {
            return View(_projRepository.GetProjectRiskByID(base.SelectedProjectID, pRiskID));
        }

        public ActionResult EditProjectRisk(int pRiskID)
        {
            // Clearing the lists.
            ClearLists(1);
            var risk = _projRepository.GetProjectRiskByID(base.SelectedProjectID, pRiskID);

            ViewData["currentProbValue"] = risk.Probability;
            ViewData["currentImpValue"] = risk.Impact;

            return View(risk);
        }

        [HttpPost]
        public ActionResult EditProjectRisk(FormCollection pFormData, int pRiskID)
        {
            ProjectRisk risk = new ProjectRisk
                                    {
                                        ProjID = base.SelectedProjectID,
                                        RiskID = pRiskID,
                                        Probability = short.Parse(pFormData[0]),
                                        Impact = short.Parse(pFormData[1]),
                                        Mitigation = pFormData[2]
                                    };

            if (_projRepository.EditProjectRisk(risk))
            {
                base.ShowMessage("Risk details updated successfully.", false);
            }

            else
            {
                base.ShowMessage("Risk details did not update successfully.", true);
            }

            ClearLists(1);
            return RedirectToAction("MoreActionsTabs");
        }

        public ActionResult AddRisks()
        {
            // Getting the existing project's risks to filter out the risk list.
            var projectRisks = _projRepository.GetProjectRisks(base.SelectedProjectID, 0).Select( x => x.RiskID);

            return View(_orgRepository.RiskList(projectRisks.ToArray()));
        }

        public ActionResult NewRisk()
        {
            int index = 0;

            // Creating risk category list.
            SelectListItem[] categories = new SelectListItem[_orgRepository.RiskCategories().Count()];

            foreach (var item in _orgRepository.RiskCategories())
            {
                categories[index++] = new SelectListItem { Text = "" + item.CategoryName, Value = "" + item.CategoryID };
            }

            ViewData["categories"] = categories;

            // Clearing the lists.
            ClearLists(1);
            return View();
        }

        [HttpPost]
        public ActionResult NewRisk(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_orgRepository.AddRisk(data[0], int.Parse(pFormData[1]), false))
                {
                    ProjectRisk risk = new ProjectRisk
                                            {
                                                RiskID = _orgRepository.GetNewlyAddedRiskID(),
                                                Probability = 1,
                                                Impact = 1,
                                                Mitigation = ""
                                            };

                    if (_projRepository.AddRisk(base.SelectedProjectID, risk))
                    {
                        base.ShowMessage("Risk added successfully.", false);
                    }
                }

                else
                {
                    base.ShowMessage("Risk did not add successfully.", true);
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
                return RedirectToAction("NewRisk");
            }
        }

        [HttpPost]
        public ActionResult AddRisks(FormCollection pFormData)
        {
            List<ProjectRisk> riskAssessmentData = new List<ProjectRisk>();

            // Getting the existing project's risks to filter out the risk list.
            var projectRisks = _projRepository.GetProjectRisks(base.SelectedProjectID, 0).Select(x => x.RiskID);

            foreach (var item in _orgRepository.RiskList(projectRisks.ToArray()))
            {
                if (pFormData["risk_" + item.RiskID] != "false")
                {
                    riskAssessmentData.Add(new ProjectRisk
                    {
                        RiskID = item.RiskID,
                        Probability = short.Parse(pFormData["prob_" + item.RiskID]),
                        Impact = short.Parse(pFormData["imp_" + item.RiskID]),
                        Mitigation = pFormData["mit_" + item.RiskID]
                    }
                                );
                }
            }

            if (_projRepository.AddRisks(base.SelectedProjectID, riskAssessmentData))
            {
                base.ShowMessage("Risk(s) added successfully.", false);
            }

            else
            {
                base.ShowMessage("Risk(s) did not add successfully.", true);
            }

            return RedirectToAction("MoreActionsTabs");
        }

        public JsonResult AddtoFavorite()
        {
            return Json((_projRepository.AddToFavorite(base.SelectedProjectID, base.UserID))? 'A': 'N', JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromFavorite()
        {
            return Json((_projRepository.RemoveFromFavorite(base.SelectedProjectID, base.UserID))? 'R': 'N', JsonRequestBehavior.AllowGet);
        }
    }
}
