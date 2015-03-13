<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Domain.Entities.MeetingDetail>" %>

<% if (Model != null)
 {%>
    <% using (Html.BeginForm("EditMeetingDetail", "Sprint", new { pMeetingID = Model.MeetingID, pEmpID = Model.EmpID }))
       { %>
    <h3>Edit Details</h3>    
    <table class="bodyTable">
        <tr>
            <td >
                &nbsp &nbsp &nbsp &nbsp &nbsp <b>User</b> &nbsp &nbsp &nbsp &nbsp <%:ViewData["userName"]%>, <b>ID: </b><%: Model.EmpID%>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp &nbsp &nbsp &nbsp &nbsp <b>Yesterday</b><br /> &nbsp &nbsp &nbsp &nbsp &nbsp <%: Html.TextArea("yesterday", new { id = "yesterday", rows = "4", cols = "50" })%>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp &nbsp &nbsp &nbsp &nbsp <b>Today</b><br /> &nbsp &nbsp &nbsp &nbsp &nbsp <%: Html.TextArea("today", new { id = "today", rows = "4", cols = "50" })%>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp &nbsp &nbsp &nbsp &nbsp <b>Tomorrow</b><br /> &nbsp &nbsp &nbsp &nbsp &nbsp <%: Html.TextArea("tomorrow", new { id = "tomorrow", rows = "4", cols = "50" })%>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp &nbsp &nbsp &nbsp &nbsp <b>Comments</b><br /> &nbsp &nbsp &nbsp &nbsp &nbsp <%: Html.TextArea("comments", new { id = "comments", rows = "4", cols = "50" })%>
            </td>
        </tr>
    </table>    
    <br />
    <input class="buttonDesign" style="margin-left:40%;" type="submit" value="Save" id="save"/>
<%  }
}
else
{%>
    <h3><i>User has no meeting on this date.</i></h3>  
<%} %>
<br />
