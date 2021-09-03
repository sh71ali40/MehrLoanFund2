using System;
using System.Collections.Generic;

#nullable disable

namespace DataLayer.Context.Entities
{
    public partial class Installment
    {
        public int Id { get; set; }
        
        public long MemberShipFee { get; set; }
        public long MonthlyPayment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime PaymentDate { get; set; }
        public int LoanId { get; set; }
        public int PaymentId { get; set; }

        public virtual Loan Loan { get; set; }
        public virtual Payments Payment{ get; set; }
    }
}
