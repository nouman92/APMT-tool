<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Create Sprint
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#createSprintTabs").tabs({ selected: parseInt($("#createSprintTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="createSprintTabNumber" name="createSprintTabNumber" value='<%: TempData["createSprintTabNumber"] %>' />

    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <span class="topLinkButton2">Create Sprint</span>
        <a title="Go back to Project More Actions" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>More Actions</span></a>
    </div>

    <div id="createSprintTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("CreateSprint", "Sprint") %>'><span>New Sprint</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "Sprint") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "Sprint") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>
