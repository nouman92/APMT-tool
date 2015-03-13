<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.ProjAttribute>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Project - Edit Field
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editFieldInfo input:first").blur(function () {
            $.get('<%: Url.Action("IsFieldExists", "Project")  %>', { pFieldName: $("#editFieldInfo input:first").val(), pFieldID: <%: Model.ProjAttID %> }, function (data) {
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
    <% using (Html.BeginForm("EditFieldInfo", "Project", new { pFieldID = Model.ProjAttID }, FormMethod.Post, new { id = "editFieldInfo" }))
       { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Field Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = Model.ProjAttID,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__" + Model.ProjAttID] != null) ? TempData["__" + Model.ProjAttID].ToString() : Model.ProjAttName,
                                                }, true, 2
                                             )
                    %>
                    <% // Display the error message if any. 
                    if (TempData["_" + Model.ProjAttID] != null)
                    { %>
                        <span class="errorMessage"> <%: TempData["_" + Model.ProjAttID]%> </span>
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
        <a class="linkButton2" href='<%: Url.Action("CreateProjectTabs") %>'> <span> Cancel </span> </a>
        <br />
    <% } %>

</asp:Content>

