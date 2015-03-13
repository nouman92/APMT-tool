<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Add User
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#addEmployeeTab').show('blind', 700);

        });
        $("#addEmployee input[name='" + $("#fieldId").val() + "']").blur(function () {
            $.get('<%: Url.Action("IsUserNameExist", "Employee")  %>', { pUserName: $("#addEmployee input[name='" + $("#fieldId").val() + "']").val() }, function (data) {
                if (data == "True") {
                    showError("<span style='color: Red;'>User Name '" + $("#addEmployee input[name='" + $("#fieldId").val() + "']").val() + "' already exists.</span>");
                    $('#create').attr('disabled', true);
                }
                else {
                    $('#create').removeAttr('disabled');
                    jqDialog.close();
                }
            });

        });
        $("#addEmployee").validate();

    </script>

    <div id="addEmployeeTab">
        <h3>Please provide the following information:</h3>    

        <% using (Html.BeginForm("AddEmployee", "Employee", FormMethod.Post, new { id = "addEmployee" }))
           { 
        %>
            <table class="bodyTable">
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                           if ((ControlType)item.FieldType != ControlType.CheckBox)
                           {%>
                                <b><%: item.EmpAttName %></b>
                        <%} %>
                        <%if (item.EmpAttName == "User Name")
                          { %>
                            <input type="hidden" id="fieldId" value="_<%: item.EmpAttID %>" />
                            <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.EmpAttID,
                                                        DefaultValue = item.DefaultValue,
                                                        CanNull = item.CanNull, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                        ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.FieldType,
                                                        Value = (TempData["__" + item.EmpAttID] != null)? TempData["__" + item.EmpAttID].ToString() : null,
                                                        ControlAttName = item.EmpAttName,
                                                        RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                 )
                        %>
                        <% // Display the error message if any. 
                           if (TempData["_" + item.EmpAttID] != null)
                           { %>
                               <span class="errorMessage"> <%: TempData["_" + item.EmpAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
            </table>
            <br />
            <input class="buttonDesign" type="submit" value="Create" id="create"/>
            <a class="linkButton2" href='<%: Url.Action("EmployeeTabs") %>'> <span> Cancel </span> </a>
            <br />
            <br />
        <%} %>
    </div>

</asp:Content>
