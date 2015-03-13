<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="titleContent" runat="server">
	Issue Details
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    
    <script type="text/javascript">

        $('#iMoreActionsTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

    </script>

    <div id="issueDetails">

        <div>
            <br />
            <b>&nbsp &nbsp &nbsp &nbsp &nbsp Project: </b>&nbsp <%:ViewData["issueProjectName"]%>
            <% if (ViewData["issueSprintName"] != null)
               {%>
                <br />
                <br />
                <b>&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Sprint: </b>&nbsp <%:ViewData["issueSprintName"]%>
            <%} %>
                <br />
                <br />
                <b>&nbsp &nbsp &nbsp Assignee: </b>
            <% if (ViewData["assignedto"] != "")
               {%>
                    &nbsp <%: Html.DropDownList("assigned", (IEnumerable<SelectListItem>)ViewData["assignedto"])%>
            <%}
               else
               {%>
                    &nbsp Unassigned
            <%} %>
            <br />
            <br />
            <br />
        </div>

        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b> Issue ID</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().IssueID %>        
                </td>
            </tr>
            <% foreach (var item in Model) { %>
            <tr>
                <td class="labelPortion">
                    <b> <%: item.IssueAttribute.IssueAttName %> </b>
                </td>
                <td class="controlPortion">
                    <%: item.Value %>
                </td>
            </tr>    
        <% } %>
            <tr>
                <td class="labelPortion">
                    <b> Issue Type</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().Issue.IssueType.TypeName %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b> Issue Priority</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().Issue.IssuePriority.PriorityName %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b> Issue State</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().Issue.IssueWorkFlow.StateName %>
                </td>
            </tr>
        </table>
        <br />

        <%
            if (Session["showIEditButton"] != "")
          {  
        %>
            <script type="text/javascript">
                $(function () {
                    $('#issueDetails').show('blind', 700);
                });
            </script>
            <a name = "ajaxLink" title="Edit Issue Details" class="linkButton2" href='<%: Url.Action("EditIssue", "Issue") %>'> <span> Edit </span></a>
            <br />
        <%} %>

    </div>

</asp:Content>
