using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

#nullable disable

namespace DataLayer.Context.Entities
{
    public partial class Loan
    {
        public Loan()
        {
            Installments = new HashSet<Installment>();

        }
        public int Id { get; set; }
        public long PrePayment { get; set; }
        public long? MonthlyPayment { get; set; }
        [DefaultValue(0)]
        public long MemberShipFee { get; set; }
        public DateTime CreateDate { get; set; }

        public string PersianCreateDate
        {
            get
            {
                var pc = new PersianCalendar();
                return $"{pc.GetYear(CreateDate)}/{pc.GetMonth(CreateDate)}/{pc.GetDayOfMonth(CreateDate)}";
            }
        }

        public int CreatedBy { get; set; }
        public string CreatorFullName { get; set; }
        public long? LoanAmount { get; set; }
        public DateTime? LoanPaymentDate { get; set; }
        
        public int? LoanAssignerUser { get; set; }
        public string LoanAssignerFullName { get; set; }
        public int UserId { get; set; }
        [DefaultValue(false)]
        public bool IsLoanAssigned { get; set; }

        [DefaultValue(10)]
        public int NumberOfInstallment { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsFinished { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
    }
}
