<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Issue
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $('#newIssueTab').show('blind', 700);
            $("#askForMore").hide();
            $("#addIssueType").bind("submit", function () {
                return false;
            });
        });

        $("#createIssue").validate();

        $(function () {
            $.ajax({
                url: '<%: Url.Action("GetEmployeesNames", "Employee")  %>',
                success: function (data) {
                    var empList = data;
                    $("#_8").autocomplete({
                        source: empList
                    });
                }
            });
        });

        function Ask() {
            $("#askForMore").dialog({
                height: 'auto',
                width: 300,
                modal: true,
                draggable: false,
                position: 'center',
                resizable: false,
                title: 'Create More Issues',
                closeOnEscape: false,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); }
            });

            $("#yes").click(function () {
                $("#askForMore").dialog('close');
            });
        }

        $("#addButton").click(function () {
            if ($('#addIssueType').validate().form()) {

                $.get('<%: Url.Action("AddIssueType", "Issue")  %>', { pTypeName: $("#_0").val() }, function (data) {
                    if (data > 0) {
                        var newOption = document.createElement("OPTION");
                        newOption.text = $("#_0").val();
                        newOption.value = data;
                        document.createIssue.issueTypes.options.add(newOption);

                        showNotification("Issue Type added successfully.");
                    }
                    else if (data == 0) {
                        showError("<span style='color: Red;'>Issue Type '" + $("#_0").val() + "' already exists.</span>");
                    }
                    else if (data == -2) {
                        showError("<span style='color: Red;'>Invalid type name.</span>");
                    }
                    else {
                        showError("<span style='color: Red;'>Issue Type did not add successfully.</span>");
                    }
                });

                $("#addType").dialog('close');
            }
        });

        $("#addTypeLink").click(function () {
            var validator = $("#addIssueType").validate();
            validator.resetForm();
            $("#_0").attr("value", "");
            $("#addType").dialog('open');
            return false;
        });

        $("#addType").dialog({
            autoOpen: false,
            height: 'auto',
            width: 700,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Add Issue Type',
            show: 'clip'
        });

        $("#addIssueType").validate();

    </script>

    <div id="newIssueTab">

        <% if(TempData["askForMore"] != null) {%>
            <script type="text/javascript">
                Ask();
            </script>
        <%} %>

        <h3>Please provide the following information:</h3>    
        <% using (Html.BeginForm("CreateIssue", "Issue", FormMethod.Post, new { id = "createIssue", name = "createIssue" }))
           { 
        %>
            <table class="bodyTable">
                <tr>
                    <td class="labelPortion">
                        <b>Issue Type</b>
                    </td>
                    <td  class="controlPortion">
                        <%: Html.DropDownList("issueTypes", (IEnumerable<SelectListItem>)ViewData["issueTypes"], new { id = "issueTypes" })%>
                        <span style="padding-left:10px;" title="Add new Issue Type">[ <%: Html.ActionLink("Add Type", "#", null ,new { id="addTypeLink"})%> ]</span>
                    </td>
                </tr>
                <tr>
                    <td class="labelPortion">
                        <b>Issue Priority</b>
                    </td>
                    <td class="controlPortion">
                        <%: Html.DropDownList("issuePriorities", (IEnumerable<SelectListItem>)ViewData["issuePriorities"], new { id = "issuePriorities" })%>
                    </td>
                </tr>
            <% foreach (var item in Model) { %>
                <tr>
                    <td class="labelPortion">
                        <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                           if ((ControlType)item.FieldType != ControlType.CheckBox)
                           {%>
                                <b><%: item.IssueAttName%></b>
                        <%} %>
                    </td>
                    <td class="controlPortion">
                        <%: Html.RenderControl(new ControlInfo ()
                                                    {
                                                        ControlID = item.IssueAttID,
                                                        DefaultValue = item.DefaultValue,
                                                        CanNull = item.CanNull, 
                                                        // This field can be null. So, if field is null then don't pick the value of expression
                                                        // from Regular Expression table. 
                                                        RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                        ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                        Type = (ControlType)item.FieldType,
                                                        Value = (TempData["__" + item.IssueAttID] != null)? TempData["__" + item.IssueAttID].ToString() : null,
                                                        ControlAttName = item.IssueAttName,
                                                        RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                    }, true
                                                 )
                        %>
                        <% // Display the error message if any. 
                           if (TempData["_" + item.IssueAttID] != null)
                           { %>
                               <span class="errorMessage"> <%: TempData["_" + item.IssueAttID]%> </span>
                        <% } %>
                    </td>
                </tr>
            <% } %>
            </table>
            <br />
            <input class="buttonDesign" type="submit" value="Create" />
            <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span> </a>
            <br />
            <br />
        <%} %>
    </div>

    <div id="askForMore">
        <p>
        Issue created successfully.<br />
        Do you want to create more?<br />
        </p>    
        <input class="buttonDesign" type="button" value="Yes" id="yes" />
        <a tabindex="1" class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> &nbsp No &nbsp </span></a>
    </div>

    <div id="addType">

        <% using (Html.BeginForm("AddIssueType", "Issue", FormMethod.Post, new { id = "addIssueType" }))
        { %>
            <br />
            <b>Type Name</b> &nbsp &nbsp
            <%: Html.RenderControl(new ControlInfo()
                                        {
                                            ControlID = 0,
                                            CanNull = false,
                                            RegularExpression = @"^[a-zA-Z\s]*$",
                                            ErrorMessage = "*Field contains invalid character(s).",
                                            Type = ControlType.TextField,
                                            Value = null
                                        }, false
                                        )
            %>
            <br />
            <br />
            <input style="margin-left:40%;" class="buttonDesign" type="button" value="Add" id="addButton" />
        <% } %>

    </div>

</asp:Content>

