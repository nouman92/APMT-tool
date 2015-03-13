<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.SubCompAttribute>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Sub-Component - Field Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <br />
    <table class="bodyTable">
        <tr>
            <td class="labelPortion">
                <b>Field Name</b>
            </td>
            <td class="controlPortion">
                <%: Model.SubCompAttName %>
            </td>
        </tr>
        <tr>
            <td class="labelPortion">
                <b>Field Type</b>
            </td>
            <td class="controlPortion">
                <%: Model.FieldType1.FieldName %>
            </td>
        </tr>
        <tr>
            <% // If the field is of type list, then display the list.
              if ((ControlType)Model.FieldType == ControlType.List)
              {%>
                    <td class="labelPortion">
                        <b>List Option(s)</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = Model.SubCompAttID,
                                                        DefaultValue = Model.DefaultValue,
                                                        CanNull = Model.CanNull,
                                                        RegularExpression = null,
                                                        ErrorMessage = null,
                                                        Type = (ControlType)Model.FieldType,
                                                        RegExpressionID = -1
                                                    }, false
                                                 )
                        %>
                    </td>
              <%} // If field type is check box, then this info is useless.
              else if ((ControlType)Model.FieldType != ControlType.CheckBox)
              { %>
                    <td class="labelPortion">
                        <b>Default Value</b>
                    </td>
                    <td class="controlPortion">
                        <%: Model.DefaultValue%>
                    </td>
            <%} %>
        </tr>
        <tr>
            <%if ((ControlType)Model.FieldType != ControlType.CheckBox && (ControlType)Model.FieldType != ControlType.List)
              { %>
                    <td class="labelPortion">
                        <b>Can Null</b>
                    </td>
                    <td class="controlPortion">
                        <%: Model.CanNull%>
                    </td>
        </tr>
        <tr>
                    <td class="labelPortion">
                        <b>Regular Expression</b>
                    </td>
                    <td class="controlPortion">
                        <%: (Model.RegularExpression != null)? Model.RegularExpression1.ExpressionName:"None" %>
                    </td>
            <%} %>
        </tr>
        <tr>
            <td class="labelPortion">
                <b>Organization Level</b>
            </td>
            <td class="controlPortion">
                <%: Model.IsSystemLevel%>
            </td>
        </tr>    
    </table>
    <br />

</asp:Content>

