<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.IssueWorkFlow>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Issue Work Flow
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

	<style>

	    #states { list-style-type: none; margin: 0; padding: 0;}
	    #states li { margin: 8px 8px 8px 8px; padding: 0.5em; padding-left: 1.5em; font-size: 14px; height: 18px; font-weight:bold; }
    	#states li span { position: absolute; margin-left: -1.3em; }
    	#states #cBox { width:62%;float:left;padding-left:1.5em;font-size:14px; }
        #statesBox {border: solid 3px;}

	</style>

	<script type="text/javascript">

	    $(function () {
	        $("#states").sortable({
	            axis: 'y',
	            cursor: 'Move',
	            opacity: 0.7,
	            revert: true,
                containment: '#statesBox'
    	    });
	        $('#workFlowTab').show('blind', 700);
	    });

	    $("#addStateLink").click(function () {
	        var validator = $("#addIssueState").validate();
	        validator.resetForm();
	        $("#_0").attr("value", "");
	        $("#addState").dialog('open');
	        return false;
	    });

	    $("#addState").dialog({
	        autoOpen: false,
	        height: 'auto',
	        width: 700,
	        modal: true,
	        draggable: false,
	        position: 'center',
	        resizable: false,
	        title: 'Add Issue State',
            show: 'clip'
	    });

	    $("#renameStateLink").click(function () {
	        $("#renameState").load('<%: Url.Action("RenameState", "Issue")  %>', function () { $(this).dialog('open'); });
	        return false;
	    });

	    $("#renameState").dialog({
	        autoOpen: false,
	        height: 'auto',
	        width: 760,
	        modal: true,
	        draggable: false,
	        position: 'center',
	        resizable: false,
	        title: 'Rename Issue State',
            show: 'clip'
	    });

        $("#moreLink").click(function () {
            $("#moreDetails").dialog('open');
            return false;
        });

        $("#moreDetails").dialog({
            autoOpen: false,
            height: 'auto',
            width: 760,
            modal: true,
            draggable: false,
            position: 'center',
            resizable: false,
            title: 'Issue Work-Flow Details',
            show: 'clip'
        });

        $("#addIssueState").validate();

	</script>

    <div id="workFlowTab">
        <Span style="float:right;" title="Add new state in the work-flow">[ <%: Html.ActionLink("Add State", "#", null, new { id = "addStateLink" })%> ]</Span>
        <Span style="float:right;" title="Rename an existing state">[ <%: Html.ActionLink("Rename State", "#", null, new { id = "renameStateLink" })%> ]&nbsp</Span>
        <br />
        <h5>
            <i>* Each issue will flow top to the bottom from the following states.</i>
            <br />
            <i>* A state can be active or inactive.</i>
            <br />
            <i>* Inactive state will be skipped during the issue work-flow.</i>
            <br />
            <i>* Any issue in the last active state will not be assignable.</i>
            <Span " title="More on Issue Work-Flow">[ <%: Html.ActionLink("more", "#", null, new { id = "moreLink" })%> ]</Span>
            <br />
        </h5>

        <b>Issue States</b><span class="helpSign" title="Drag and drop to change the flow order"><b>?</b></span>
        <br />
        <br />
        <% using (Html.BeginForm("UpdateWorkFlow", "Issue", FormMethod.Post, new { id = "iWorkFlow" }))
           { 
        %>
        <div id="statesBox" style="width:35%;">
            <ul id="states">
                <% foreach (var state in Model)
                   {%>
    	            <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>
                        <span id="cBox" style="font-size:14px;"><%: Html.RenderControl(new ControlInfo()
                                                {
                                                    ControlID = state.StateID,
                                                    DefaultValue = null,
                                                    CanNull = false,
                                                    Type = ControlType.CheckBox,
                                                    Value = (state.Active == true)?"Yes":null,
                                                    ControlAttName = state.StateName
                                                }, false
                                                )
                        %><input type="hidden" value="false" name='_<%:state.StateID %>'/></span>
                    </li>
                <%} %>
            </ul>
        </div>
        <br />
        <input class="buttonDesign" type="submit" value="Update Flow" />
        <br />
        <%} %>

    </div>

    <div id="addState">

        <% using (Html.BeginForm("AddState", "Issue", FormMethod.Post, new { id = "addIssueState" }))
        { %>
            <br />
            <b>State Name</b> &nbsp &nbsp
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
            <input class="buttonDesign" type="submit" value="Add" id="addButton" />
        <% } %>

    </div>

    <div id="renameState">
    </div>

    <div id="moreDetails">
        <br />
        <b>An issue changes its state in the following events:</b>
        <br />
        <ol>
            <li>When an issue is created, it goes in first active state</li>
            <li>When an issue is assigned, it goes to next active state </li>
            <li>When issue work-flow is executed, issue goes to next active state</li>
        </ol>
        <b>Note:</b>
        <br />
        <br />
        <i>&nbsp&nbsp * The last active state means that issue has been resolved.</i>
        <br />
        <i>&nbsp&nbsp * If an issue is assigned to more than one user then on issue work-flow execution,
            <br />&nbsp&nbsp&nbsp&nbsp issue does not change its state but removed from the user assigned list.
        </i>
        <br />
        <br />
    </div>

</asp:Content>

