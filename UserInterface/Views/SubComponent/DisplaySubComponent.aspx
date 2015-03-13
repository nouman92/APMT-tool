<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.SubCompAttValue>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Display Sub-Component
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <div id="subCompDetails">
        <br />
        <table class="bodyTable">
            <tr>
                <td style="width:40%; padding-left:20px;padding-bottom:15px;">
                    <b> Sub-Component ID</b>
                </td>
                <td style="width:60%;padding-bottom:15px;">
                    <%: Model.First().SubCompID %>        
                </td>
            </tr>
            <% foreach (var item in Model) { %>
            <tr>
                <td style="width:40%; padding-left:20px;padding-bottom:15px;">
                    <b> <%: item.SubCompAttribute.SubCompAttName %> </b>
                </td>
                <td style="width:60%;padding-bottom:15px;">
                    <% // If check box is not checked then display the value "No".
                       if ((ControlType)item.SubCompAttribute.FieldType == ControlType.CheckBox && item.Value == null)
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
    </div>

</asp:Content>

