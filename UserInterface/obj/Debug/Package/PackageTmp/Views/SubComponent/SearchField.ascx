<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Domain.Entities.SubCompAttribute>" %>

<% if (ViewData["subCompIDField"] == null)
   {%>
    <%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = Model.SubCompAttID,
                                    CanNull = Model.CanNull,
                                    DefaultValue = ((ControlType)Model.FieldType == ControlType.List) ? Model.DefaultValue : "",
                                    // This field can be null. So, if field is null then don't pick the value of expression
                                    // from Regular Expression table.
                                    RegularExpression = (Model.RegularExpression != null) ? Model.RegularExpression1.Value : null,
                                    ErrorMessage = (Model.RegularExpression != null) ? Model.RegularExpression1.Error : "",
                                    Type = (ControlType)Model.FieldType,
                                    ControlAttName = Model.SubCompAttName,
                                    RegExpressionID = (Model.RegularExpression != null) ? Model.RegularExpression1.ExpressionID : -1
                                }, false
                          )
    %>
<%} else{ %>
    <%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = Model.SubCompAttID,
                                    CanNull = Model.CanNull,
                                    DefaultValue = Model.DefaultValue,
                                    RegularExpression = @"^\d+$",
                                    ErrorMessage = "*Invalid Numeric Value",
                                    Type = (ControlType) Model.FieldType,
                                    ControlAttName = Model.SubCompAttName,
                                    RegExpressionID = -1
                                }, false
                          )
    %>
<%} %>

