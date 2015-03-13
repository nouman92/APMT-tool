<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User Assigned Issues
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        var ID = 0;
        $(function () {
            $('#assignedIssuesTab').show('blind', 700);
            $("#workDone").bind("submit", function () {
                return false;
            });
            $("#issuesListTable").tablesorter();
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

        $("#update").click(function () {
            if ($('#workDone').validate().form()) {

                $.get('<%: Url.Action("WorkDone", "Issue")  %>', {pIssueID: ID, pHours: $("#_0").val() }, function (data) {
                    if (data == 1) {
                        showNotification("Operation completed successfully.");
                    }
                    else if (data == 0) {
                        showError("<span style='color: Red;'>Invalid operation.</span>");
                    }
                    else if (data == -2) {
                        showError("<span style='color: Red;'>Invalid hour(s).</span>");
                    }
                    else {
                        showError("<span style='color: Red;'>Operation did not complete successfully.</span>");
                    }
                });

                $("#effort").dialog('close');
            }
        });

        $("a[name='effortLink']").click(function () {
            var link = $(this).attr('href');
            ID = link.split('=')[1];
            var validator = $("#workDone").validate();
            validator.resetForm();
            $("#_0").attr("value", "");
            $("#remaining").load(link);
            $("#effort").dialog('open');
            return false;
        });

        $("#effort").dialog({
            autoOpen: false,
            height: 'auto',
            width: 530,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Enter Work Done',
            show: 'clip'
        });

        $("#workDone").validate();

    </script>

	<div id="assignedIssuesTab">
        <table id="issuesListTable" class="bodyTable">
             <% if (Model.FirstOrDefault() != null)
                {
                    int index = 0;
                    List<bool> isValidSprint = (List<bool>)ViewData["isValidSprint"];
                    List<bool> isSprintStarted = (List<bool>)ViewData["isSprintStarted"];
             %>
             <thead title="Click to Sort the List"> 
                <tr>
                    <th>
                        <h3>You have the follwing <%:Model.Count()%> issue(s) to be done:</h3> 
                    </th>
                 </tr>
             </thead>
             <tbody class="rowHover">
             <% foreach (var issue in Model)
                {%>
                <%// If sprint not started yet then do not display the assigned issue.
                    if (isSprintStarted[index])
                  {%>
                <tr>
                   <td class="tableColPadding" style="width:75%;">
                       <%: issue.Value%>
                   </td>
                   <td style="width:25%;">
                       <%: Html.ActionLink("Details", "DisplayIssue", "Issue", new { pIssueID = issue.IssueID }, new { name = "dAjaxLink" })%> 
                       <%if (isValidSprint[index])
                         {%>
                       | <span title="Enter work done"><%: Html.ActionLink("Effort", "GetManHours", "Issue", new { pIssueID = issue.IssueID }, new { name = "effortLink" })%></span> | 
                         <span title="Execute issue work-flow"><%: Html.ActionLink("Execute", "ExecuteWorkFlow", "Issue", new { pIssueID = issue.IssueID }, null)%></span>
                       <%} %>
                   </td>
                </tr>
                 <%}
                     index++;%>
                <%}
                }
                else
                {%>
                <tr>
                    <td>
                        <h3><i>You don't have any issue to work out.</i></h3>
                    </td>
                </tr>
            <%} %>
            </tbody>
        </table>
        <br />
	</div>

    <div id="details">
    </div>

    <div id="effort">

        <br />
        <b>Remaining Man Hours: </b> &nbsp<span id="remaining"></span>
        <% using (Html.BeginForm("WorkDone", "Issue", FormMethod.Post, new { id = "workDone" }))
        { %>
            <br />
            <b>Work Done</b> &nbsp &nbsp &nbsp &nbsp &nbsp
            <%: Html.RenderControl(new ControlInfo()
                                        {
                                            ControlID = 0,
                                            CanNull = false,
                                            RegularExpression = @"^\d+((.25)|(.50)|(.5)|(.75)|(.0)|(.00))?$",
                                            ErrorMessage = "*Invalid Hour(s)",
                                            Type = ControlType.TextField,
                                            Value = null
                                        }, false
                                        )
            %>
            <br />
            <br />
            <input style="margin-left:40%;" class="buttonDesign" type="button" value="Update" id="update" />
            <br />
        <% } %>

    </div>

</asp:Content>

