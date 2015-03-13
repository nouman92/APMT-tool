<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Tabs.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Domain.Entities.ProjAttribute>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Create Project
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#newProjectAccordion").accordion({
                autoHeight: false
            });
            $('#newProjectTab').show('blind', 800);
        });

        $("#createProject").validate();

    </script>

    <div id="newProjectTab">
        <% using (Html.BeginForm("CreateProject", "Project", FormMethod.Post, new { id = "createProject" }))
            {
        %>
            <div id="newProjectAccordion">
          
                <h3><a href="#"><b> Please provide the following information: </b></a></h3>    
                <div>
                        <table class="bodyTable">
                        <% foreach (var item in Model) { %>
                            <tr>
                                <td class="labelPortion">
                                    <% // If the control is checkbox then don't display the Attribute name as it will be with checkbox. 
                                       if ((ControlType)item.FieldType != ControlType.CheckBox)
                                       {%>
                                            <b><%: item.ProjAttName %></b>
                                    <%} %>
                                </td>
                                <td class="controlPortion">
                                    <%: Html.RenderControl(new ControlInfo ()
                                                                {
                                                                    ControlID = item.ProjAttID,
                                                                    DefaultValue = item.DefaultValue,
                                                                    CanNull = item.CanNull, 
                                                                    // This field can be null. So, if field is null then don't pick the value of expression
                                                                    // from Regular Expression table. 
                                                                    RegularExpression = (item.RegularExpression != null)?  item.RegularExpression1.Value: null,
                                                                    ErrorMessage = (item.RegularExpression != null) ? item.RegularExpression1.Error : "",
                                                                    Type = (ControlType)item.FieldType,
                                                                    Value = (TempData["__" + item.ProjAttID] != null)? TempData["__" + item.ProjAttID].ToString() : null,
                                                                    ControlAttName = item.ProjAttName,
                                                                    RegExpressionID = (item.RegularExpression != null) ? item.RegularExpression1.ExpressionID : -1
                                                                }, true
                                                             )
                                    %>
                                    <% // Display the error message if any. 
                                       if (TempData["_" + item.ProjAttID] != null)
                                       { %>
                                           <span class="errorMessage"> <%: TempData["_" + item.ProjAttID]%> </span>
                                    <% } %>
                                </td>
                            </tr>
                        <% } %>
                        </table>
                        <br />
                        <input class="buttonDesign" type="submit" value="Create" />
                        <a class="linkButton2" href='<%: Url.Action("ProjectTabs") %>'> <span> Cancel </span> </a>
                        <br />
                        <br />
                </div>
                                    
                <h3><a title="You can add risks later as well" href="#"><b> Risk Assessment </b></a></h3>
                <div id="riskAssessment">
                    <% Html.RenderAction("RiskList", "Organization"); %>
                </div>

            </div>
        <%} %>
    </div>
    
</asp:Content>
