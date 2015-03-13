<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $("#renameIssueState").validate();

</script>

<% using (Html.BeginForm("RenameState", "Issue", FormMethod.Post, new { id = "renameIssueState" }))
{ %>
    <br />
        <b>State to be Renamed</b> &nbsp &nbsp
    <%: Html.DropDownList("states", (IEnumerable<SelectListItem>)ViewData["states"])%>
    <br />
    <br />
    <b>New Name</b>&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp
    <%: Html.RenderControl(new ControlInfo()
                                {
                                    ControlID = 0,
                                    CanNull = false,
                                    RegularExpression = @"^[a-zA-Z\s]*$",
                                    ErrorMessage = "*Field contains invalid character(s).",
                                    Type = ControlType.TextField,
                                    Value = null
                                }, false
                                )
    %>
    <br />
    <br />
    <input style="margin-left:40%;" class="buttonDesign" type="submit" value="Rename" id="addButton" />
<% } %>
