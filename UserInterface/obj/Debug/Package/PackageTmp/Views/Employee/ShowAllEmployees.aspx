<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	All Users
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#allUsersTab').show('blind', 700);
        });

        $(function () {
            $("#employeeListTable2").tablesorter();
        });

        $("#details").dialog({
            autoOpen: false,
            height: 'auto',
            width: 800,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'User Details',
            show: 'clip'
        });

        $("a[name='dAjaxLink']").click(function () {
            var link = $(this).attr('href');
            $("#details").load(link, function () { $(this).dialog('open'); });
            return false;
        });
        
    </script>

    <div id="allUsersTab">
        <table id="employeeListTable2" class="bodyTable">
             <% if (Model.FirstOrDefault() != null)
               {
             %>
             <thead title="Click to Sort the List"> 
                <tr>
                    <th>
                        <h3>User Name</h3> 
                    </th>
                 </tr>
             </thead>
             <tbody class="rowHover">
                <% foreach (var emp in Model)
                   { %>
                <tr> 
                    <td class="tableColPadding" style="width:80%;">       
                        <%: emp.Value %>
                    </td>
                    <td style="width:20%;">
                        <%: Html.ActionLink("Details", "DisplayEmployee", new { pEmployeeID = emp.EmpID }, new { name = "dAjaxLink" })%> |
                        <%: Html.ActionLink("More", "MoreActionsTabs", new { pEmployeeID = emp.EmpID, pEmployeeName = emp.Value })%>
                    </td>
                </tr>
                 <% } %>
            <%}                       
            else
            {%>
                <tr>
                    <td>
                        <h3><i>No User is available.</i></h3>
                    </td>
                </tr>
            <%} %>
            </tbody>
        </table>
        <br />    
    </div>

    <div id="details">
    </div>

</asp:Content>

