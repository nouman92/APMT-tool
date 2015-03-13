using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UserInterface.Models
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "Enter your user name")]
        public string UserName
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Enter your password")]
        public string Password
        {
            get;
            set;
        }
    }
}