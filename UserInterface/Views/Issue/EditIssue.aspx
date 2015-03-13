<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Issue
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editIssue").validate();

        $(function () {
            $.ajax({
                url: '<%: Url.Action("GetEmployeesNames", "Employee")  %>',
                data: Array(),
                success: function (data) {
                    var empList = data;
                    $("#_8").autocomplete({
                        source: empList
                    });
                }
            });
        });

    </script>

    <h3>Edit Issue</h3>
    <br />
    <% using (Html.BeginForm("EditIssue", "Issue", null, FormMethod.Post, new { id = "editIssue" }))
       { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Issue State</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("issueState", (IEnumerable<SelectListItem>)ViewData["states"], new { id = "issueState" })%>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Issue Type</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("issueType", (IEnumerable<SelectListItem>)ViewData["types"], new { id = "issueType" })%>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Issue Priority</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("issuePriority", (IEnumerable<SelectListItem>)ViewData["priorities"], new { id = "issuePriority" })%>
                </td>
            </tr>
        <% foreach (var item in Model)
           { %>
            <tr>
                <td class="labelPortion">
                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                       if ((ControlType)item.IssueAttribute.FieldType != ControlType.CheckBox)
                       {%>
                           <b> <%: item.IssueAttribute.IssueAttName%> </b>
                    <%} %>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = item.IssueAttID,
                                                    // If the type is List then pick the list options from the ProjAttributes.
                                                    DefaultValue = ((ControlType)item.IssueAttribute.FieldType == ControlType.List)? item.IssueAttribute.DefaultValue : item.Value,
                                                    CanNull = item.IssueAttribute.CanNull,
                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                    // from Regular Expression table. 
                                                    RegularExpression = (item.IssueAttribute.RegularExpression != null) ? item.IssueAttribute.RegularExpression1.Value : null,
                                                    ErrorMessage = (item.IssueAttribute.RegularExpression != null) ? item.IssueAttribute.RegularExpression1.Error : "",
                                                    Type = (ControlType)item.IssueAttribute.FieldType,
                                                    Value = (TempData["__" + item.IssueAttID] != null) ? TempData["__" + item.IssueAttID].ToString() : item.Value,
                                                    ControlAttName = item.IssueAttribute.IssueAttName,
                                                    RegExpressionID = (item.IssueAttribute.RegularExpression != null) ? item.IssueAttribute.RegularExpression1.ExpressionID : -1
                                                }, true
                                             )
                    %>
                    <% // Display the error message if any. 
                       if (TempData["_" + item.IssueAttID] != null)
                       { %>
                           <span class="errorMessage"> <%: TempData["_" + item.IssueAttID]%> </span>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
    <br />
    <input class="buttonDesign" type="submit" value="Save" />
    <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Issue") %>'> <span> Cancel </span></a>
    <br />
    <%} %>

</asp:Content>
