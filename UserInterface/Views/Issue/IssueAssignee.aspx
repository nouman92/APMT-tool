<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue Assignee
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#removeAssigneeTab').show('blind', 700);
            $("#issueAssigneeTable").tablesorter();
        });

    </script>

    <div id="removeAssigneeTab">
        
        <% if (Model != null)
        {
        %>
            <table id="issueAssigneeTable" class="bodyTable">
                <thead style="cursor:default">
                    <tr>
                        <th>
                            <h3 title="Click to sort the list">Issue has the following <%: Model.Count() %> <%: (Model.Count() > 1)?"assignees":"assignee" %>:</h3>
                        </th>
                    </tr>
                    </thead>
                <tbody class="rowHover">
                <% foreach (var emp in Model)
                    { %>
                    <tr> 
                        <td class="tableColPadding" style="width:80%;">       
                            <%: emp.Value%>, ID: <%: emp.EmpID %>
                        </td>
                        <td style="width:20%;">
                            <%: Html.ActionLink("Remove", "RemoveIssueAssignee", "Issue", new { pEmpID = emp.EmpID }, null)%>
                        </td>
                    </tr>
                <% } %>
                </tbody>
            </table>
        <%}
        else
        {%>
            <h3><i>This issue has no assignee. </i></h3>
        <%}%>
        <br />    
    </div>

</asp:Content>

