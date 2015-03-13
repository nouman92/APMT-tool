<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.CompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Display Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $('#cMoreActionsTabs').tabs({
            load: function (event, ui) {
                $("a[name='ajaxLink']", ui.panel).click(function () {
                    $(ui.panel).load(this.href);
                    return false;
                });
            }
        });

    </script>

    <div id="componentDetails">
        <br />
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b> Component ID</b>
                </td>
                <td class="controlPortion">
                    <%: Model.First().CompID %>        
                </td>
            </tr>
            <% foreach (var item in Model) { %>
            <tr>
                <td class="labelPortion">
                    <b> <%: item.CompAttribute.CompAttName %> </b>
                </td>
                <td class="controlPortion">
                    <% // If check box is not checked then display the value "No".
                       if ((ControlType)item.CompAttribute.FieldType == ControlType.CheckBox && item.Value == null)
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
            if (Session["showCEditButton"] != "")
          {  
        %>
            <script type="text/javascript">
                $(function () {
                    $('#componentDetails').show('blind', 700);
                });
            </script>
            <a name = "ajaxLink" title="Edit Component Details" class="linkButton2" href='<%: Url.Action("EditComponent", "Component") %>'> <span> Edit </span></a>
            <br />
        <%} %>

    </div>

</asp:Content>

