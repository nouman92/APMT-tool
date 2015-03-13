<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.SubCompAttribute>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Sub-Component - Edit Field
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editFieldInfo input:first").blur(function () {
            $.get('<%: Url.Action("IsFieldExists", "SubComponent")  %>', { pFieldName: $("#editFieldInfo input:first").val(), pFieldID: <%: Model.SubCompAttID %> }, function (data) {
                if (data == "True") {
                    showError("<span style='color: Red;'>Custom Field '" + $("#editFieldInfo input:first").val() + "' already exists.</span>");
                    $('#saveFieldInfo').attr('disabled', true);
                }
                else {
                    $('#saveFieldInfo').removeAttr('disabled');
                    jqDialog.close();
                }
            });
        });

        $("#editFieldInfo").validate();

    </script>

    <h3>Edit Field</h3>
    <br />
    <% using (Html.BeginForm("EditFieldInfo", "SubComponent", new { pFieldID = Model.SubCompAttID }, FormMethod.Post, new { id = "editFieldInfo" }))
       { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Field Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = Model.SubCompAttID,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__" + Model.SubCompAttID] != null) ? TempData["__" + Model.SubCompAttID].ToString() : Model.SubCompAttName,
                                                }, true, 2
                                             )
                    %>
                    <% // Display the error message if any. 
                    if (TempData["_" + Model.SubCompAttID] != null)
                    { %>
                        <span class="errorMessage"> <%: TempData["_" + Model.SubCompAttID]%> </span>
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
        <input class="buttonDesign" type="submit" value="Save" id="saveFieldInfo" />
        <a class="linkButton2" href='<%: Url.Action("CreateSubComponentTabs") %>'> <span> Cancel </span> </a>
        <br />
    <% } %>

</asp:Content>


