<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Change Password
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#match").hide();
            $('#changePasswordTabs').show('blind', 700);
            $("#changePassword").bind("submit", function () {
                if ($("#changePassword input[name='_1']").val() != $("#changePassword input[name='_2']").val()) {
                    $("#match").show();
                    $("#match").addClass("errorMessage");
                    $("#match").text("Confirm password did not match.");
                    return false;
                }
                else {
                    $("#match").hide();
                    return true;
                }
            });
        });

        $('#cancel3').click(function () {
            $('#moreActionsTabs').tabs('select', 0);
            return false;
        });
        $("#changePassword").validate();
    </script>
    <div id="changePasswordTabs">
        <br />
        <% using (Html.BeginForm("ResetUserPassword", "Employee", FormMethod.Post, new { id = "changePassword" }))
           { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>New Password</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                    {
                                                        ControlID = 1,
                                                        CanNull = false,
                                                        RegularExpression = null,
                                                        ErrorMessage = null,
                                                        Type = ControlType.Password,
                                                        Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : null,
                                                    }, true
                                                 )
                    %>
                    <% // Display the error message if any. 
                        if (TempData["_1"] != null)
                        { %>
                    <span class="errorMessage">
                        <%: TempData["_1"]%>
                    </span>
                    <% } %>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Comfirm Password</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                    {
                                                        ControlID = 2,
                                                        CanNull = false,
                                                        RegularExpression = null,
                                                        ErrorMessage = null,
                                                        Type = ControlType.Password,
                                                        Value = (TempData["__2"] != null) ? TempData["__2"].ToString() : null,
                                                    }, true
                                                 )
                    %>
                    <% // Display the error message if any. 
                        if (TempData["_2"] != null)
                        { %>
                    <span class="errorMessage">
                        <%: TempData["_2"]%>
                    </span>
                    <% } %>
                    <span id="match"/>
                </td>
            </tr>
        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Reset" />
        <input class="buttonDesign2" type="button" value="Cancel" id="cancel3" />
        <% } %>
        <br />
    </div>
</asp:Content>
