<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["projectName"] %> - Backlog
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#projectBacklogTab').show('blind', 700);
        });

        $("#details").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Issue Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details").load(link, function () { $(this).dialog('open');});
            return false;
        });

        $("#filterList").click(function () {
            $("#filterIssueListDialog").dialog('open');
            return false;
        });

        $("#filterIssueListDialog").dialog({
            autoOpen: false,
            height: 'auto',
            width: 520,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Filter Issue List',
            show: 'clip'
        });

        $("#search3").click(function () {
            var link = $(this).attr('href');
            $("#searchDialog3").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#searchDialog3").dialog({
            autoOpen: false,
            height: 'auto',
            width: 720,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Search Issue',
            show: 'clip'
        });

    </script>

    <div id="projectBacklogTab">
        
             <% if (Model != null)
               {
             %>
                <Span style="float:right;" title="Add new issues in backlog">[ <%: Html.ActionLink("Add More", "CreateIssueTabs", "Issue")%> ]</Span>
                <Span style="float:right;" title="Search the issue">[ <%: Html.ActionLink("Search", "SearchIssue", "Issue", null, new { id = "search3" })%> ]&nbsp</Span>
                <Span style="float:right;" title="Filter out the issue list from the whole backlog">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList" })%> ]&nbsp</Span>
                <table id="projectBacklogTable" class="bodyTable">
                    <thead style="cursor:default">
                        <tr>
                            <th>
                            <% if (ViewData["filtered"] == null)
                               { %>
                                <h3 title="Sorted according to Priority">Backlog has the following <%: Model.Count() %> issue(s):</h3>
                            <%}
                               else
                               { %>
                                <h3 title="Sorted according to Priority (filtered list)">Following <%: Model.Count()%> issue(s) found:</h3>
                            <%} %>
                            </th>
                         </tr>
                     </thead>
                     <tbody class="rowHover">
                        <% foreach (var issue in Model)
                           { %>
                        <tr> 
                            <td class="tableColPadding" style="width:80%;">
                                <%: issue.Value%>
                            </td>
                            <td style="width:20%;">
                                <%: Html.ActionLink("Details", "DisplayIssue", "Issue", new { pIssueID = issue.IssueID }, new { name = "dAjaxLink" })%> |
                                <%: Html.ActionLink("More", "MoreActionsTabs", "Issue", new { pIssueID = issue.IssueID, pIssueName = issue.Value }, null)%>
                            </td>
                        </tr>
                         <% } %>
                        </tbody>
                    </table>
                <%}
                else if (ViewData["filtered"] == null)
                {%>
                    <h3><i>Backlog is empty. </i><span style="font-weight:normal; font-size:18px;"> [ <%: Html.ActionLink("Create Now", "CreateIssueTabs", "Issue")%> ]</span></h3>
                <%}
                else
                {%>
                    <Span style="float:right;" title="Add new issues in backlog">[ <%: Html.ActionLink("Add More", "CreateIssueTabs", "Issue")%> ]</Span>
                    <Span style="float:right;" title="Search the Issue">[ <%: Html.ActionLink("Search", "SearchIssue", "Issue", null, new { id = "search3" })%> ]&nbsp</Span>
                    <Span style="float:right;" title="Filter the list on the basis of Issue Priority">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList" })%> ]&nbsp</Span>
                    <br />
                    <h3><i>No issue is found.</i></h3>
                <%} %>
        <br />    
    </div>

    <div id="details">
    </div>

    <div id="filterIssueListDialog">
    
        <% using (Html.BeginForm("FilterIssueList", "Project", FormMethod.Post))
           { %>
            <br />
                <% Html.RenderAction("FilterIssueListOptions", "Issue"); %>
            <br />
            <br />
            <input style="margin-left:40%;" class="buttonDesign" type="submit" value="Filter" id="filterButton" />
            <br />
            <br />
        <% } %>
        
    </div>

    <div id="searchDialog3">        
    </div>

</asp:Content>

