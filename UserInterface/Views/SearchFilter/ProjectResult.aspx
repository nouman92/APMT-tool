<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Project Results
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
             <% foreach (var proj in Model)
             { %>
             <tr> 
                 <td class="tableColPadding">       
                     <%: proj.Value %>
                 </td>
                 <td>
                     <%Session["showPEditButton"] = ""; %>
                     <%: Html.ActionLink("Details", "DisplayProject","Project", new { pProjectID = proj.ProjID }, new { name = "dAjaxLink" })%> |
                     <%: Html.ActionLink("More", "MoreActionsTabs", "Project", new { pProjectID = proj.ProjID, pProjectName = proj.Value, pIsFilterCall=1 }, null)%>
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

