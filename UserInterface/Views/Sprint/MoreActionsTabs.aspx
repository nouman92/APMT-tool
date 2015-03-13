<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Sprint - More Actions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#sMoreActionsTabs").tabs({ selected: parseInt($("#sprintMoreActionsTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="sprintMoreActionsTabNumber" name="sprintMoreActionsTabNumber" value='<%: Session["sprintMoreActionsTabNumber"] %>' />
    
    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
        <br />
        <br />
        <span title="Selected Sprint" class="selectedItem" style="margin-left:25px;"><%:Session["sprintName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Sprints" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Sprints</span></a>
        <% if (ViewData["valid"].ToString() == "True")
        { %>
            <a title="Conduct daily Scrum Meeting" class="topLinkButton" href='<%: Url.Action("ScrumMeetingTabs", "Sprint") %>'><span>Scrum Meeting</span></a>
        <%} %>
    </div>

    <div id="sMoreActionsTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("DisplaySprint", "Sprint", new { pSprintID = (long)Session["sprintID"] }) %>'><span>Sprint Details</span></a></li>
             <li><a href='<%: Url.Action("SprintBacklog", "Sprint") %>'><span>Sprint Backlog</span></a></li>
             <li><a href='<%: Url.Action("MeetingsList", "Sprint") %>'><span>Scrum Meetings</span></a></li>
             <li><a href='<%: Url.Action("BurndownChart", "Sprint", new { pSprintID = (long)Session["sprintID"] }) %>'><span>Burndown Chart</span></a></li>
         </ul>
    </div>

</asp:Content>
