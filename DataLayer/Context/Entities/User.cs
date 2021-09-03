using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace DataLayer.Context.Entities
{
    public partial class User
    {
        public User()
        {
            Loans = new HashSet<Loan>();
            Payments = new HashSet<Payments>();
            UserRoles = new HashSet<UserRole>();
        }
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string Username { get; set; }
        [StringLength(500)]
        [Required]
        public string Password { get; set; }
        public string NationalNum { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountNum { get; set; }
        public string CardNum { get; set; }
        public string Sheba { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdateBy { get; set; }
        public int NumOfFailLogin { get; set; }
        public string UserIp { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
