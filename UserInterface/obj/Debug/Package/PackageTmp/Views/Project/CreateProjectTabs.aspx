<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Create Project
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#createProjectTabs").tabs({ selected: parseInt($("#createProjectTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="createProjectTabNumber" name="createProjectTabNumber" value='<%: TempData["createProjectTabNumber"] %>' />

    <div id="topNavigation">
        <span class="topLinkButton2">Create</span>
        <a title="Go back to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs") %>'><span>Project</span></a>
    </div>

    <div id="createProjectTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("CreateProject", "Project") %>'><span>New Project</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "Project") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "Project") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>
