<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.AccessRight>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Add New Role
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        $(function () {
            $('#addRoleTabs').show('blind', 700)
        });
        $("#addRole input[name='_0']").blur(function () {
            $.get('<%: Url.Action("IsRoleNameExist", "Organization")  %>', { pRoleName: $("#addRole input[name='_0']").val() }, function (data) {
                if (data == "True") {
                    showError("<span style='color: Red;'>Role Name '" + $("#addRole input[name='_0']").val() + "' already exists.</span>");
                    $('#add').attr('disabled', true);
                }
                else {
                    $('#add').removeAttr('disabled');
                    jqDialog.close();
                }
            });

        });
        $("#addRole").validate();
    </script>
    <div id="addRoleTabs">
        <h3>
            Add Role</h3>
        <% using (Html.BeginForm("AddRole", "Organization", FormMethod.Post, new { id = "addRole" }))
           { 
        %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Role Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 0,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z''-'\s]*$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__0"] != null) ? TempData["__0"].ToString() : null
                                                }, true
                                                )
                    %>
                    <% // Display the error message if any. 
                        if (TempData["_0"] != null)
                        { %>
                    <span class="errorMessage">
                        <%: TempData["_0"]%>
                    </span>
                    <% } %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Access Rights:</b>
                </td>
            </tr>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="labelPortion">
                    <b></b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.RightID,
                                                        DefaultValue = "",
                                                        CanNull = true, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression =null,
                                                        ErrorMessage = null,
                                                        Type = (ControlType)5,
                                                        Value = null,
                                                        ControlAttName = item.Description,
                                                        RegExpressionID = -1
                                                    }, true
                                                 )
                    %>
                </td>
            </tr>
            <% } %>
        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Add" id="add" />
        <a class="linkButton2" href='<%: Url.Action("OrganizationTabs", "Organization") %>'>
            <span>Cancel </span></a>
        <br />
        <br />
        <%} %>
    </div>
</asp:Content>
