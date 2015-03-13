<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ScrumMeeting>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Scrum Meetings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#meetingsTab').show('blind', 700);
        });

        $(function () {
            $("#meetingsTable").tablesorter();
        });

        $("#details5").dialog({
            autoOpen: false,
            height: 'auto',
            width: 680,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Meeting Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details5").load(link, function () {
                $(this).dialog('open');
                $("#ok").click(function () {
                    $("#details5").load('<%: Url.Action("MeetingDetails", "Sprint")%>', { pEmpID: $("#users").val() });
                });
            });
            return false;
        });

        $("a[name='dEAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details5").load(link, function () {
                $(this).dialog('open');
                $("#ok").click(function () {
                    $("#details5").load('<%: Url.Action("EditMeetingDetails", "Sprint")%>', { pEmpID: $("#users").val() });
                });
            });
            return false;
        });

    </script>

    <div id="meetingsTab">
        
            <% if (Model.Count() > 0)
            {
            %>
            <table id="meetingsTable" class="bodyTable">
                <thead>
                    <tr>
                        <th>
                            <h3 title="Click to Sort the List">Sprint has the following <%: Model.Count() %> meeting(s):</h3>
                        </th>
                    </tr>
                </thead>
                <tbody class="rowHover">
                <% foreach (var meeting in Model)
                    { %>
                <tr> 
                    <td class="tableColPadding" style="width:80%;">
                            <%: meeting.MeetingDate.ToString("dd MMMM yyyy")%>
                    </td>
                    <td style="width:20%;">
                        <%: Html.ActionLink("Details", "SprintTeam", "Sprint", new { pMeetingID = meeting.MeetingID }, new { name = "dAjaxLink" })%> |
                        <%: Html.ActionLink("Edit", "SprintTeam", "Sprint", new { pMeetingID = meeting.MeetingID }, new { name = "dEAjaxLink" })%>
                    </td>
                </tr>
                    <% } %>
                </tbody>
            </table>
            <%}
            else
            {%>
                <br /> 
                <h3><i>Sprint has no meeting. </i></h3>
            <%}%>
        <br />    
    </div>
   
    <div id="details5">
    </div>

    <div id="searchDialog2">        
    </div>

</asp:Content>

