<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#createComponentTabs").tabs({ selected: parseInt($("#createCompTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="createCompTabNumber" name="createCompTabNumber" value='<%: TempData["createCompTabNumber"] %>' />

    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <span class="topLinkButton2">Create Component</span>
        <a title="Go back to Project More Actions" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>More Actions</span></a>
    </div>

    <div id="createComponentTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("CreateComponent", "Component") %>'><span>New Component</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "Component") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "Component") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>


