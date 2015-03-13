<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Daily Scrum Meeting
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#meetingTab').show('blind', 700);
        });

    </script> 

    <div id="meetingTab">

        <% if (ViewData["users"] != null)
           {%>
        <% using (Html.BeginForm("ScrumMeeting", "Sprint"))
           { %>
            <table class="bodyTable">
                <tr>
                    <td >
                        &nbsp &nbsp &nbsp &nbsp &nbsp <b>Select User</b> &nbsp &nbsp &nbsp &nbsp <%: Html.DropDownList("users", (IEnumerable<SelectListItem>)ViewData["users"], new { id = "users" })%>
                        <br />
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="controlPortion">
                        &nbsp &nbsp &nbsp <b>Yesterday</b><br /> &nbsp &nbsp &nbsp<%: Html.TextArea("yesterday", new { id = "yesterday", rows = "4", cols = "50" })%>
                    </td>
                </tr>
                <tr>
                    <td class="controlPortion">
                        &nbsp &nbsp &nbsp <b>Today</b><br /> &nbsp &nbsp &nbsp<%: Html.TextArea("today", new { id = "today", rows = "4", cols = "50" })%>
                    </td>
                </tr>
                <tr>
                    <td class="controlPortion">
                        &nbsp &nbsp &nbsp <b>Tomorrow</b><br /> &nbsp &nbsp &nbsp<%: Html.TextArea("tomorrow", new { id = "tomorrow", rows = "4", cols = "50" })%>
                    </td>
                </tr>
                <tr>
                    <td class="controlPortion">
                        &nbsp &nbsp &nbsp <b>Comments</b><br /> &nbsp &nbsp &nbsp<%: Html.TextArea("comments", new { id = "comments", rows = "4", cols = "50" })%>
                    </td>
                </tr>
            </table>    
            <br />
            <input class="buttonDesign" type="submit" value="Save" id="save"/>
            <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs") %>'> <span> Cancel </span> </a>
        <%  }
           }
           else
           {%>
                <h3><i>No user in available.</i></h3>
                <br />
                <br />
            <a class="linkButton" href='<%: Url.Action("MoreActionsTabs") %>'> <span> Back </span> </a>
         <%} %>
        <br />

    </div>

</asp:Content>
