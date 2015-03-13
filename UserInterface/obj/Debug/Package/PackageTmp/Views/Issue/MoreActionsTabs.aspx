<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue - More Actions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#iMoreActionsTabs").tabs({ selected: parseInt($("#issueMoreActionsTabNumber").val(), 10) });
        });

    </script>

    <input type="hidden" id="issueMoreActionsTabNumber" name="issueMoreActionsTabNumber" value='<%: Session["issueMoreActionsTabNumber"] %>' />
    
    <div style="margin-top:9px;">
        <span title="Selected Project" class="selectedItem"><%:Session["projectName"]%></span>
        <br />
        <br />
        <span title="Selected Issue" class="selectedItem" style="margin-left:25px;"><%:Session["issueName"]%></span>
    </div>

    <div id="topNavigation" class="topNavigation">
        <a title="Go to Project Tabs" class="topLinkButton" href='<%: Url.Action("ProjectTabs", "Project") %>'><span>Project</span></a>
        <a title="Go back to Project Backlog" class="topLinkButton" href='<%: Url.Action("MoreActionsTabs", "Project") %>'><span>Backlog</span></a>
    </div>

    <div id="iMoreActionsTabs" class="spacing">
        <ul>
            <li><a href='<%: Url.Action("IssueDetails", "Issue", new { pIssueID = (long)Session["issueID"] }) %>'><span>Issue Details</span></a></li>
            <li><a href='<%: Url.Action("IssueDependency", "Issue") %>'><span>Issue Dependency</span></a></li>
            <li><a href='<%: Url.Action("IssueAssignee", "Issue") %>'><span>Remove Assignee</span></a></li>
        </ul>
    </div>

</asp:Content>
