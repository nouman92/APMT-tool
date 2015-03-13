<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<b>&nbsp Issue Priority</b>&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp
    <%: Html.DropDownList("filterPriority", (IEnumerable<SelectListItem>)ViewData["priorities"], new { id = "filterPriority" })%>
<br />
<br />
<b>&nbsp Issue State</b>&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp
    <%: Html.DropDownList("filterState", (IEnumerable<SelectListItem>)ViewData["states"], new { id = "filterState" })%>
<br />
<br />
<b>&nbsp Issue Type</b>&nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp
    <%: Html.DropDownList("filterType", (IEnumerable<SelectListItem>)ViewData["types"], new { id = "filterType" })%>
<br />
<br />
<b>&nbsp Assignment State</b>&nbsp &nbsp &nbsp
    <%: Html.DropDownList("filterAState", (IEnumerable<SelectListItem>)ViewData["aStates"], new { id = "filterAState" })%>
<br />

