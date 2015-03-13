<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Project - More Actions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#moreActionsTabs").tabs({ selected: parseInt($("#projMoreActionsTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="projMoreActionsTabNumber" name="projMoreActionsTabNumber" value='<%: Session["projMoreActionsTabNumber"] %>' />
    
    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go back to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Create new Component" class="topLinkButton" href='<%: Url.Action("CreateComponentTabs", "Component") %>'><span>Create Component</span></a>
        <a title="Create new Sprint" class="topLinkButton" href='<%: Url.Action("CreateSprintTabs", "Sprint") %>'><span>Create Sprint</span></a>
    </div>
        
    <div id="moreActionsTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("DisplayProject", "Project", new { pProjectID = (long)Session["projectID"] }) %>'><span>Project Details</span></a></li>
             <li><a href='<%: Url.Action("ProjectBacklog", "Project") %>'><span>Project Backlog</span></a></li>
             <li><a href='<%: Url.Action("ProjectRisks", "Project") %>'><span>Project Risks</span></a></li>
             <li><a href='<%: Url.Action("ProjectComponents", "Component") %>'><span>Components</span></a></li>
             <li><a href='<%: Url.Action("ProjectSprints", "Sprint") %>'><span>Sprints</span></a></li>
         </ul>
    </div>

</asp:Content>
