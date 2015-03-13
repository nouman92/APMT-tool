<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.PersonalNote>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Edit Note
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editNote").validate();

    </script>

    <h3>Edit Note</h3>
    <br />
    <% using (Html.BeginForm("EditNote", "Employee", new { pNoteID = Model.NoteID}, FormMethod.Post, new { id = "editNote" }))
       {%>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Subject</b>
                </td>
                <td class="controlPortion" style="width:40em;">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 1,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : Model.Subject,
                                                }, true
                                                )
                    %>
                    <% // Display the error message if any. 
        if (TempData["_1"] != null)
        { %>
                            <span class="errorMessage"> <%: TempData["_1"]%> </span>
                    <% } %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Body</b>
                </td>
                <td class="controlPortion">
                    <%: Html.TextArea("body", new { id = "body", rows = "20", cols = "70", value = Model.Body})%>
                </td>
            </tr>
        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Update"/>
        <a class="linkButton2" href='<%: Url.Action("HomeTabs", "Home") %>'> <span> Cancel </span></a>
    <% } %>
    <br />

</asp:Content>
