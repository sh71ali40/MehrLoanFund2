using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Objects
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required(ErrorMessage = "کد ملی اجباری است")]
        [MaxLength(10,ErrorMessage = "کد ملی باید 10 رقم باشد")]
        [MinLength(10, ErrorMessage = "کد ملی باید 10 رقم باشد")]
        [RegularExpression("^\\d{10}",ErrorMessage = "کد ملی نباید حاوی حروف باشد")]
        public string NationalNum { get; set; }
        [Required(ErrorMessage = "نام اجباری است")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "نام خانوادگی اجباری است")]
        public string LastName { get; set; }
        public string AccountNum { get; set; }
        
        public string CardNum { get; set; }
        public string Sheba { get; set; }
        public bool IsDeleted { get; set; }
    }
}
