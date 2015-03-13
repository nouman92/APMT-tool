<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.PersonalNote>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Display Note
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <br />
    <table class="bodyTable">
        <tr>
            <td class="labelPortion">
                <b>Creation Date</b>
            </td>
            <td class="controlPortion">
                <%: Model.CreationDate.ToString("dd MMMM yyyy")%>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b>Subject</b>
            </td>
            <td class="controlPortion">
                <%: Model.Subject %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b>Body</b>
            </td>
            <td class="controlPortion">
                <%: Model.Body %>
            </td>
        </tr>
    </table>
    <br />

</asp:Content>
