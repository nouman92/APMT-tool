<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Project    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#projectTabs").tabs({ selected: parseInt($("#projectTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="projectTabNumber" name="projectTabNumber" value='<%: Session["projectTabNumber"] %>' />

    <div id="topNavigation">
        <a title="Create new project" class="topLinkButton" href='<%: Url.Action("CreateProjectTabs", "Project") %>'><span>Create</span></a>
        <span class="topLinkButton2">Project</span>
    </div>
    
    <div id="projectTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("ShowActiveProjects", "Project") %>'><span>Active Projects</span></a></li>
             <li><a href='<%: Url.Action("ShowAllProjects", "Project") %>'><span>Show All</span></a></li>
             <li><a href='<%: Url.Action("SearchProject", "Project") %>'><span>Search</span></a></li>
         </ul>
    </div>

</asp:Content>

