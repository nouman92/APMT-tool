<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserInterface.Models.ControlInfo>" %>
<%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = Model.ControlID,
                                    CanNull = Model.CanNull,
                                    DefaultValue = Model.DefaultValue,
                                    // This field can be null. So, if field is null then don't pick the value of expression
                                    // from Regular Expression table.
                                    RegularExpression = null,//Model.RegularExpression,
                                    ErrorMessage = "",//Model.ErrorMessage,
                                    Type = Model.Type,
                                    ControlAttName = Model.ControlAttName,
                                    RegExpressionID = -1//Model.RegExpressionID
                                }, false
                          )
%>
<span class="errorMessage" id="__<%:Model.ControlID%>"></span>
<%: Html.Hidden("RegularExpression", Model.RegularExpression == null ? "" : Model.RegularExpression, new { id = "RegExp" + Model.ControlID })%>
<%: Html.Hidden("ErrorMessage", Model.ErrorMessage, new { id = "ErrMsg" + Model.ControlID })%>
