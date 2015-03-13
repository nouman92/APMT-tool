using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace UserInterface.Models
{
    public class NavigationLink
    {
        public string Text { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public bool IsSelected { get; set; }
    }
}