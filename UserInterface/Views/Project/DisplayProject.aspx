<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Display Project
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

        $("#favoriteLink").click(function () {
            $.get(this.href, function (data) {
                if (data == 'A') {
                    $("#favoriteLink").html("Remove from Favorite")
                    $("#favoriteLink").attr("href", "RemoveFromFavorite");
                    showNotification("Project successsfully added in your favorite list.");
                }
                else if (data == 'R') {
                    $("#favoriteLink").html("Add to Favorite")
                    $("#favoriteLink").attr("href", "AddtoFavorite");
                    showNotification("Project successsfully removed from your favorite list.");
                }
                else {
                    showError("<span style='color: Red;'>The operation was unsuccessful.</span>");
                }
            });
            return false;
        });

    </script>

    <div id="projectDetails">

        <%if (Session["showPEditButton"] != "")
          {    
              if (ViewData["isFavorite"] == null)
              {%>
                 <Span style="float:right;">[ <%: Html.ActionLink("Add to Favorite", "AddtoFavorite", null, new {id = "favoriteLink"})%> ]</Span>
              <%}
              else
              { %>
                  <Span style="float:right;">[ <%: Html.ActionLink("Remove from Favorite", "RemoveFromFavorite", null, new {id = "favoriteLink"})%> ]</Span>
              <% } 
          }%>
        <br />
        <br />
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b> Project ID</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().ProjID %>        
                </td>
            </tr>
            <% foreach (var item in Model) { %>
            <tr>
                <td class="labelPortion">
                    <b> <%: item.ProjAttribute.ProjAttName %> </b>
                </td>
                <td class="controlPortion">
                    <% // If check box is not checked then display the value "No".
                       if ((ControlType)item.ProjAttribute.FieldType == ControlType.CheckBox && item.Value == null)
                       {%>
                        <span>No</span>
                    <%} else { %>
                        <%: item.Value %>
                    <%} %>
                </td>
            </tr>
        <% } %>
        </table>
        <br />

        <%
            if (Session["showPEditButton"] != "")
          {  
        %>
            <script type="text/javascript">
                $(function () {
                    $('#projectDetails').show('blind', 700);
                });
            </script>
            <a name = "ajaxLink" title="Edit Project Details" class="linkButton2" href='<%: Url.Action("EditProject", "Project") %>'> <span> Edit </span></a>
            <br />
        <%} %>

    </div>

</asp:Content>

