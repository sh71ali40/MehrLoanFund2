using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Objects
{
    public class PaymentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "فیلد مبلغ پرداختی اجباری است")]
        public long? Amount { get; set; }
        [Required(ErrorMessage = "فیلد تاریخ پرداخت اجباری است")]
        public DateTime? PaymentDate { get; set; }
        public DateTime SubmitDateTime { get; set; }

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
        public string PersianSubmitDateTime
        {
            get
            {
                if (SubmitDateTime == null)
                {
                    return null;
                }
                var pc = new PersianCalendar();
                return $"{pc.GetYear(SubmitDateTime)}/{pc.GetMonth(SubmitDateTime)}/{pc.GetDayOfMonth(SubmitDateTime)}";
            }
        }
        public int UserId { get; set; }
    }
}
