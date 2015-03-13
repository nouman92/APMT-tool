<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	<%: Session["sprintName"]%> - Backlog
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#sprintBacklogTab').show('blind', 700);
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
            $("#details").load(link, function () { $(this).dialog('open'); });
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
            $("#searchDialog3").load(link, function () {
                $("#searchIssue").attr('action', "/Issue/SearchIssue?pBacklog=1");
                $(this).dialog('open');                 
            });
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

        $(function () {
            $("#progressbar2").progressbar({
                value: <%:ViewData["loadPercent"]%>
            });
            $("#progressbar2").css({ 'background': '#c9c9c9' });

            var value = $( "#progressbar2" ).progressbar( "option", "value" );
            if (value < 50)
               $("#progressbar2 > div").css({ 'background': 'Orange' });
            else if (value < 85)
               $("#progressbar2 > div").css({ 'background': 'Yellow' });
            else if (value < 101)
               $("#progressbar2 > div").css({ 'background': 'Green' });
            else 
               $("#progressbar2 > div").css({ 'background': 'Red' });
            
        });

        $("#addIssueDialog").dialog({
            autoOpen: false,
            height: 320,
            width: 750,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Add Issues',
            show: 'clip',
            closeOnEscape: false
        });

        $("#addLink").click(function () {
            var link = $(this).attr('href');
            $("#addIssueDialog").load(link, function () {
                $("#cancel6").click(function () {
                    $("#addIssueDialog").dialog('close');
                });
                $("#showAllLink").click(function () {
                    var link = $(this).attr('href');
                    $("#addIssueDialog").load(link, function () {
                        $("#cancel6").click(function () {
                            $("#addIssueDialog").dialog('close');
                        });
                    });
                    return false;
                });
            }).dialog("open");
            return false;
        });

        $("#assignIssue").dialog({
            autoOpen: false,
            height: 'auto',
            height: 400,
            width: 900,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Assign Issue',
            show: 'clip'
        });

        $("a[name='aAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#assignIssue").load(link, function () {             
                $("#cancel8").click(function () {
                    $("#assignIssue").dialog('close');
                });
                $(this).dialog('open'); 
                });
            return false;
        });

    </script>

    <div id="sprintBacklogTab">
        
        <% if (Model != null)
        {
        %>
        <% if (ViewData["valid"].ToString() == "True")
            { %>
        <Span style="float:right;">[ <%: Html.ActionLink("Add More", "AddIssue", "Sprint", null, new { id = "addLink" })%> ]</Span>
        <%} %>
        <Span style="float:right;" title="Search the issue">[ <%: Html.ActionLink("Search", "SearchIssue", "Issue", null, new { id = "search3" })%> ]&nbsp</Span>
        <Span style="float:right;" title="Filter out the issue list from the whole backlog">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList" })%> ]&nbsp</Span>
        <table>
            <tr>
                <td>
                    <br />
                    <div title='<%:ViewData["load"]%>' id="progressbar2" style="width:170px; height:20px;"></div>
                </td>
                <td>
                    <br />
                    &nbsp <span title="Loaded hour(s)"><%:ViewData["wHours"]%> hr</span> / <span title="Available hour(s)"><%:ViewData["tHours"]%> hr</span>
                </td>
            </tr>
        </table>
        <table id="sprintBacklogTable" class="bodyTable">
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
                <% List<bool> isIssueCompleted = (List<bool>)ViewData["isIssueCompleted"];
                    int index = 0;
                    foreach (var issue in Model)
                    { %>
                <tr> 
                    <td class="tableColPadding" style="width:75%;">
                        <%: issue.Value%>
                    </td>
                    <td style="width:25%;">
                        <%: Html.ActionLink("Details", "DisplayIssue", "Issue", new { pIssueID = issue.IssueID }, new { name = "dAjaxLink" })%>
                        <% if (ViewData["valid"].ToString() == "True" && !(isIssueCompleted[index++]))
                            { %>
                            | <%: Html.ActionLink("Assign", "AssignIssue", "Sprint", new { pIssueID = issue.IssueID }, new { name = "aAjaxLink" })%>
                        <%} %>
                        | <%: Html.ActionLink("Remove", "RemoveSprintIssue", "Sprint", new { pIssueID = issue.IssueID }, null)%>
                    </td>
                </tr>
                    <% } %>
                </tbody>
            </table>
        <%}
        else if (ViewData["filtered"] == null)
        {%>
            <h3><i>Backlog is empty. </i> 
            <% if (ViewData["valid"].ToString() == "True")
                { %>
                <span style="font-weight:normal; font-size:18px;">
                [ <%: Html.ActionLink("Create Now", "AddIssue", "Sprint", null, new { id = "addLink" })%> ]</Span>
                <%} %></h3>
        <%}
        else
        {%>
            <% if (ViewData["valid"].ToString() == "True")
                { %>
            <Span style="float:right;">[ <%: Html.ActionLink("Add More", "AddIssue", "Sprint", null, new { id = "addLink" })%> ]</Span>
            <%} %>
            <Span style="float:right;" title="Search the issue">[ <%: Html.ActionLink("Search", "SearchIssue", "Issue", null, new { id = "search3" })%> ]&nbsp</Span>
            <Span style="float:right;" title="Filter out the issue list from the whole backlog">[ <%: Html.ActionLink("Filter List", "#", null, new { id = "filterList" })%> ]&nbsp</Span>
            <br />
            <h3><i>No issue is found.</i></h3>
        <%} %>
        <br />    
    </div>

    <div id="details">
    </div>

    <div id="filterIssueListDialog">
    
        <% using (Html.BeginForm("FilterIssueList", "Sprint", FormMethod.Post))
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

    <div id="addIssueDialog">       
    </div>

    <div id="assignIssue">       
    </div>

</asp:Content>

