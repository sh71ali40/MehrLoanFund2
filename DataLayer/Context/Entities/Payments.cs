using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Context.Entities
{
    public class Payments
    {
        public Payments()
        {
            Installments = new HashSet<Installment>();

        }
        public int Id { get; set; }
        public long Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AddedBy { get; set; }
        public DateTime SubmitDateTime { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
    }
}
