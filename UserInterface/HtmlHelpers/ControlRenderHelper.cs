using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserInterface.Models;
using System.Web.Mvc;
using System.Text;

namespace UserInterface.HtmlHelpers
{
    // This enum defines the control types.
    public enum ControlType
    {
        TextField = 1,
        Password = 2,
        TextArea = 3,
        List = 4,
        CheckBox = 5
    }

    public static class ControlRenderHelper
    {
        public static MvcHtmlString RenderControl(this HtmlHelper html, ControlInfo pControlInfo, bool pSaveInfo, int pListSetID = 1)
        {
            string scriptString = "";
            MvcHtmlString controlString = MvcHtmlString.Create("");
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            TagBuilder tag;

            if (pControlInfo.RegExpressionID == 6)
            {
                // Adding jquery code to show datepicker widget
                scriptString = "<script>$(function() { $('#_" + pControlInfo.ControlID + "').datepicker({ dateFormat: 'dd MM yy' });});</script>";
            }

            switch (pControlInfo.Type)
            {
                case ControlType.TextField:

                    tag = new TagBuilder("input");

                    // Tag attributes.
                    attributes.Add("type", "text");
                    attributes.Add("id", "_" + pControlInfo.ControlID);
                    attributes.Add("name", "_" + pControlInfo.ControlID);
                    
                    // For client side validation (using Jquery).
                    if (!pControlInfo.CanNull)
                    {
                        attributes.Add("class", "required");
                    }
                    
                    if (pControlInfo.Value != null)
                    {
                        attributes.Add("value", pControlInfo.Value);
                    }

                    else
                    {
                        attributes.Add("value", pControlInfo.DefaultValue);
                    }

                    tag.MergeAttributes(attributes);

                    // For client side validation.
                    scriptString += GetValidationScript(pControlInfo.ControlID, pControlInfo.RegularExpression, pControlInfo.ErrorMessage);
                    controlString = MvcHtmlString.Create(tag.ToString() + scriptString);

                    break;

                case ControlType.CheckBox:

                    tag = new TagBuilder("input");

                    // Tag attributes.
                    attributes.Add("type", "checkbox");
                    attributes.Add("id", "_" + pControlInfo.ControlID);
                    attributes.Add("name", "_" + pControlInfo.ControlID);
                    attributes.Add("value", "Yes");
                    if (pControlInfo.Value == "Yes")
                    {
                        attributes.Add("checked", "yes");
                    }

                    tag.MergeAttributes(attributes);
                    tag.InnerHtml = "  " + pControlInfo.ControlAttName;
                    controlString = MvcHtmlString.Create(tag.ToString());

                    break;

                case ControlType.TextArea:

                    tag = new TagBuilder("textarea");

                    // Tag attributes.
                    attributes.Add("rows", "4");
                    attributes.Add("cols", "40");
                    attributes.Add("id", "_" + pControlInfo.ControlID);
                    attributes.Add("name", "_" + pControlInfo.ControlID);

                    // For client side validation (using Jquery).
                    if (!pControlInfo.CanNull)
                    {
                        attributes.Add("class", "required");
                    }

                    tag.MergeAttributes(attributes);

                    if (pControlInfo.Value != null)
                    {
                        tag.InnerHtml = pControlInfo.Value;
                    }

                    else
                    {
                        tag.InnerHtml = pControlInfo.DefaultValue;
                    }

                    scriptString += GetValidationScript(pControlInfo.ControlID, pControlInfo.RegularExpression, pControlInfo.ErrorMessage);
                    controlString = MvcHtmlString.Create(tag.ToString() + scriptString);

                    break;

                case ControlType.List:

                    tag = new TagBuilder("select");
                    string[] options = pControlInfo.DefaultValue.Split(';');
                    string selectedValue = pControlInfo.Value;
                    StringBuilder temp = new StringBuilder();

                    // Tag attributes.
                    attributes.Add("id", "_" + pControlInfo.ControlID);
                    attributes.Add("name", "_" + pControlInfo.ControlID);
                    attributes.Add("class", "selectSize");
                    tag.MergeAttributes(attributes);

                    foreach (string val in options)
                    {
                        TagBuilder optionTag = new TagBuilder("option");
                        optionTag.InnerHtml = val;

                        if (val == selectedValue)
                        {
                            optionTag.MergeAttribute("selected", "selected");
                        }

                        temp.Append(optionTag.ToString());
                    }

                    tag.InnerHtml = temp.ToString();
                    controlString = MvcHtmlString.Create(tag.ToString());

                    break;

                case ControlType.Password:

                    tag = new TagBuilder("input");

                    // Tag attributes.
                    attributes.Add("type", "password");
                    attributes.Add("id", "_" + pControlInfo.ControlID);
                    attributes.Add("name", "_" + pControlInfo.ControlID);

                    // For client side validation (using Jquery).
                    if (!pControlInfo.CanNull)
                    {
                        attributes.Add("class", "required");
                    }

                    tag.MergeAttributes(attributes);

                    // For client side validation.
                    scriptString = GetValidationScript(pControlInfo.ControlID, pControlInfo.RegularExpression, pControlInfo.ErrorMessage);
                    controlString = MvcHtmlString.Create(tag.ToString() + scriptString);

                    break;
            }

            if (pSaveInfo)
            {
                if (pListSetID == 1)
                {
                    // Please see the RenderedControlsInfo class to understand the purpose of following code snippet.
                    RenderedControlsInfo.L1ControlsID.Add(pControlInfo.ControlID);
                    RenderedControlsInfo.L1ControlsRegExpression.Add(pControlInfo.RegularExpression);
                    RenderedControlsInfo.L1ControlsCanNullProperty.Add(pControlInfo.CanNull);
                    RenderedControlsInfo.L1ControlsErrorMessage.Add(pControlInfo.ErrorMessage);
                }

                else
                {
                    RenderedControlsInfo.L2ControlsID.Add(pControlInfo.ControlID);
                    RenderedControlsInfo.L2ControlsRegExpression.Add(pControlInfo.RegularExpression);
                    RenderedControlsInfo.L2ControlsCanNullProperty.Add(pControlInfo.CanNull);
                    RenderedControlsInfo.L2ControlsErrorMessage.Add(pControlInfo.ErrorMessage);
                }
            }

            return controlString;
        }

        static string GetValidationScript(long pControlID, string pRegExp, string pErrorMessage)
        {
            string validationScript = null;

            if (pRegExp != null)
            {
                pRegExp = pRegExp.Replace("/", @"\/");
                // For client side validation.
                validationScript = "<script> $(function() {$.validator.addMethod('_" + pControlID + "', function(value, element) {" +
                                        "return this.optional(element) || /" + pRegExp + "/i.test(value); }, '" + pErrorMessage + "');$('#_" +
                                         pControlID + "').rules('add', '_" + pControlID + "');});</script>";

            }

            return validationScript;
        }
    }
}