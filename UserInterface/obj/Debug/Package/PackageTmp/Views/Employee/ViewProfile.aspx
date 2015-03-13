<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    User Profile
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">
        $(function () {
            $('#employeeDetails').show('blind', 700);
        });
        $('#userProfileTabs').tabs({
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
        <a name="ajaxLink" title="Edit your Profile" class="linkButton2" href='<%: Url.Action("EditProfile", "Employee") %>'>
            <span>Edit</span></a>
        <br />
    </div>

</asp:Content>
