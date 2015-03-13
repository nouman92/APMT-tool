<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<script type="text/javascript">

    $(function () {
        $("[title]").tooltip({
            open: function () {
                var tooltip = $(this).tooltip("widget");
                $(document).mousemove(function (event) {
                    tooltip.position({
                        my: "left center",
                        at: "right center",
                        offset: "25 25",
                        of: event
                    });
                })
                // trigger once to override element-relative positioning
				.mousemove();
            },
            close: function () {
                $(document).unbind("mousemove");
            }
        });
    });

    $(function () {
        $("#issueListTable").tablesorter();
    });

    $("#selectAll7").change(function () {
        if ($(this).attr('checked')) {
            $("#issueListTable input[type='checkbox']").attr("checked", true);
        }
        else {
            $("#issueListTable input[type='checkbox']").attr("checked", false);
        }
    });

    $("#details").dialog({
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
        $("#details").load(link, function () { $(this).dialog('open'); });
        return false;
    });

    $("#searchViewLink").click(function () {
        var link = $(this).attr('href');
        $("#addIssueDialog").load(link, function () {
            $("#cancel6").click(function () {
                $("#addIssueDialog").dialog('close');
            });
            $("#showAllLink").click(function () {
                var link = $(this).attr('href');
                $("#addIssueDialog").load(link, function () {
                    $("#cancel6").click(function () {
                        $("#addIssueDialog").dialog('close');
                    });
                });
                return false;
            });
        });
        return false;
    });

</script>

<div id="issuesList">

    <Span style="float:right;" title="Search issue by id">[ <%: Html.ActionLink("Search View", "AddIssue", "Sprint", null, new { id = "searchViewLink" })%> ]</Span>
    <% using (Html.BeginForm("AddIssueList", "Sprint"))
        {
    %>
        <table class="bodyTable" id="issueListTable">
            <% if (Model != null)
                {%>
                <thead title="Click to Sort the List"> 
                    <tr>
                        <th>
                            <h3>Select the issue(s) to add in Sprint Backlog.</h3>
                        </th>
                    </tr>
                    </thead>
                    <tbody class="rowHover">
                    <%  int count = 1;
                        foreach (var item in Model)
                        { %>
                        <tr>
                            <td class="controlPortion" style="width:90%;">
                            <%: Html.CheckBox("_" + count, new { id = "_" + count, value = "" + item.IssueID  })%> <b><%: item.Value%></b>
                            </td>
                            <td>
                                <%: Html.ActionLink("Details", "DisplayIssue", "Issue", new { pIssueID = item.IssueID }, new { name = "dAjaxLink" })%>
                            </td>
                        </tr>
                        <%count++; %>
                <% } %>
            </tbody>
        </table>
        <br />
        <span class="controlPortion" style="width:75%;"><input type="checkbox" id="selectAll7" value="Select All" /><b>Select All</b></span>
        <br />
        <br />
        <br />
        <input class="buttonDesign" type="submit" value="Add" />
        <input class="buttonDesign2" type="button" value="Cancel" id="cancel6" />
        <%}
        else
        {%>
            <h3><i>No Issues are available.</i></h3>
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <input class="buttonDesign" type="button" value="Close" id="cancel6"/>
        <% } %>
    <% } %>
    <br />
</div>

<div id="details">
</div>


