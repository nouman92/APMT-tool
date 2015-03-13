<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Scrum Meeting Tabs
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#scrumMeetingTabs").tabs();
        });

    </script>

    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
        <br />
        <br />
        <span title="Selected Sprint" class="selectedItem" style="margin-left:25px;"><%:Session["sprintName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Sprints" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Sprints</span></a>
        <a title="Go back Sprint More Actions" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Sprint") %>'><span>More Actions</span></a>
        <span class="topLinkButton2">Scrum Meeting</span>
    </div>

    <div id="scrumMeetingTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("ScrumMeeting", "Sprint") %>'><span>Daily Scrum Meeting</span></a></li>
         </ul>
    </div>

</asp:Content>
