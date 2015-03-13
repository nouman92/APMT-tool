<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.Role>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Role List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#showAllTab').show('blind', 700);
        });

        $(function () {
            $("#roleListTable2").tablesorter();
        });

        $('#organizationTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

        $("#details3").dialog({
            autoOpen: false,
            height: 'auto',
            width: 450,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Role Access Rights',
            show: 'clip'
        });


        $("#showAllTab a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details3").load(link, function () { $(this).dialog('open'); });
            return false;
        });
        
    </script>

    <div id="showAllTab">
    <span style="float:right;" title="Add new role in the Role List">[<%: Html.ActionLink("Add Role", "AddRole",null, new { name = "ajaxLink" })%>]</span>
        <table id="roleListTable2" class="bodyTable">
             <% if (Model.FirstOrDefault() != null)
               {
             %>
             <thead title="Click to Sort the List"> 
                <tr>
                    <th>
                        <h3>Role Name</h3> 
                    </th>
                 </tr>
             </thead>
             <tbody class="rowHover">
                <% foreach (var role in Model)
                   { %>
                <tr> 
                    <td class="tableColPadding"style="width:80%;">       
                           <%: role.RoleName%>
                    </td>
                    <td>
                           <%: Html.ActionLink("Update Rights", "EditRole", new { pRoleID = role.RoleID }, new { name = "dAjaxLink" })%>
                    </td>
                </tr>
                 <% } %>
            <%}                       
            else
            {%>
                <tr>
                    <td>
                        <h3><i>No Role is available.</i></h3>
                    </td>
                </tr>
            <%} %>
            </tbody>
        </table>
        <br />    
    </div>

    <div id="details3">
    </div>

</asp:Content>
