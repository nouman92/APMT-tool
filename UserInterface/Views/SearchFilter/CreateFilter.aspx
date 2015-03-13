<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Create Filter
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <script type="text/javascript">

        $(function () {
            $('#filterTab').show('blind', 700);
            $("#createFilters").bind("submit", function () { return false; });
            $('#__1').hide();
            ClearFilter();
            AddFilter();
            $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
            $("#clearButton").attr('disabled', true).addClass('ui-state-disabled');
        });
        $('#CreateFilters').validate();
        $("#addButton").click(AddFilter);
        $('#searchOptions').change(function () {
            ClearFilter();
            AddFilter();
            $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
            $("#clearButton").attr('disabled', true).addClass('ui-state-disabled');
        });

        function AddFilter() {
            AddRow();           
            SeachByField();
        }

        function AddRow() {
            var n = $('#Table2 tr').length;
            $("#removeButton").attr('disabled', false).removeClass('ui-state-disabled');
            $("#clearButton").attr('disabled', false).removeClass('ui-state-disabled');
            $('#Table2 tr:last').after(
            '<tr><td class="searchBytable" id="searchBy' + n + '"></td>' +
            '<td class="attributetable" id="attribute' + n + '"></td>' +
            '<td class="valuetable" id="value' + n + '"></td>' +
            '<td class="matchtable" id="match' + n + '"></td>' +
            '<td class="operatortable" id="operator' + n + '"></td></tr>');
        }

        function SeachByField() {
            var n = $('#Table2 tr').length;
            n--;
            var id = $('#searchOptions').val();
            var field12 = $('#searchOptions');
            var searchby = n + 100;
            $("#searchBy" + n + "").load("SearchBy", { pSelectedID: id, pSearchByRowID: n }, function () {
                var searchbyfield = $('#_' + searchby + ' *:first');
                $('#_' + searchby).width(150);
                AttributeField(searchbyfield.text(), n)
                $("#_" + searchby + "").change(function () {
                    searchbyfield = $('#_' + searchby + ' :selected');
                    AttributeField(searchbyfield.text(), n)
                });
            });
            $("#operator" + n + "").load("Operator", { pOperatorRowID: n });
        }
        
        function AttributeField(pSearchBy, n) {
            var attribute = n + 200;
            $("#attribute" + n + "").load("Attributes", { pSelected: pSearchBy, pAttributeRowID: n }, function () {
                $('#_' + attribute).width(150);
                var attributeField = $('#_' + attribute + ' *:first');
                ValueField(pSearchBy, attributeField.text(), n);
                $("#_" + attribute + "").change(function () {
                    attributeField = $('#_' + attribute + ' :selected');
                    ValueField(pSearchBy, attributeField.text(), n);
                });
            });
        }

        function ValueField(pSearchBy, pAttribute, n) {
            var value = n + 300;
            var match = n + 500;
            $("#value" + n + "").load("ValueField", { pSelected: pSearchBy, pSelectedAttribute: pAttribute, pValueFieldRowID: n }, function () {
                if ($('#_' + value + '').attr('type') != "checkbox") {
                    $('#_' + value + '').width("35%");
                }
                else {
                    $('#_' + value + '').width("15px");
                }
                $('#__' + value).hide();
            });
            $("#match" + n + "").load("Match", { pSelectedAttribute: pAttribute, pMatchRowID: n }, function () {
                $('#_' + match).width(100);
            });
        }

        function ClearFilter() {
            var n = $('#Table2 tr').length;
            while (n > 1) {
                $('#Table2 tr:last').remove();
                $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
                n--;
            }
        }

        $("#removeButton").click(function () {
            var n = $('#Table2 tr').length;
            if (n > 2) {
                $('#Table2 tr:last').remove();
                if (n == 2) {
                    $("#clearButton").attr('disabled', true).addClass('ui-state-disabled');
                    $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
                }
            }
            if (n == 3) {
                $("#clearButton").attr('disabled', true).addClass('ui-state-disabled');
                $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
            }
        });

        $("#clearButton").click(function () {
            var n = $('#Table2 tr').length;
            while (n > 2) {
                $('#Table2 tr:last').remove();
                if (n == 3) {
                    $("#removeButton").attr('disabled', true).addClass('ui-state-disabled');
                    $("#clearButton").attr('disabled', true).addClass('ui-state-disabled');
                }
                n--;
            }
        });

        function Validation(a) {
            var n = $('#Table2 tr').length;
            n += 300;
            var count = 0,flag=0;
            var value = "";
            var regExp = "";
            var errMsg = "";
            jQuery.ajaxSetup({ async: false });
            value = $('#_1').val();
            regExp = $('#RegExp1').val();
            errMsg = $('#ErrMsg1').val();
            if (a==1) {
                $.get('<%: Url.Action("Validation", "SearchFilter")  %>', { pValue: value, pRegExp: regExp, pErrMsg: errMsg, pValueID: 1 }, function (data) {
                    var array = data.split(",");
                    var j = array[0];
                    $('#__' + j).text(array[1]);
                    if (array[1] == "No") {
                        $('#__' + j).hide();
                    } else {
                        count++;
                        $('#__' + j).show();
                    }
                });
            }
            var k = 301;
            for (var i = 301; i < n; i++) {
                value = $('#_' + i).val();
                regExp = $('#RegExp' + i).val();
                errMsg = $('#ErrMsg' + i).val();
                $.get('<%: Url.Action("Validation", "SearchFilter")  %>', { pValue: value, pRegExp: regExp, pErrMsg: errMsg, pValueID: i }, function (data) {
                    var array = data.split(",");
                    var j = array[0];
                    k++;
                    $('#__' + j).text(array[1]);
                    if (array[1] == "No") {
                        $('#__' + j).hide();
                    } else {
                        count++;
                        $('#__' + j).show();
                    }
                });
            }
            jQuery.ajaxSetup({ async: true });
            if (count == 0)
                return true;
            else
                return false;
        }
        $('#testButton').click(function () {
            $("#_1").removeClass('required');
            Validation(0);
            if ($('#createFilters').validate().form()) {
                if (Validation(0)) {
                    var searchField = ($('#searchOptions :selected')).text();
                    var n = $('#Table2 tr').length;
                    var searchByField = "";
                    var attributeField = "";
                    var valueField = "";
                    var operatorField = "";
                    var matchField = "";
                    var typeField = "";
                    n--;

                    var temp = 0;
                    for (var i = 1; i <= n; i++) {

                        temp = i + 100;
                        var field = $('#_' + temp + ' :selected');
                        searchByField += field.text() + ";";

                        temp = i + 200;
                        var field1 = $('#_' + temp + ' :selected');
                        attributeField += field1.text() + ";";

                        temp = i + 300;
                        var field2 = $('#_' + temp + '');
                        typeField += TypeChecking(field2);
                        valueField += ValueChecking(field2);

                        temp = i + 400;
                        var field3 = $('#_' + temp + ' :selected');
                        operatorField += field3.text() + ";";

                        temp = i + 500;
                        var field4 = $('#_' + temp + ' :selected');
                        matchField += field4.text() + ";";
                    }
                    $("#result").load("TestFilter", { pSearch: searchField, pSearchBy: searchByField, pAttribute: attributeField, pValue: valueField, pOperator: operatorField, pMatch: matchField, pValueType: typeField }, function () {
                        $("#details").dialog({
                            autoOpen: false,
                            height: 'auto',
                            width: 800,
                            modal: true,
                            draggable: false,
                            position: 'center',
                            resizable: false,
                            title: 'Details',
                            show: 'clip'
                        });

                        $("#result a[name='dAjaxLink']").click(function () {
                            var link = $(this).attr('href');
                            $("#details").load(link, function () { $("#details").dialog('open'); });
                            return false;
                        });
                    });
                }
            }
        });

        $('#saveButton').click(function () {
            $("#_1").addClass('required');
            Validation(1)
            if ($('#createFilters').validate().form()) {
                if (Validation(1)) {
                    var filterName = $('#_1').val();
                    var searchField = ($('#searchOptions :selected')).text();
                    var n = $('#Table2 tr').length;
                    var searchByField = "";
                    var attributeField = "";
                    var valueField = "";
                    var operatorField = "";
                    var matchField = "";
                    var typeField = "";
                    n--;

                    var temp = 0;
                    for (var i = 1; i <= n; i++) {

                        temp = i + 100;
                        var field = $('#_' + temp + ' :selected');
                        searchByField += field.text() + ";";

                        temp = i + 200;
                        var field1 = $('#_' + temp + ' :selected');
                        attributeField += field1.text() + ";";

                        temp = i + 300;
                        var field2 = $('#_' + temp + '');
                        typeField += TypeChecking(field2);
                        valueField += ValueChecking(field2);

                        temp = i + 400;
                        var field3 = $('#_' + temp + ' :selected');
                        operatorField += field3.text() + ";";

                        temp = i + 500;
                        var field4 = $('#_' + temp + ' :selected');
                        matchField += field4.text() + ";";
                    }
                    $.get('<%: Url.Action("IsFilterNameExist", "SearchFilter")  %>', { pFilterName: filterName }, function (data) {
                        if (data == "True") {
                            showError("<span style='color: Red;'>Search Filter Name '" + filterName + "' already exists.</span>");

                        }
                        else {
                            $.get('<%: Url.Action("SaveFilter", "SearchFilter")  %>', { pFilterName: filterName, pSearch: searchField, pSearchBy: searchByField, pAttribute: attributeField, pValue: valueField, pOperator: operatorField, pMatch: matchField, pValueType: typeField }, function (data) {
                                if (data == "True") {
                                    jqDialog.close();
                                    showNotification("<span>Search Filter Saved.</span>");
                                }
                                else {
                                    showError("<span style='color: Red;'>Search Filter not saved.</span>");
                                }
                            });
                        }
                    });
                    $("#result").load("TestFilter", { pSearch: searchField, pSearchBy: searchByField, pAttribute: attributeField, pValue: valueField, pOperator: operatorField, pMatch: matchField, pValueType: typeField }, function () {
                        $("#details").dialog({
                            autoOpen: false,
                            height: 'auto',
                            width: 800,
                            modal: true,
                            draggable: false,
                            position: 'center',
                            resizable: false,
                            title: 'Details',
                            show: 'clip'
                        });

                        $("#result a[name='dAjaxLink']").click(function () {
                            var link = $(this).attr('href');
                            $("#details").load(link, function () { $("#details").dialog('open'); });
                            return false;
                        });
                    });
                }
            }
        });
        function TypeChecking(field) {
            if (field.attr('type') == "text")
                return "0;"
            else if (field.attr('type') == "textarea")
                return "1;"
            else if (field.attr('type') == "select-one")
                return "2;";
            return "3;";
        }

        function ValueChecking(field) {

            if (field.attr('type') == "text")
                return field.val() + ";"
            else if (field.attr('type') == "textarea")
                return field.val() + ";"
            else if (field.attr('type') == "select-one") {
                return field.val() + ";"
            }
            if (field.attr('checked') == true) {
                return "Yes;";
            }
            else {
                return "No;";
            }
        }
        
    </script>
    <br />
    <div id="filterTab">
        <% using (Html.BeginForm("CreateFilter", "SearchFilter", FormMethod.Post, new { id = "createFilters" }))
           { %>
        <table id="Table0" class="bodyTable">
            <tr>
                <td class="labelPortion">
                    <b>Filter Name</b>
                </td>
                <td class="controlPortion">
                    <%: Html.RenderControl(new ControlInfo ()
                                                                {
                                                                    ControlID =1,
                                                                    DefaultValue = "",
                                                                    CanNull = false,
                                                                    RegularExpression = @"^[a-zA-Z0-9\s]+$",
                                                                    ErrorMessage = "Only alphanumeric is allowed.",
                                                                    Type = (ControlType)1,
                                                                    Value = (TempData["__" + 1] != null)? TempData["__" + 1].ToString() : null,
                                                                    ControlAttName = "filterName",
                                                                    RegExpressionID = -1
                                                                }, false
                                                             )
                    %>
                    <%// Display the error message if any.%> 
                    <span class="errorMessage" id="__1"></span>
                    <%: Html.Hidden("RegularExpression", @"^[a-zA-Z0-9\s]+$", new { id = "RegExp1" })%>
                    <%: Html.Hidden("ErrorMessage", "Only alphanumeric is allowed.", new { id = "ErrMsg1" })%>
                </td>
            </tr>
            <tr>
                <td class="labelPortion">
                    <b>Filter for</b>
                </td>
                <td class="controlPortion">
                    <%: Html.DropDownList("searchOptions", (IEnumerable<SelectListItem>)ViewData["searchOptions"], new { id = "searchOptions"})%>
                </td>
            </tr>
        </table>
        <br />
        <table id="Table1" class="bodyTable">
            <tr>
                <td class="searchBy">
                    <b>SearchBy</b>
                </td>
                <td class="attribute">
                    <b>Attribute</b>
                </td>
                <td class="value">
                    <b>Value</b>
                </td>
                <td class="match">
                    <b>Match</b>
                </td>
                <td class="operator">
                    <b>Operator</b>
                </td>
            </tr>
        </table>
        <asp:Panel ID="Panel1" runat="server" class="panelFilter">
            <table id="Table2" class="bodyTable">
                <tr>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <div id="buttons" class="filterButtonsDiv">
            <input id="saveButton" class="filterButtonDesign" type="button" value="Save" />
            <input id="testButton" class="filterButtonDesign" type="button" value="Test" />
            <input id="addButton" class="filterButtonDesign" type="button" value="Add" />
            <input id="removeButton" class="filterButtonDesign" type="button" value="Remove" />
            <input id="clearButton" class="filterButtonDesign" type="button" value="Clear" />
            <%} %>
        </div>
        <br />
    </div>
    <div id="result">
    </div>
    <div id="details">
    </div>
</asp:Content>
