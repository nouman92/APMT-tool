<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%: Html.RenderControl(new ControlInfo ()
                                        {
                                            ControlID = (int)ViewData["searchByRowID"]+100,
                                            DefaultValue = (string)ViewData["searchByOptions"],
                                            Type = (ControlType)4,
                                            ControlAttName = "searchBy" + ViewData["searchByRowID"]
                                        }, false
                    )
%>