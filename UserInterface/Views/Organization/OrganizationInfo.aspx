<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.OrgAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Organization Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        $('#organizationTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });
        $(function () {
            $('#organizationInfoTab').show('blind', 700);
        });
        $(function () {
            $('#employeeDetails').show('blind', 700);
        });
    </script>
    <div id="organizationInfoTab">
        <br />
        <table id="orgTable" class="bodyTable">
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="labelPortion">
                    <b>
                        <%: item.OrgAttribute.OrgAttName %>
                    </b>
                </td>
                <td class="controlPortion">
                    <%: item.Value %>
                </td>
            </tr>
            <% } %>
        </table>
        <a name="ajaxLink" title="Edit Organization Info" class="linkButton2" href='<%: Url.Action("EditOrganizationInfo", "Organization") %>'>
            <span>Edit </span></a>
        <br />
    </div>
</asp:Content>
