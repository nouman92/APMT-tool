using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using UserInterface.Models;

/******************* About this Controller *******************
 *                                                           *
 * This Controller is for Global Access of any property,     *
 * function etc.                                             *
 *                                                           *
 *************************************************************/

namespace UserInterface.Controllers
{
    public class BaseController : Controller
    {
        private bool _isValidData;

        protected bool IsValidData
        {
            get
            {
                return _isValidData;
            }
        }

        protected long UserID
        {
            get
            {
                if (Session["userID"] != null)
                {
                    return (long)Session["userID"];
                }

                return -1;
            }

            set
            {
                if (value == -1)
                {
                    Session.Add("userID", null);
                }

                else
                {
                    Session.Add("userID", value);
                }
            }
        }

        protected string UserName
        {
            get
            {
                if (Session["userName"] != null)
                {
                    return (string)Session["userName"];
                }

                return null;
            }

            set
            {
                Session.Add("userName", value);
            }
        }

        protected string UserDesignation
        {
            get
            {
                if (Session["userDesignation"] != null)
                {
                    return (string)Session["userDesignation"];
                }

                return null;
            }

            set
            {
                Session.Add("userDesignation", value);
            }
        }

        protected bool IsValidUser
        {
            get
            {
                if (Session["userID"] != null)
                {
                    return true;
                }

                return false;
            }
        }

        protected long SelectedProjectID
        {
            get
            {
                if (Session["projectID"] != null)
                {
                    return (long)Session["projectID"];
                }

                return -1;
            }

            set
            {
                Session["projectID"] = value;
            }
        }

        protected string SelectedProjectName
        {
            get
            {
                if (Session["projectName"] != null)
                {
                    return (string)Session["projectName"];
                }

                return null;
            }

            set
            {
                Session["projectName"] = value;
            }
        }

        protected long SelectedComponentID
        {
            get
            {
                if (Session["componentID"] != null)
                {
                    return (long)Session["componentID"];
                }

                return -1;
            }

            set
            {
                Session["componentID"] = value;
            }
        }

        protected string SelectedComponentName
        {
            get
            {
                if (Session["componentName"] != null)
                {
                    return (string)Session["componentName"];
                }

                return null;
            }

            set
            {
                Session["componentName"] = value;
            }
        }

        protected long SelectedIssueID
        {
            get
            {
                if (Session["issueID"] != null)
                {
                    return (long)Session["issueID"];
                }

                return -1;
            }

            set
            {
                Session["issueID"] = value;
            }
        }

        protected string SelectedIssueName
        {
            get
            {
                if (Session["issueName"] != null)
                {
                    return (string)Session["issueName"];
                }

                return null;
            }

            set
            {
                Session["issueName"] = value;
            }
        }

        protected long SelectedSprintID
        {
            get
            {
                if (Session["sprintID"] != null)
                {
                    return (long)Session["sprintID"];
                }

                return -1;
            }

            set
            {
                Session["sprintID"] = value;
            }
        }

        protected string SelectedSprintName
        {
            get
            {
                if (Session["sprintName"] != null)
                {
                    return (string)Session["sprintName"];
                }

                return null;
            }

            set
            {
                Session["sprintName"] = value;
            }
        }

        protected long SelectedEmployeeID
        {
            get
            {
                if (Session["employeeID"] != null)
                {
                    return (long)Session["employeeID"];
                }

                return -1;
            }

            set
            {
                Session["employeeID"] = value;
            }
        }

        protected string SelectedEmployeeName
        {
            get
            {
                if (Session["employeeName"] != null)
                {
                    return (string)Session["employeeName"];
                }

                return null;
            }

            set
            {
                Session["employeeName"] = value;
            }
        }

        protected int SelectedRoleID
        {
            get
            {
                if (Session["roleID"] != null)
                {
                    return (int)Session["roleID"];
                }

                return -1;
            }

            set
            {
                Session["roleID"] = value;
            }
        }

        // This function will collect the data available in pFormData object.
        // It will also verify the input and will add error in TempData if any.
        protected string[] CollectData(FormCollection pFormData, int pListSetID = 1)
        {
            List<int> controlIDs;
            List<bool> canNull;
            List<string> regularExpression;
            List<string> errorMessage;

            if (pListSetID == 1)
            {
                controlIDs = RenderedControlsInfo.L1ControlsID;
                canNull = RenderedControlsInfo.L1ControlsCanNullProperty;
                regularExpression = RenderedControlsInfo.L1ControlsRegExpression;
                errorMessage = RenderedControlsInfo.L1ControlsErrorMessage;
            }

            else
            {
                controlIDs = RenderedControlsInfo.L2ControlsID;
                canNull = RenderedControlsInfo.L2ControlsCanNullProperty;
                regularExpression = RenderedControlsInfo.L2ControlsRegExpression;
                errorMessage = RenderedControlsInfo.L2ControlsErrorMessage;
            }

            string[] data = new string[controlIDs.Count];
            int index = 0;
            _isValidData = true;

            foreach (int id in controlIDs)
            {
                data[index] = pFormData["_" + id];

                if ((data[index] == "") && !(canNull[index]))
                {
                    // Storing message to be displayed
                    TempData["_" + id] = "*This field is required";
                    _isValidData = false;
                }

                else if ((regularExpression[index] != null) && (data[index] != ""))
                {
                    if (!VerifyInput(data[index], regularExpression[index]))
                    {
                        // Storing message to be displayed
                        TempData["_" + id] = errorMessage[index];
                        _isValidData = false;
                    }
                }

                // 1. Saving the value that user entered to display again in case of error(s).
                //    This value will go in "ControlInfo's Value property".
                // 2. There is check while giving the value to TempData that if the value is 
                //    "null" then give it "Empty String". This is tricky :). Actually, this 
                //    condition is required to maintain the "Check Box" state.
                TempData["__" + id] = (data[index] == null) ? "" : data[index];

                index++;
            }

            // If data contains no error then dicards all the values stored in TempData.
            if (_isValidData)
            {
                TempData.Clear();
            }

            return data;
        }

        protected bool VerifyInput(string pInput, string pRegExpression)
        {
            bool result = false;

            if (Regex.IsMatch(pInput, pRegExpression))
            {
                result = true;
            }

            return result;
        }

        // To display the Notification, Warning or Error messages to user.
        protected void ShowMessage(string Message, bool IsErrorMessage)
        {
            if (IsErrorMessage)
            {
                TempData["message"] = "<script> showMessage(\"<span style='color: Red;'>" + Message + "</span>\", true); </script>";
            }

            else
            {
                TempData["message"] = @"<script> showMessage('" + Message + "', false); </script>";
            }
        }

        protected void ClearLists(int pListSetID, bool pBoth = false)
        {
            if (pBoth)
            {
                // Removing the controls' info from the lists' set one.
                RenderedControlsInfo.L1ControlsID.Clear();
                RenderedControlsInfo.L1ControlsErrorMessage.Clear();
                RenderedControlsInfo.L1ControlsRegExpression.Clear();
                RenderedControlsInfo.L1ControlsCanNullProperty.Clear();

                // Removing the controls' info from the lists' set two.
                RenderedControlsInfo.L2ControlsID.Clear();
                RenderedControlsInfo.L2ControlsErrorMessage.Clear();
                RenderedControlsInfo.L2ControlsRegExpression.Clear();
                RenderedControlsInfo.L2ControlsCanNullProperty.Clear();
            }

            else if (pListSetID == 1)
            {
                // Removing the controls' info from the lists' set one.
                RenderedControlsInfo.L1ControlsID.Clear();
                RenderedControlsInfo.L1ControlsErrorMessage.Clear();
                RenderedControlsInfo.L1ControlsRegExpression.Clear();
                RenderedControlsInfo.L1ControlsCanNullProperty.Clear();
            }

            else
            {
                // Removing the controls' info from the lists' set two.
                RenderedControlsInfo.L2ControlsID.Clear();
                RenderedControlsInfo.L2ControlsErrorMessage.Clear();
                RenderedControlsInfo.L2ControlsRegExpression.Clear();
                RenderedControlsInfo.L2ControlsCanNullProperty.Clear();
            }
        }
    }
}