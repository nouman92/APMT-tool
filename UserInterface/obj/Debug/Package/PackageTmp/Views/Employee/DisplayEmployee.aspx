<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Display User
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        $('#moreActionsTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });
    </script>
    <div id="employeeDetails">
        <br />
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Employee ID</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().EmpID %>
                </td>
            </tr>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="labelPortion">
                    <b>
                        <%: item.EmpAttribute.EmpAttName %>
                    </b>
                </td>
                <td class="controlPortion">
                    <% // If check box is not checked then display the value "No".
                        if ((ControlType)item.EmpAttribute.FieldType == ControlType.CheckBox && item.Value == null)
                        {%>
                    <span>No</span>
                    <%}
                       else
                       { %>
                    <%: item.Value %>
                    <%} %>
                </td>
            </tr>
            <% } %>
        </table>
        <br />
        <%  %>
        <%
            if (Session["showEEditButton"] != "")
            {  
        %>
        <script type="text/javascript">
            $(function () {
                $('#employeeDetails').show('blind', 700);
            });
        </script>
        <a name="ajaxLink" title="Edit User Details" class="linkButton2" href='<%: Url.Action("EditEmployee", "Employee") %>'>
            <span>Edit </span></a>
        <br />
        <%} %>
    </div>

</asp:Content>
