﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User - Create Custom Field
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#newCustomFieldTab').show('blind', 700);
            $("#createCustomField").bind("submit", function () {
                if ($('#createCustomField').validate().form()) {
                    $('#canNull').removeAttr('disabled');
                    $('#regularExpression').removeAttr('disabled');
                    $('#defaultValue').removeAttr('class');
                    $('#defaultValue').removeAttr('disabled');
                }
            });
        });

        $("#createCustomField input[name='_1']").blur(function () {
            $.get('<%: Url.Action("IsFieldExists", "Employee")  %>', { pFieldName: $("#createCustomField input[name='_1']").val() }, function (data) {
                if (data == "True") {
                    showError("<span style='color: Red;'>Custom Field '" + $("#createCustomField input[name='_1']").val() + "' already exists.</span>");
                    $('#create2').attr('disabled', true);
                }
                else {
                    $('#create2').removeAttr('disabled');
                    jqDialog.close();
                }
            });
        });


        $('#cancel3').click(function () {
            $('#addEmployeeTabs').tabs('select', 0);
            return false;
        });

        $("#fieldType").change(function () {
            var id = $(this).val();

            if (id == 5) {
                $('#defaultValue').attr('disabled', true);
                $('#canNull').attr('disabled', true);
                $('#regularExpression').attr('disabled', true);
            }
            else if (id == 4) {
                $('#defaultValue').removeAttr('disabled');
                $('#defaultValue').attr("class", "required");
                $('#canNull').attr('disabled', true);
                $('#regularExpression').attr('disabled', true);
            }
            else {
                $('#defaultValue').removeAttr('disabled');
                $('#defaultValue').removeAttr('class');
                $('#canNull').removeAttr('disabled');
                $('#regularExpression').removeAttr('disabled');
            }
        });

        $("#createCustomField").validate();

    </script>

    <div id="newCustomFieldTab">
        <h5>
            <i>* For the field type List, put the list options in Default Value field separated with <b>;</b></i> <span class="helpSign" title="C#;Java;C++;Visual Basic"><b>?</b></span>
            <br />
            <i>* You can only modify the Field Name property once created.</i>
            <br />
            <i>* Organization Level field will be available in all Users by default from onwards.</i> <span class="helpSign" title="Will not modify later"><b>!</b></span>
            <br />
        </h5>

        <h3>Please provide the following information:</h3>
        <br />
        <% using (Html.BeginForm("CreateCustomField", "Employee", FormMethod.Post, new { id = "createCustomField" }))
           {%>
            <table class="bodyTable">
                <tr>
                    <td class="labelPortion">
                        <b>Field Name</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo()
                                                    {
                                                        ControlID = 1,
                                                        CanNull = false,
                                                        RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                        ErrorMessage = "*Field contains invalid character(s).",
                                                        Type = ControlType.TextField,
                                                        Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : null,
                                                    }, true
                                                 )
                        %>
                        <% // Display the error message if any. 
            if (TempData["_1"] != null)
            { %>
                               <span class="errorMessage"> <%: TempData["_1"]%> </span>
                        <% } %>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Field Type</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.DropDownList("fieldType", (IEnumerable<SelectListItem>)ViewData["fieldTypes"], new { id = "fieldType" })%>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Default Value</b>
                    </td>
                    <td class="controlPortion">
                        <% if (TempData["defaultValue"] != null)
                           {%>
                        <%: Html.TextBox("defaultValue", "", new { id = "defaultValue" })%>
                        <%} // Display the value saved in TempData dictionay.
                           else
                           { %>
                               <%: Html.TextBox("defaultValue", TempData["defaultValue"], new { id = "defaultValue" })%>
                        <% } %>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Can Null</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.DropDownList("canNull", (IEnumerable<SelectListItem>)ViewData["trueFalseOption"], new { id = "canNull" })%>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Regular Expression</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.DropDownList("regularExpression", (IEnumerable<SelectListItem>)ViewData["regularExpressions"], new { id = "regularExpression" })%>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Organization Level</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.DropDownList("isSystemLevel", (IEnumerable<SelectListItem>)ViewData["trueFalseOption"])%>
                    </td>
                </tr> 
            </table>    
            <br />
            <input class="buttonDesign" type="submit" value="Create" id ="create2"/>
            <input class="buttonDesign2" type="button" value="Cancel" id="cancel3" />        
        <% } %>
        <br />
    </div>

</asp:Content>