<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SprintAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["projectName"] %> - Sprints
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#projectSprintsTab').show('blind', 700);
        });

        $(function () {
            $("#projectSprintsTable").tablesorter();
        });

        $("#details5").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Sprint Details',
            show: 'clip'
        });

        $("#burndownChartDialog").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Sprint Burndown Chart',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details5").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("a[name='chart']").click(function () {
            var link = $(this).attr('href');
            $("#burndownChartDialog").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#search2").click(function () {
            var link = $(this).attr('href');
            $("#searchDialog2").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#searchDialog2").dialog({
            autoOpen: false,
            height: 'auto',
            width: 720,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Search Sprint',
            show: 'clip'
        });

    </script>

    <div id="projectSprintsTab">
        
             <% if (Model != null)
               {
             %>
                <Span style="float:right;" title="Search the Sprint">[ <%: Html.ActionLink("Search", "SearchSprint", "Sprint", null, new { id = "search2" })%> ]</Span>
                <table id="projectSprintsTable" class="bodyTable">
                    <thead>
                        <tr>
                            <th>
                            <% if (TempData["sprintAttID"] == null)
                               { %>
                                <h3 title="Click to Sort the List">Project has the following <%: Model.Count() %> sprint(s):</h3>
                            <%}
                               else
                               { %>
                                <h3 title="Click to Sort the List">Following <%: Model.Count()%> record(s) found:</h3>
                            <%} %>
                            </th>
                         </tr>
                     </thead>
                     <tbody class="rowHover">
                        <% foreach (var sprint in Model)
                           { %>
                        <tr> 
                            <td class="tableColPadding" style="width:75%;">
                                <%: sprint.Value%>
                            </td>
                            <td style="width:25%;">
                                <%: Html.ActionLink("Details", "DisplaySprint", "Sprint", new { pSprintID = sprint.SprintID }, new { name = "dAjaxLink" })%> |
                                <%: Html.ActionLink("Burndown Chart", "BurndownChart", "Sprint", new { pSprintID = sprint.SprintID }, new { name = "chart" })%> | 
                                <%: Html.ActionLink("More", "MoreActionsTabs", "Sprint", new { pSprintID = sprint.SprintID, pSprintName = sprint.Value }, null)%>
                            </td>
                        </tr>
                         <% } %>
                        </tbody>
                    </table>
                <%}
                else if (TempData["sprintAttID"] == null)
                {%>
                    <br />    
                    <h3><i>Project has no sprint. </i></h3>
                <%}
                else
                {%>
                    <Span style="float:right;" title="Search the Sprint">[ <%: Html.ActionLink("Search", "SearchSprint", "Sprint", null, new { id = "search2" })%> ]</Span>
                    <br />
                    <h3><i>No sprint is found.</i></h3>
                <%} %>
        <br />    
    </div>

    <div id="details5">
    </div>
    <div id="burndownChartDialog">
    </div>
    <div id="searchDialog2">        
    </div>

</asp:Content>

