using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Objects
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "نام کاربری خود را وارد نمایید")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "رمز خود را وارد نمایید")]
        public string Password { get; set; }
    }
}
