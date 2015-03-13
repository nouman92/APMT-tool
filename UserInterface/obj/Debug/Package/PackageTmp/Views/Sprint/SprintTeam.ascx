<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<br />
<table class="bodyTable">
    <tr>
        <td >
            &nbsp &nbsp &nbsp &nbsp &nbsp <b>Select User</b> &nbsp &nbsp &nbsp &nbsp <%: Html.DropDownList("users", (IEnumerable<SelectListItem>)ViewData["users"], new { id = "users" })%>
            <br />
            <br />
            <br />
        </td>
    </tr>
</table>
<input class="buttonDesign" style="margin-left:40%;" type="button" value="OK" id="ok"/>
<br />
