using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/********************* About this Class **********************
 *                                                           *
 * This class contains some properties of the control rende- *
 * red by the ControlRenderHelper class. Like control ID its *
 * Regular Expression if any etc. This will help controller  *
 * class to retrieve the data from Form collection and will  *
 * aslo help in error handling on server side.               *
 *                                                           *   
 * Note:                                                     *
 * -----                                                     *
 * It has two copies of same lists(4). The reason for doing  * 
 * this is that in some situations we need to render some new*
 * controls keeping the info of previous ones. So, second 4  *
 * lists' set will save info in this situation.              *
 *                                                           *
 *************************************************************/

namespace UserInterface.Models
{
    public static class RenderedControlsInfo
    {
        // First lists' set.
        public static List<int> L1ControlsID = new List<int>();
        public static List<string> L1ControlsRegExpression = new List<string>();
        public static List<string> L1ControlsErrorMessage = new List<string>();
        public static List<bool> L1ControlsCanNullProperty = new List<bool>();

        // Second lists' set.
        public static List<int> L2ControlsID = new List<int>();
        public static List<string> L2ControlsRegExpression = new List<string>();
        public static List<string> L2ControlsErrorMessage = new List<string>();
        public static List<bool> L2ControlsCanNullProperty = new List<bool>();    
    }
}