<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Component - More Actions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#cMoreActionsTabs").tabs({ selected: parseInt($("#compMoreActionsTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="compMoreActionsTabNumber" name="compMoreActionsTabNumber" value='<%: Session["compMoreActionsTabNumber"] %>' />
    
    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
        <br />
        <br />
        <span title="Selected Component" class="selectedItem" style="margin-left:25px;"><%:Session["componentName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Components" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Components</span></a>
        <a title="Create new Sub-Component" class="topLinkButton" href='<%: Url.Action("CreateSubComponentTabs", "SubComponent") %>'><span>Create Sub-Component</span></a>
    </div>

    <div id="cMoreActionsTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("DisplayComponent", "Component", new { pCompID = (long)Session["componentID"] }) %>'><span>Component Details</span></a></li>
             <li><a href='<%: Url.Action("ListSubComponents", "SubComponent") %>'><span>Sub-Components</span></a></li>
         </ul>
    </div>

</asp:Content>
