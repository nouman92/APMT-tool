<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.EmpAttribute>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User - Edit Field
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editFieldInfo").validate();
        $("#editFieldInfo input[name='_1']").blur(function () {
            $.get('<%: Url.Action("IsFieldExists", "Employee")  %>', { pFieldName: $("#editFieldInfo input[name='_1']").val(), pFieldID: <%: Model.EmpAttID %> }, function (data) {
                if (data=="True") {
                    showError("<span style='color: Red;'>Custom Field '" + $("#editFieldInfo input[name='_1']").val() + "' already exists.</span>");
                    $('#edit').attr('disabled', true);
                }
                else {
                    $('#edit').removeAttr('disabled');
                    jqDialog.close();
                }
            });
        });
    </script>

    <h3>Field Details</h3>
    <br />
    <% using (Html.BeginForm("EditFieldInfo", "Employee", new { pFieldID = Model.EmpAttID }, FormMethod.Post, new { id = "editFieldInfo" }))
       {%>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Field Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 1,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__" + Model.EmpAttID] != null) ? TempData["__" + Model.EmpAttID].ToString() : Model.EmpAttName,
                                                }, true, 2
                                             )
                    %>
                    <% // Display the error message if any. 
                    if (TempData["_" + Model.EmpAttID] != null)
                    { %>
                        <span class="errorMessage"> <%: TempData["_" + Model.EmpAttID]%> </span>
                    <% } %>
                </td>
            </tr>

            <% if ((ControlType)Model.FieldType == ControlType.List)
               {%>
                    <tr>
                        <td class="labelPortion">
                            <b>New Option(s)</b>
                        </td>
                        <td class="controlPortion">
                            <span title="Option(s) will be added in existing list">
                            <%: Html.RenderControl(new ControlInfo()
                                                        {
                                                            ControlID = 0,
                                                            CanNull = true,
                                                            Type = ControlType.TextField,
                                                            RegularExpression = null,
                                                            ErrorMessage = null,
                                                            Value = (TempData["__0"] != null) ? TempData["__0"].ToString() : "",
                                                        }, true, 2
                                                     )
                            %>
                            </span> 
                            <span class="helpSign" title="C#;Java;C++;Visual Basic"><b>?</b></span>
                        </td>
                    </tr>
            <% } %>

        </table>
        <br />
        <input class="buttonDesign" type="submit" value="Save" id="edit" />
        <a class="linkButton2" href='<%: Url.Action("AddEmployeeTabs") %>'> <span> Cancel </span> </a>
        <br />
    <% } %>

</asp:Content>