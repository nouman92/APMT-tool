<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue Dependency List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#issueDependencyTab').show('blind', 700);
        });

        $("#details7").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Issue Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details7").load(link, function () { $(this).dialog('open'); });
            return false;
        });

        $("#addDepDialog").dialog({
            autoOpen: false,
            height: 320,
            width: 750,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Add Issue Dependency',
            show: 'clip',
            closeOnEscape: false
        });

        $("#addLink").click(function () {
            var link = $(this).attr('href');
            $("#addDepDialog").load(link, function () {
                $("#cancel6").click(function () {
                    $("#addDepDialog").dialog('close');
                });
                $("#showAllLink").click(function () {
                    var link = $(this).attr('href');
                    $("#addDepDialog").load(link, function () {
                        $("#cancel6").click(function () {
                            $("#addDepDialog").dialog('close');
                        });
                    });
                    return false;
                });
            }).dialog("open");
            return false;
        });

    </script>

    <div id="issueDependencyTab">
        
        <% if (Model != null)
        {
        %>
            <Span style="float:right;">[ <%: Html.ActionLink("Add More", "AddIssueDependency","Issue", null, new { id = "addLink" })%> ]</Span>
            <table class="bodyTable">
                <thead style="cursor:default">
                    <tr>
                        <th>
                            <h3 title="Sorted according to Priority">Issue has the following <%: Model.Count() %> dependenc<%: (Model.Count() > 1)?"ies":"y" %>:</h3>
                        </th>
                    </tr>
                    </thead>
                <tbody class="rowHover">
                <% foreach (var issue in Model)
                    { %>
                    <tr> 
                        <td class="tableColPadding" style="width:80%;">       
                            <%: issue.Value%>
                        </td>
                        <td style="width:20%;">
                            <%: Html.ActionLink("Details", "DisplayIssue", "Issue", new { pIssueID = issue.IssueID }, new { name = "dAjaxLink" })%> |
                            <%: Html.ActionLink("Remove", "RemoveIssueDependency", "Issue", new { pDependsOnID = issue.IssueID }, null)%>
                        </td>
                    </tr>
                <% } %>
                </tbody>
            </table>
        <%}
        else
        {%>
            <h3><i>This issue has no dependency. </i><span style="font-weight:normal; font-size:18px;"> [ <%: Html.ActionLink("Add Now", "AddIssueDependency", "Issue", null, new { id = "addLink" })%> ]</span></h3>
        <%}%>
        <br />    
    </div>

    <div id="addDepDialog">       
    </div>

    <div id="details7">
    </div>

</asp:Content>

