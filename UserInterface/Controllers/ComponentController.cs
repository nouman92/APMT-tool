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
    [Authorize (Roles="3")]
    public class ComponentController : BaseController
    {
        private IComponentRepository _compRepository;
        private IProjectRepository _projRepository;

        public ComponentController(IComponentRepository pCompRepository, IProjectRepository pProjRepository)
        {
            _projRepository = pProjRepository;
            _compRepository = pCompRepository;
        }

        public ActionResult CreateComponentTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["createCompTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        // The default values will help when the data is saved and no need to pass again. Specially
        // in redirection.
        public ActionResult MoreActionsTabs(long pProjectID = 0, long pComponentID = 0, string pComponentName = "")
        {
            // For redirection from SearchFilter to Component
            if (pProjectID != 0 && pComponentName == "")
            {
                base.SelectedProjectName = _projRepository.GetProjectName(pProjectID);
                base.SelectedProjectID = pProjectID;
                Session["compMoreActionsTabNumber"] = 1;
                pComponentName = _compRepository.GetComponentName(pComponentID);
            }

            else if (pProjectID != 0)
            {
                base.SelectedProjectName = _projRepository.GetProjectName(pProjectID);
                base.SelectedProjectID = pProjectID;
                Session["selectedMenu"] = "Project";
                Session["compMoreActionsTabNumber"] = 0;
            }

            if (pComponentID != 0)
            {
                // Saving the selected project name and id for further use.
                base.SelectedComponentName = pComponentName;
                base.SelectedComponentID = pComponentID;

                Session["showCEditButton"] = "Yes";
            }

            return View();
        }

        public ActionResult ProjectComponents()
        {
            Session["projMoreActionsTabNumber"] = 3;
            Session["showCEditButton"] = "";

            if (TempData["compAttID"] == null)
            {
                return View(_compRepository.GetProjectComponents(base.SelectedProjectID));
            }

            else
            {
                return View(_compRepository.SearchComponent(int.Parse(TempData["compAttID"].ToString()), TempData["compAttVal"].ToString(), base.SelectedProjectID));
            }
        }

        public ActionResult CreateComponent()
        {
            ClearLists(1);
            return View(_compRepository.GetComponentAttributes(CustomControlsInfo.CustomControlsID));
        }

        [HttpPost]
        public ActionResult CreateComponent(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                if (_compRepository.CreateComponent(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedProjectID))
                {
                    base.ShowMessage("Component created successfully.", false);
                }

                else
                {
                    base.ShowMessage("Component did not create successfully.", true);
                }

                ClearLists(1);
                CustomControlsInfo.CustomControlsID.Clear();
                return RedirectToAction("MoreActionsTabs", "Project");
            }

            else
            {
                ClearLists(1);
                TempData["createCompTabNumber"] = 0;
                return RedirectToAction("CreateComponentTabs");
            }
        }

        public ActionResult EditComponent()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_compRepository.GetComponentByID(base.SelectedComponentID));
        }

        [HttpPost]
        public ActionResult EditComponent(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_compRepository.EditComponent(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedComponentID))
                {
                    base.ShowMessage("Component details updated successfully.", false);

                    // Updating the saved name.
                    base.SelectedComponentName = _compRepository.GetComponentName(base.SelectedComponentID);
                }

                else
                {
                    base.ShowMessage("Component details did not update successfully.", true);
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
                return RedirectToAction("EditComponent");
            }
        }

        public ActionResult DisplayComponent(long pCompID)
        {
            Session["compMoreActionsTabNumber"] = 0;

            return View(_compRepository.GetComponentByID(pCompID));
        }

        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_compRepository.GetComponentAttributes(true));
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
            TempData["createCompTabNumber"] = 0;
            return RedirectToAction("CreateComponentTabs");
        }

        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_compRepository.GetFieldTypes().Count()];

            foreach (var item in _compRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_compRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _compRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
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

                int result = _compRepository.CreateCustomField(data);

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
                TempData["createCompTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["createCompTabNumber"] = 2;
            }

            return RedirectToAction("CreateComponentTabs");
        }

        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _compRepository.IsFieldExists(pFieldName, pFieldID);
        }

        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["createCompTabNumber"] = 1;
            return View(_compRepository.GetFieldByID(pFieldID));
        }

        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["createCompTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(2);
            return View(_compRepository.GetFieldByID(pFieldID));
        }

        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);

            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _compRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

                if (result == 1)
                {
                    base.ShowMessage("Field Details updated successfully.", false);
                }

                else if (result == -1)
                {
                    base.ShowMessage("Field Details did not update successfully.", true);
                }

                else
                {
                    base.ShowMessage("Custom Field '" + data[0] + "' already exists.", true);
                }

                ClearLists(2);
                // Tab number to be opened.
                TempData["createCompTabNumber"] = 1;
                return RedirectToAction("CreateComponentTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        public ActionResult SearchComponent()
        {
            // Creating field types' list.
            var attributes = _compRepository.GetComponentAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Component ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.CompAttName, Value = "" + item.CompAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            return View();
        }

        [HttpPost]
        public ActionResult SearchComponent(FormCollection pFormData)
        {
            TempData["compAttID"] = pFormData[0];
            TempData["compAttVal"] = pFormData[1];

            return RedirectToAction("MoreActionsTabs", "Project");
        }

        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Component ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["compIDField"] = "Yes";
                CompAttribute attribute  = new CompAttribute
                                            {
                                                CompAttID = 0,
                                                CanNull = false,
                                                DefaultValue = "",
                                                FieldType = 1,
                                                CompAttName = "Component ID"
                                            };

                return View(attribute);
            }

            ViewData["compIDField"] = null;
            return View(_compRepository.GetFieldByID(pFieldID));
        }
    }
}
