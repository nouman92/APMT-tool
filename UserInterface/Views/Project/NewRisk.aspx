<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TabContent.Master" Inherits="System.Web.Mvc.ViewPage<Domain.Entities.Risk>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	New Risk
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#addRiskCategory").bind("submit", function () {
                return false;
            });
        });

        $("#addButton").click(function () {
            if ($('#addRiskCategory').validate().form()) {

                $.get('<%: Url.Action("AddRiskCategory", "Organization")  %>', { pCategoryName: $("#_2").val() }, function (data) {
                    if (data > 0) {
                        var newOption = document.createElement("OPTION");
                        newOption.text = $("#_2").val();
                        newOption.value = data;
                        document.addRisk.categories.options.add(newOption);

                        showNotification("Category added successfully.");
                    }
                    else if (data == 0) {
                        showError("<span style='color: Red;'>Category '" + $("#_2").val() + "' already exists.</span>");
                    }
                    else if (data == -2) {
                        showError("<span style='color: Red;'>Invalid category name.</span>");
                    }
                    else {
                        showError("<span style='color: Red;'>Category did not add successfully.</span>");
                    }
                });

                $("#addCategory").dialog('close');
            }
        });

        $("#addCategoryLink").click(function () {
            var validator = $("#addRiskCategory").validate();
            validator.resetForm();
            $("#_2").attr("value", "");
            $("#addCategory").dialog('open');
            return false;
        });

        $("#addCategory").dialog({
            autoOpen: false,
            height: 'auto',
            width: 700,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Add Risk Category',
            show: 'clip'
        });

        $("#addRisk").validate();
        $("#addRiskCategory").validate();

    </script>
    
    <h3>Add Risk</h3>
    <br />
    <% using (Html.BeginForm("NewRisk", "Project", FormMethod.Post, new { id = "addRisk", name="addRisk"}))
        { %>
        <table class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Risk Description</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = 1,
                                                    CanNull = false,
                                                    RegularExpression = @"^[a-zA-Z''-'\s]*$",
                                                    ErrorMessage = "*Field contains invalid character(s).",
                                                    Type = ControlType.TextArea,
                                                    Value = (TempData["__1"] != null) ? TempData["__1"].ToString() : null
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
                    <b>Category</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("categories", (IEnumerable<SelectListItem>)ViewData["categories"], new { id = "categories" })%> <span style="padding-left:10px;" title="Add new Risk Category">[ <%: Html.ActionLink("Add Category", "#", null ,new { id="addCategoryLink"})%> ]</span>
                </td>
            </tr>
        </table>    
        <br />
        <input class="buttonDesign" type="submit" value="Add"/>
        <a class="linkButton2" href='<%: Url.Action("MoreActionsTabs", "Project") %>'> <span> Cancel </span></a>
    <% } %>
    <br />

    <div id="addCategory">

        <% using (Html.BeginForm("AddRiskCategory", "Organization", FormMethod.Post, new { id = "addRiskCategory" }))
        { %>
            <br />
            <b>Category Name</b> &nbsp &nbsp
            <%: Html.RenderControl(new ControlInfo()
                                        {
                                            ControlID = 2,
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
