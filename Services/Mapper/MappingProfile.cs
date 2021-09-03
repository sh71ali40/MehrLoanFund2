using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Context.Entities;
using Models.Objects;

namespace Services.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<Loan, LoanDto>();
            CreateMap<LoanDto, Loan>();
            CreateMap<InstallmentDto, Installment>();
            CreateMap<Installment, InstallmentDto>();
            CreateMap<Payments, PaymentDto>();
            CreateMap<PaymentDto, Payments>();

        }
    }
}
