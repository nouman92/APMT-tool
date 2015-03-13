using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserInterface.HtmlHelpers;

/********************* About this Class **********************
 *                                                           *
 * This class is for data trasfer purpose between View and   *
 * ControlHper class. It contains all the required info of   *
 * the control being rendered as you can guess from the      *
 * properties.                                               *
 *                                                           *
 *************************************************************/

namespace UserInterface.Models
{
    public class ControlInfo
    {
        public int ControlID { get; set; }
        public string DefaultValue { get; set; }
        public bool CanNull { get; set; }
        public string RegularExpression { get; set; }
        public string ErrorMessage { get; set; }
        public ControlType Type { get; set; }
        public string Value { get; set; }
        public string ControlAttName { get; set; }
        public int RegExpressionID { get; set; }
    }
}