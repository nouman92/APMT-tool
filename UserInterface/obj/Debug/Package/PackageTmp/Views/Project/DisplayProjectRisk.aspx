<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.ProjectRisk>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Project Risk
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <br />
    <table class="bodyTable">
        <tr>
            <td class="labelPortion">
                <b> Risk </b>
            </td>
            <td class="controlPortion">
                <%: Model.Risk.Description %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b> Risk ID </b>
            </td>
            <td class="controlPortion">
                <%: Model.RiskID %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b> Category </b>
            </td>
            <td class="controlPortion">
                <%: Model.Risk.RiskCategory.CategoryName %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b> Risk Exposure </b>
            </td>
            <td class="controlPortion">
                <%: Model.Probability * Model.Impact %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b> Mitigation </b>
            </td>
            <td class="controlPortion">
                <%: Model.Mitigation %>
            </td>
        </tr>
    </table>
    <br />

</asp:Content>
