<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SprintAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Sprint
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editSprint").validate();
            
    </script>

    <h3>Edit Sprint</h3>
    <br />
    <% using (Html.BeginForm("EditSprint", "Sprint", FormMethod.Post, new { id = "editSprint" }))
       { %>
        <table class="bodyTable">
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                       if ((ControlType)item.SprintAttribute.FieldType != ControlType.CheckBox)
                       {%>
                           <b> <%: item.SprintAttribute.SprintAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.SprintAttID,
                                                    // If the type is List then pick the list options from the ProjAttributes.
                                                    DefaultValue = ((ControlType)item.SprintAttribute.FieldType == ControlType.List)? item.SprintAttribute.DefaultValue : item.Value,
                                                    CanNull = item.SprintAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.SprintAttribute.RegularExpression != null) ? item.SprintAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.SprintAttribute.RegularExpression != null) ? item.SprintAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.SprintAttribute.FieldType,
                                                    Value = (TempData["__" + item.SprintAttID] != null) ? TempData["__" + item.SprintAttID].ToString() : item.Value,
                                                    ControlAttName = item.SprintAttribute.SprintAttName,
                                                    RegExpressionID = (item.SprintAttribute.RegularExpression != null) ? item.SprintAttribute.RegularExpression1.ExpressionID : -1
                                                }, true
                                             )
                    %>
                    <% // Display the error message if any. 
                       if (TempData["_" + item.SprintAttID] != null)
                       { %>
                           <span class="errorMessage"> <%: TempData["_" + item.SprintAttID]%> </span>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
    <br />
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Sprint") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>
