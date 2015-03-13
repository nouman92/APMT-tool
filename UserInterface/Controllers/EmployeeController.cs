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
    public class EmployeeController : BaseController
    {
        private IEmployeeRepository _empRepository;

        public EmployeeController(IEmployeeRepository pempRepository)
        {
            _empRepository = pempRepository;
        }

        [Authorize(Roles = "2")]
        public ActionResult EmployeeTabs(int pTabNumber = -1)
        {
            if (pTabNumber != -1)
            {
                Session["employeeTabNumber"] = pTabNumber;
            }

            Session["showEEditButton"] = "";
            return View();
        }

        [Authorize(Roles = "2")]
        public ActionResult ShowAllEmployees()
        {
            Session["showEEditButton"] = "";
            Session["employeeTabNumber"] = 0;
            return View(_empRepository.GetEmployees());
        }

        [Authorize(Roles = "2")]
        public ActionResult DisplayEmployee(long pEmployeeID)
        {
            return View(_empRepository.GetEmployeeByID(pEmployeeID, false));
        }

        [Authorize(Roles = "2")]
        public ActionResult MoreActionsTabs(long pEmployeeID = 0, string pEmployeeName = "")
        {
            if (pEmployeeID != 0)
            {
                // Saving the selected employee name and id for further use.
                base.SelectedEmployeeName = pEmployeeName;
                base.SelectedEmployeeID = pEmployeeID;

                // In the "DisplayEmployee" view, there is an "Edit" button and that should be
                // shown to user only if he/she is in "MoreActionTabs". Actually, this view
                // is called from some other places where the edit button is not required like
                // in search.
                Session["selectedMenu"] = "Employee";
                Session["showEEditButton"] = "Yes";
            }

            Session["empMoreActionsTabNumber"] = 0;
            return View();
        }

        [Authorize(Roles = "2")]
        public ActionResult EditEmployee()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_empRepository.GetEmployeeByID(base.SelectedEmployeeID, true));
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult EditEmployee(FormCollection pFormData, long pEmployeeID)
        {
            string[] data = base.CollectData(pFormData);
            if (base.IsValidData)
            {
                if (_empRepository.EditEmployee(data, RenderedControlsInfo.L1ControlsID.ToArray(), pEmployeeID))
                {
                    base.ShowMessage("User information updated successfully.", false);

                    // Updating the saved name.
                    base.SelectedEmployeeName = _empRepository.GetEmployeeName(base.SelectedEmployeeID);
                }

                else
                    base.ShowMessage("User information did not update successfully.", true);
                
                ClearLists(1);
                return RedirectToAction("MoreActionsTabs", new { pEmployeeID = base.SelectedEmployeeID, pEmployeeName = base.SelectedEmployeeName });
            }

            else
            {
                return RedirectToAction("EditEmployee");
            }
        }

        [Authorize(Roles = "2")]
        public ActionResult AddEmployeeTabs()
        {
            // If, it is new request then clear the previous list.
            if (TempData["addEmployeeTabNumber"] == null)
            {
                // Clearing the list.
                CustomControlsInfo.CustomControlsID.Clear();
            }

            return View();
        }

        [Authorize(Roles = "2")]
        public ActionResult AddEmployee()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_empRepository.GetEmployeeAttributes(CustomControlsInfo.CustomControlsID));
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult AddEmployee(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (base.IsValidData)
            {
                if (_empRepository.AddEmployee(data, RenderedControlsInfo.L1ControlsID.ToArray()))
                {
                    base.ShowMessage("User added successfully.", false);
                }

                else
                {
                    base.ShowMessage("User did not add successfully.", true);
                }

                ClearLists(1);
                CustomControlsInfo.CustomControlsID.Clear();
                return RedirectToAction("EmployeeTabs", new { pTabNumber = 0 });
            }

            else
            {
                ClearLists(1);
                TempData["addEmployeeTabNumber"] = 0;
                return RedirectToAction("AddEmployeeTabs");
            }
        }

        [Authorize(Roles = "2")]
        public bool IsUserNameExist(string pUserName)
        {
            return _empRepository.IsUserNameExist(pUserName);
        }

        [Authorize(Roles = "2")]
        public ActionResult ResetUserPassword()
        {
            // Return tab number,
            Session["empMoreActionsTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(1);
            return View();
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult ResetUserPassword(FormCollection pFormData)
        {
            string[] data = new string[2];

            // Collecting and verifying the "New Password".
            data[0] = (base.CollectData(pFormData))[0];

            if (base.IsValidData)
            {
                if (_empRepository.ChangePassword(base.UserID, data[0]))
                {
                    base.ShowMessage("Password changed successfully.", false);
                }

                else
                {
                    base.ShowMessage("Password did not change sccessfully.", true);
                }
            }

            ClearLists(1);
            return RedirectToAction("MoreActionsTabs", new { pEmployeeID = base.SelectedEmployeeID, pEmployeeName = base.SelectedEmployeeName });
        }

        [Authorize(Roles = "2")]
        public ActionResult SearchEmployee()
        {
            // Creating field types' list.
            var attributes = _empRepository.GetEmployeeAttributes(false);
            SelectListItem[] searchOptions = new SelectListItem[attributes.Count() + 2];
            int index = 0;

            searchOptions[index++] = new SelectListItem { Text = " --Select Option--", Value = "-1" };
            searchOptions[index++] = new SelectListItem { Text = " Employee ID", Value = "0" };

            foreach (var item in attributes)
            {
                searchOptions[index++] = new SelectListItem { Text = "" + item.EmpAttName, Value = "" + item.EmpAttID };
            }

            ViewData["searchOptions"] = searchOptions;
            Session["EmployeeTabNumber"] = 1;
            Session["showEEditButton"] = "";
            return View();
        }

        [Authorize(Roles = "2")]
        public ActionResult SearchField(int pFieldID)
        {
            // If pFieldID = 0 then it means search is by Employee ID.
            // So, returning the text field info.
            if (pFieldID == 0)
            {
                ViewData["employeeIDField"] = "Yes";
                EmpAttribute attribute = new EmpAttribute
                {
                    EmpAttID = 0,
                    CanNull = false,
                    DefaultValue = "",
                    FieldType = 1,
                    EmpAttName = "Employee ID"
                };
                return View(attribute);
            }

            ViewData["employeeIDField"] = null;
            return View(_empRepository.GetFieldByID(pFieldID));
        }

        // Employee search can be on any attribute. The user will select the attribute from
        // the list and this action will render the field according to attribute field type.
        // e.g. if the attribute is of type "Checkbox" then this action will render a checkbox
        // and user can search checked or unchecked Employee on this attribute. Please run the
        // demo for further understanding.
        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult SearchEmployee(int pFieldID, string pSearchValue)
        {
            return View("SearchResult", _empRepository.SearchEmployee(pFieldID, pSearchValue));
        }

        [Authorize(Roles = "2")]
        public ActionResult CustomFields()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_empRepository.GetEmployeeAttributes(true));
        }

        [Authorize(Roles = "2")]
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
            TempData["addEmployeeTabNumber"] = 0;
            return RedirectToAction("AddEmployeeTabs");
        }

        [Authorize(Roles = "2")]
        public ActionResult CreateCustomField()
        {
            int index = 0;

            // Creating field types' list.
            SelectListItem[] fieldTypes = new SelectListItem[_empRepository.GetFieldTypes().Count()];

            foreach (var item in _empRepository.GetFieldTypes())
            {
                fieldTypes[index++] = new SelectListItem { Text = "" + item.FieldName, Value = "" + item.FieldID };
            }

            ViewData["fieldTypes"] = fieldTypes;

            // Creating Regular Expressions' list.
            SelectListItem[] regularExpressions = new SelectListItem[_empRepository.GetRegularExpressions().Count() + 1];
            index = 0;

            // If user don't want any regular expression then this is the option for that choice.
            regularExpressions[index++] = new SelectListItem { Text = "None", Value = "" };

            foreach (var item in _empRepository.GetRegularExpressions().OrderBy(x => x.ExpressionName))
            {
                regularExpressions[index++] = new SelectListItem { Text = "" + item.ExpressionName, Value = "" + item.ExpressionID };
            }

            ViewData["regularExpressions"] = regularExpressions;

            // True, False option
            ViewData["trueFalseOption"] = new[] { new SelectListItem { Text = "False", Value = "False" },
                                                  new SelectListItem { Text = "True", Value = "True" }
                                                 };

            ClearLists(1);
            return View();
        }

        [Authorize(Roles = "2")]
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

                int result = _empRepository.CreateCustomField(data);

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
                TempData["addEmployeeTabNumber"] = 1;
            }

            else
            {
                // To display the field value in case of error.
                TempData["defaultValue"] = pFormData[2];
                TempData["addEmployeeTabNumber"] = 2;
            }

            return RedirectToAction("addEmployeeTabs");
        }

        [Authorize(Roles = "2")]
        public bool IsFieldExists(string pFieldName, int pFieldID = 0)
        {
            return _empRepository.IsFieldExist(pFieldName, pFieldID);
        }

        [Authorize(Roles = "2")]
        public ActionResult DisplayFieldInfo(int pFieldID)
        {
            // Return tab number.
            TempData["addEmployeeTabNumber"] = 1;
            return View(_empRepository.GetFieldByID(pFieldID));
        }

        [Authorize(Roles = "3")]
        public JsonResult GetEmployeesNames()
        {
            return Json(_empRepository.EmployeesNames(), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "2")]
        public ActionResult EditFieldInfo(int pFieldID)
        {
            // Return tab number,
            TempData["addEmployeeTabNumber"] = 1;

            // Clearing lists.
            ClearLists(2);
            return View(_empRepository.GetFieldByID(pFieldID));
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult EditFieldInfo(FormCollection pFormData, int pFieldID)
        {
            string[] data = base.CollectData(pFormData, 2);
            Session["FieldId"] = null;
            if (base.IsValidData)
            {
                // If the field is of type list then user might have specified new list option(s).
                int result = _empRepository.EditField(pFieldID, data[0], (data.Count() > 1) ? data[1] : null);

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
                TempData["addEmployeeTabNumber"] = 1;
                return RedirectToAction("addEmployeeTabs");
            }

            else
            {
                return RedirectToAction("EditFieldInfo", new { pFieldID = pFieldID });
            }
        }

        [Authorize(Roles = "6")]
        public ActionResult UserProfileTabs(int pTabNumber = -1)
        {
            if (pTabNumber != -1)
            {
                Session["userAccountTabNumber"] = pTabNumber;
            }
            return View();
        }

        [Authorize(Roles = "6")]
        public ActionResult ViewProfile()
        {
            Session["userAccountTabNumber"] = 0;
            return View(_empRepository.ViewProfile(base.UserID));
        }

        [Authorize(Roles = "6")]
        public ActionResult EditProfile()
        {
            // Clearing the lists.
            ClearLists(1);
            return View(_empRepository.EditProfile(base.UserID));
        }

        [Authorize(Roles = "6")]
        [HttpPost]
        public ActionResult EditProfile(FormCollection pFormData)
        {
            string[] data = base.CollectData(pFormData);

            if (_empRepository.EditEmployee(data, RenderedControlsInfo.L1ControlsID.ToArray(), base.UserID))
            {
                base.ShowMessage("Profile information updated successfully.", false);
                base.UserName = _empRepository.GetEmployeeName(base.UserID);

                ClearLists(1);
                return RedirectToAction("UserProfileTabs", new { pTabNumber = 0 });
            }

            else
            {
                base.ShowMessage("Profile information did not update successfully.", true);
                return View("EditProfile");
            }
        }

        [Authorize(Roles = "6")]
        public ActionResult ChangePassword()
        {
            // Return tab number,
            TempData["userAccountTabNumber"] = 1;

            // Clearing the lists.
            ClearLists(1);
            return View();
        }

        [Authorize(Roles = "6")]
        [HttpPost]
        public ActionResult ChangePassword(FormCollection pFormData)
        {
            string[] data = new string[2];

            // Collecting and verifying the "New Password".
            data[0] = (base.CollectData(pFormData))[0];
            data[1] = (base.CollectData(pFormData))[1];

            if (base.IsValidData)
            {
                if (_empRepository.ChangePassword(base.UserID, data[0], data[1]) == 1)
                {
                    base.ShowMessage("Password changed successfully.", false);
                    ClearLists(1);

                    return RedirectToAction("UserProfileTabs", new { pTabNumber = 0 });
                }

                else if (_empRepository.ChangePassword(base.UserID, data[0], data[1]) == 0)
                {
                    base.ShowMessage("Incorrect old password.", true);
                }

                else
                {
                    base.ShowMessage("Password did not change sccessfully.", true);
                }
            }

            ClearLists(1);
            return RedirectToAction("UserProfileTabs", new { pTabNumber = 1 });
        }

        [Authorize]
        public ActionResult DisplayNote(long pNoteID)
        {
            return View(_empRepository.GetNoteByID(pNoteID));
        }

        [Authorize]
        public ActionResult CreateNote()
        {
            ClearLists(1);
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateNote(FormCollection pFormData)
        {
            string[] data = new string[1];

            // Collecting and verifying the "Subject".
            data[0] = (base.CollectData(pFormData))[0];

            if (base.IsValidData)
            {
                if (_empRepository.CreateNote(data[0], pFormData[1], base.UserID))
                {
                    base.ShowMessage("Note created successfully.", false);
                }

                else
                {
                    base.ShowMessage("Note did not create successfully.", true);
                }
            }

            else
            {
                base.ShowMessage("Invalid subject.", true);
            }

            ClearLists(1);
            return RedirectToAction("HomeTabs", "Home");
        }

        [Authorize]
        public ActionResult EditNote(long pNoteID)
        {
            ClearLists(1);
            return View(_empRepository.GetNoteByID(pNoteID));
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditNote(FormCollection pFormData, long pNoteID)
        {
            string[] data = new string[1];

            // Collecting and verifying the "Subject".
            data[0] = (base.CollectData(pFormData))[0];

            if (base.IsValidData)
            {
                if (_empRepository.EditNote(data[0], pFormData[1], pNoteID))
                {
                    base.ShowMessage("Note updated successfully.", false);
                }

                else
                {
                    base.ShowMessage("Note did not update successfully.", true);
                }
            }

            else
            {
                base.ShowMessage("Invalid subject.", true);
            }

            ClearLists(1);
            return RedirectToAction("HomeTabs", "Home");
        }

        [Authorize]
        public ActionResult DeleteNote(long pNoteID)
        {
            if (_empRepository.DeleteNote(pNoteID))
            {
                base.ShowMessage("Note deleted successfully.", false);
            }

            else
            {
                base.ShowMessage("Note did not delete successfully.", true);
            }

            return RedirectToAction("HomeTabs", "Home");
        }
    }
}