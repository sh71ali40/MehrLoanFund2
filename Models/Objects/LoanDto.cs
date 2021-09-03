using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Objects
{
    public class LoanDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "فیلد پیش پرداخت اجباری است")]
        public long? PrePayment { get; set; }
        
        public long? MonthlyPayment { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorFullName { get; set; }
        public long? LoanAmount { get; set; }
        public DateTime? LoanPaymentDate { get; set; }
        public string PersianLoanPaymentDate
        {
            get
            {
                if (LoanPaymentDate == null)
                {
                    return null;
                }
                var pc = new PersianCalendar();
                return $"{pc.GetYear(LoanPaymentDate.Value)}/{pc.GetMonth(LoanPaymentDate.Value)}/{pc.GetDayOfMonth(LoanPaymentDate.Value)}";
            }
        }

        public int? LoanAssignerUser { get; set; }
        public string LoanAssignerFullName { get; set; }
        public int UserId { get; set; }
        public bool IsLoanAssigned { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "فیلد حق عضویت اجباری است")]
        public long? MemberShipFee { get; set; }
        [Required(ErrorMessage = "فیلد تعداد اقساط اجباری است")]
        public int? NumberOfInstallment{ get; set; }

        public bool IsFinished { get; set; }
    }
}
