<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Note
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <script type="text/javascript">

        $("#createNote").validate();

    </script>

    <br />
    <% using (Html.BeginForm("CreateNote", "Employee", FormMethod.Post, new { id = "createNote" }))
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
                                                    Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : null,
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
                    <%: Html.TextArea("body", new { id = "body", rows = "20", cols = "70" })%>
                </td>
            </tr>
        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Create"/>
        <a class="linkButton2" href='<%: Url.Action("HomeTabs", "Home") %>'> <span> Cancel </span></a>
    <% } %>
    <br />

</asp:Content>
