<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Project
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editProject").validate();
            
    </script>

    <h3>Edit Project</h3>
    <br />
    <% using (Html.BeginForm("EditProject", "Project", FormMethod.Post, new { id = "editProject" }))
       { %>
        <table class="bodyTable">
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                       if ((ControlType)item.ProjAttribute.FieldType != ControlType.CheckBox)
                       {%>
                           <b> <%: item.ProjAttribute.ProjAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.ProjAttID,
                                                    // If the type is List then pick the list options from the ProjAttributes.
                                                    DefaultValue = ((ControlType)item.ProjAttribute.FieldType == ControlType.List)? item.ProjAttribute.DefaultValue : item.Value,
                                                    CanNull = item.ProjAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.ProjAttribute.RegularExpression != null) ? item.ProjAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.ProjAttribute.RegularExpression != null) ? item.ProjAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.ProjAttribute.FieldType,
                                                    Value = (TempData["__" + item.ProjAttID] != null) ? TempData["__" + item.ProjAttID].ToString() : item.Value,
                                                    ControlAttName = item.ProjAttribute.ProjAttName,
                                                    RegExpressionID = (item.ProjAttribute.RegularExpression != null) ? item.ProjAttribute.RegularExpression1.ExpressionID : -1
                                                }, true
                                             )
                    %>
                    <% // Display the error message if any. 
                       if (TempData["_" + item.ProjAttID] != null)
                       { %>
                           <span class="errorMessage"> <%: TempData["_" + item.ProjAttID]%> </span>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
    <br />
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>

