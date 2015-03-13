using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Repository.Abstract;
using UserInterface.Models;
using Domain.Entities;

namespace UserInterface.Controllers.ControllerFactory
{
    [Authorize(Roles = "3")]
    public class SubComponentController : BaseController
    {
        private ISubComponentRepository _subCompRepository;

        public SubComponentController(ISubComponentRepository pSubCompRepository)
        {
            _subCompRepository = pSubCompRepository;
        }

        public ActionResult ListSubComponents()
        {
            Session["compMoreActionsTabNumber"] = 1;

            if (TempData["subCompAttID"] == null)
            {
                return View(_subCompRepository.GetSubComponents(base.SelectedComponentID));
            }

            else
            {
                return View(_subCompRepository.SearchSubComponent(int.Parse(TempData["subCompAttID"].ToString()), TempData["subCompAttVal"].ToString(), base.SelectedComponentID));
            }
        }

        public ActionResult CreateSubComponentTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["createSubCompTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        public ActionResult CreateSubComponent()
        {
            ClearLists(1);
            return View(_subCompRepository.GetSubComponentAttributes(CustomControlsInfo.CustomControlsID));
        }

        [HttpPost]
        public ActionResult CreateSubComponent(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                if (_subCompRepository.CreateSubComponent(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.SelectedComponentID))
                {
                    base.ShowMessage("Sub-Component created successfully.", false);
                }

                else
                {
                    base.ShowMessage("Sub-Component did not create successfully.", true);
                }

                ClearLists(1);
                CustomControlsInfo.CustomControlsID.Clear();
                return RedirectToAction("MoreActionsTabs", "Component");
            }

            else
            {
                ClearLists(1);
                TempData["createSubCompTabNumber"] = 0;
                return RedirectToAction("CreateSubComponentTabs");
            }
        }

        public ActionResult EditSubComponent(long pSubCompID)
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_subCompRepository.GetSubComponentByID(pSubCompID));
        }

        [HttpPost]
        public ActionResult EditSubComponent(FormCollection pFormData, long pSubCompID)
        {
            string[] data = base.CollectData(pFormData);
            bool flag = false;

            if (base.IsValidData)
            {
                if (_subCompRepository.EditSubComponent(data, RenderedControlsInfo.L1ControlsID.ToArray(), pSubCompID))
                {
                    base.ShowMessage("Sub-Component details updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Sub-Component details did not update successfully.", true);
                }

                flag = true;
            }

            // Clearing the lists.
            ClearLists(1);

            if (flag)
            {
                return RedirectToAction("MoreActionsTabs", "Component");
            }

            else
            {
                return RedirectToAction("EditSubComponent", new { pSubCompID = pSubCompID });
            }
        }

        public ActionResult DisplaySubComponent(long pSubCompID)
        {
            return View(_subCompRepository.GetSubComponentByID(pSubCompID));
        }

        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_subCompRepository.GetSubComponentAttributes(true));
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
            TempData["createSubCompTabNumber"] = 0;
            return RedirectToAction("CreateSubComponentTabs");
        }

        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_subCompRepository.GetFieldTypes().Count()];

            foreach (var item in _subCompRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_subCompRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _subCompRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
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

                int result = _subCompRepository.CreateCustomField(data);

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
                TempData["createSubCompTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["createSubCompTabNumber"] = 2;
            }

            return RedirectToAction("CreateSubComponentTabs");
        }

        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _subCompRepository.IsFieldExists(pFieldName, pFieldID);
        }

        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["createSubCompTabNumber"] = 1;
            return View(_subCompRepository.GetFieldByID(pFieldID));
        }

        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["createSubCompTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(2);
            return View(_subCompRepository.GetFieldByID(pFieldID));
        }

        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);

            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _subCompRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

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
                TempData["createSubCompTabNumber"] = 1;
                return RedirectToAction("CreateSubComponentTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        public ActionResult SearchSubComponent()
        {
            // Creating field types' list.
            var attributes = _subCompRepository.GetSubComponentAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Sub-Component ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.SubCompAttName, Value = "" + item.SubCompAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            return View();
        }

        [HttpPost]
        public ActionResult SearchSubComponent(FormCollection pFormData)
        {
            TempData["subCompAttID"] = pFormData[0];
            TempData["subCompAttVal"] = pFormData[1];

            return RedirectToAction("MoreActionsTabs", "Component");
        }

        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Sub-Component ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["subCompIDField"] = "Yes";
                SubCompAttribute attribute = new SubCompAttribute
                {
                    SubCompAttID = 0,
                    CanNull = false,
                    DefaultValue = "",
                    FieldType = 1,
                    SubCompAttName = "Sub-Component ID"
                };

                return View(attribute);
            }

            ViewData["subCompIDField"] = null;
            return View(_subCompRepository.GetFieldByID(pFieldID));
        }
    }
}
