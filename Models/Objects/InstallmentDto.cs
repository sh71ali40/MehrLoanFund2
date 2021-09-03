using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Objects
{
    public class InstallmentDto
    {
        public int Id { get; set; }

        public long Price
        {
            get
            {
                return MonthlyPayment + MemberShipFee;
            }
        }

        public long MonthlyPayment { get; set; }
        public long MemberShipFee { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PersianPaymentDate
        {
            get
            {
                if (PaymentDate == null)
                {
                    return null;
                }
                var pc = new PersianCalendar();
                return $"{pc.GetYear(PaymentDate.Value)}/{pc.GetMonth(PaymentDate.Value)}/{pc.GetDayOfMonth(PaymentDate.Value)}";
            }
        }
        public int LoanId { get; set; }
    }
}
