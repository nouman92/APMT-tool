<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.IssueAttribute>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue - Edit Field
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $("#editFieldInfo input:first").blur(function () {
            $.get('<%: Url.Action("IsFieldExists", "Issue")  %>', { pFieldName: $("#editFieldInfo input:first").val(), pFieldID: <%: Model.IssueAttID %>  }, function (data) {
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
    <% using (Html.BeginForm("EditFieldInfo", "Issue", new { pFieldID = Model.IssueAttID }, FormMethod.Post, new { id = "editFieldInfo" }))
       { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Field Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = Model.IssueAttID,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextField,
                                                    Value = (TempData["__" + Model.IssueAttID] != null) ? TempData["__" + Model.IssueAttID].ToString() : Model.IssueAttName,
                                                }, true, 2
                                             )
                    %>
                    <% // Display the error message if any. 
                    if (TempData["_" + Model.IssueAttID] != null)
                    { %>
                        <span class="errorMessage"> <%: TempData["_" + Model.IssueAttID]%> </span>
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
        <a class="linkButton2" href='<%: Url.Action("CreateIssueTabs") %>'> <span> Cancel </span> </a>
        <br />
    <% } %>

</asp:Content>