<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Issue
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#createIssueTabs").tabs({ selected: parseInt($("#createIssueTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="createIssueTabNumber" name="createIssueTabNumber" value='<%: TempData["createIssueTabNumber"] %>' />

    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
    </div>

    <div id="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Backlog" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Backlog</span></a>
    </div>

    <div id="createIssueTabs" class="spacing">
         <ul>
             <li><a href='<%: Url.Action("CreateIssue", "Issue") %>'><span>New Issue</span></a></li>
             <li><a href='<%: Url.Action("CustomFields", "Issue") %>'><span>Custom Fields</span></a></li>
             <li><a href='<%: Url.Action("CreateCustomField", "Issue") %>'><span>New Custom Field</span></a></li>
         </ul>
    </div>

</asp:Content>

