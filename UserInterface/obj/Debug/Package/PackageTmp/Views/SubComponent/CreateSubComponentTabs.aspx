<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Sub-Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#createSubCompTabs").tabs({ selected: parseInt($("#createSubCompTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="createSubCompTabNumber" name="createSubCompTabNumber" value='<%: TempData["createSubCompTabNumber"] %>' />

    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
        <br />
        <br />
        <span title="Selected Component" class="selectedItem" style="margin-left:25px;"><%:Session["componentName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Components" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Components</span></a>
        <a title="Go back to Component More Actions" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Component") %>'><span>More Actions</span></a>
        <span class="topLinkButton2">Create Sub-Component</span>
    </div>

    <div id="createSubCompTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("CreateSubComponent", "SubComponent") %>'><span>New Sub-Component</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "SubComponent") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "SubComponent") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>


