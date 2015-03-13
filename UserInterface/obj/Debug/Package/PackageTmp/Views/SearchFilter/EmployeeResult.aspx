<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.EmpAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Employee Results
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">
        $(function () {
            $("#searchResultTable").tablesorter();
        }); 

    </script>
    
    <hr />
     <% if (Model != null)
      {
     %>
    <table id="searchResultTable" class="bodyTable">
        <thead title="Click to Sort the List"> 
            <tr>
                <th>
                    <h3>Following <%: Model.Count() %> record(s) found:</h3> 
                </th>
             </tr>
         </thead>
         <tbody class="rowHover">
             <% foreach (var emp in Model)
             { %>
             <tr> 
                 <td class="tableColPadding">       
                     <%: emp.Value %>
                 </td>
                 <td>
                     <%Session["showEEditButton"] = ""; %>
                     <%: Html.ActionLink("Details", "DisplayEmployee","Employee", new { pEmployeeID = emp.EmpID }, new { name = "dAjaxLink" })%> |
                     <%: Html.ActionLink("More", "MoreActionsTabs", "Employee", new { pEmployeeID = emp.EmpID, pEmployeeName = emp.Value }, null)%>
                 </td>
             </tr>
             <% } %>
         </tbody>
     </table>
     <%}
     else
     {%>
        <h3><i>No record found.</i></h3>
     <%} %>
     <br />

</asp:Content>

