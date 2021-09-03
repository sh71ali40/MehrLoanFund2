using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Context;
using DataLayer.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Objects;

namespace Services.Services
{
    public interface IPaymentService
    {
        public Task<IEnumerable<PaymentDto>> GetAllByMemberId(int memberId);
        public Task<bool> Remove(PaymentDto payment);
        public Task<PaymentDto> AddNew(PaymentDto paymentDto, IEnumerable<LoanDto> loanDtos);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<Payments> _paymentDbSet;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IUserServcie _userService;
        private readonly IInstallmentService _installmentService;

        public PaymentService(IUnitOfWork unitOfWork
            , IMapper mapper, ILogger<PaymentService> logger, IUserServcie userService, IInstallmentService installmentService)
        {
            _unitOfWork = unitOfWork;
            _paymentDbSet = _unitOfWork.Set<Payments>();
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _installmentService = installmentService;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllByMemberId(int memberId)
        {
            try
            {
                var payments = _paymentDbSet.Where(p => p.UserId == memberId).OrderByDescending(p => p.Id);
                var paymentsDto = _mapper.Map<IEnumerable<Payments>, IEnumerable<PaymentDto>>(payments);
                return paymentsDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

        }

        public async Task<bool> Remove(PaymentDto paymentDto)
        {
            try
            {
                var installments = await _installmentService.RemoveByPaymentId(paymentDto.Id);
                if (!installments)
                {
                    return false;
                }

                var payment = _mapper.Map<PaymentDto, Payments>(paymentDto);
                _paymentDbSet.Remove(payment);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<PaymentDto> AddNew(PaymentDto paymentDto, IEnumerable<LoanDto> loanDtos )
        {

            try
            {
                var payment = _mapper.Map<PaymentDto, Payments>(paymentDto);
                foreach (var loanDto in loanDtos)
                {
                    payment.Installments.Add(new Installment()
                    {
                        LoanId = loanDto.Id,
                        CreatedBy = _userService.CurrentUserId,
                        CreatedDate = DateTime.Now,
                        MonthlyPayment = loanDto.MonthlyPayment ?? 0,
                        MemberShipFee = loanDto.MemberShipFee.Value,
                        PaymentDate = payment.PaymentDate,
                        
                       
                    });
                }

               var addResult = await _paymentDbSet.AddAsync(payment);
               await _unitOfWork.SaveChangesAsync();
               return _mapper.Map<Payments, PaymentDto>(payment);


                // 10000.r/ZbQH482ZLKoS5/+qucgw==.kn5YjgMjhafjxhzKyXwfbrLRR7hz7Zawz8Ut1Fr2F2E=
                

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
