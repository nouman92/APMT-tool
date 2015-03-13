<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $(function () {
        $("[title]").tooltip({
            open: function () {
                var tooltip = $(this).tooltip("widget");
                $(document).mousemove(function (event) {
                    tooltip.position({
                        my: "left center",
                        at: "right center",
                        offset: "25 25",
                        of: event
                    });
                })
                // trigger once to override element-relative positioning
				.mousemove();
            },
            close: function () {
                $(document).unbind("mousemove");
            }
        });
    });

    $("#_0").keyup(function () {
        if ($('#addDepForm').validate().form()) {
            $.get('<%: Url.Action("GetIssuesIDs", "Issue")  %>', { pSearchID: $("#_0").val() }, function (data) {
                $("#_0").autocomplete({
                    source: data
                });
            });

            $("#issueName").load('<%: Url.Action("IssueName", "Issue")  %>', { pIssueID: $("#_0").val() });
        } else {
            $("#issueName").html("");
        }
    });

    $("#addDepForm").validate();

</script>

<div>
    <Span style="float:right;" title="List all the issues with check boxes for selection">[ <%: Html.ActionLink("Show All", "ShowAllIssues", "Issue", null, new { id = "showAllLink" })%> ]</Span>
    <br />
    <h3>Enter the depends on issue ID.</h3>
    <% using (Html.BeginForm("AddIssueDependency", "Issue", FormMethod.Post, new { id = "addDepForm" }))
        { %>
        <br />
        <b>Issue Name</b>&nbsp &nbsp <span id="issueName"></span>
        <br />
        <br />
        <b>Issue ID</b> &nbsp &nbsp &nbsp  &nbsp &nbsp
        <span><%: Html.RenderControl(new ControlInfo()
                                    {
                                        ControlID = 0,
                                        CanNull = false,
                                        RegularExpression = @"^\d+$",
                                        ErrorMessage = "*Invalid ID",
                                        Type = ControlType.TextField,
                                        Value = null
                                    }, false
                                )
        %></span>
        <br />
        <br />
        <br />
        <input class="buttonDesign" type="submit" value="Add" id="addButton" />
        <input class="buttonDesign2" type="button" value="Cancel" id="cancel6" />
    <% } %>

</div>
