<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Domain.Entities.SprintAttribute>" %>

<% if (ViewData["sprintIDField"] == null)
   {%>
    <%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = Model.SprintAttID,
                                    CanNull = Model.CanNull,
                                    DefaultValue = ((ControlType)Model.FieldType == ControlType.List) ? Model.DefaultValue : "",
                                    // This field can be null. So, if field is null then don't pick the value of expression
                                    // from Regular Expression table.
                                    RegularExpression = (Model.RegularExpression != null) ? Model.RegularExpression1.Value : null,
                                    ErrorMessage = (Model.RegularExpression != null) ? Model.RegularExpression1.Error : "",
                                    Type = (ControlType)Model.FieldType,
                                    ControlAttName = Model.SprintAttName,
                                    RegExpressionID = (Model.RegularExpression != null) ? Model.RegularExpression1.ExpressionID : -1
                                }, false
                          )
    %>
<%} else{ %>
    <%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = Model.SprintAttID,
                                    CanNull = Model.CanNull,
                                    DefaultValue = Model.DefaultValue,
                                    RegularExpression = @"^\d+$",
                                    ErrorMessage = "*Invalid Numeric Value",
                                    Type = (ControlType) Model.FieldType,
                                    ControlAttName = Model.SprintAttName,
                                    RegExpressionID = -1
                                }, false
                          )
    %>
<%} %>

