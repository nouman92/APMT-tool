<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Domain.Entities.MeetingDetail>" %>

<% if(Model != null) {%>
    <br />
    <table style="padding-left:20px;">
        <tr>
            <td>
                <b>User</b> 
            </td>
            <td style="padding-left:25px;">
                <%:ViewData["userName"] %>, <b>ID: </b><%: Model.EmpID%>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <b>Yesterday</b> 
            </td>
            <td style="padding-left:25px;">
                <br />
                <%: Model.Yesterday%>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <b>Today</b> 
            </td>
            <td style="padding-left:25px;">
                <br />
                <%: Model.Today%>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <b>Tomorrow</b> 
            </td>
            <td style="padding-left:25px;">
                <br />
                <%: Model.Tomorrow%>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <b>Comments</b> 
            </td>
            <td style="padding-left:25px;">
                <br />
                <%: Model.Comments%>
            </td>
        </tr>
    </table>
    <br />
<%} else{%>
    <h3><i>User has no meeting on this date.</i></h3>  
<%} %>