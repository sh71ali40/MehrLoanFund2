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
    public interface IInstallmentService : IBaseService<Installment>
    {
        public Task<InstallmentDto> GetById(int installmentId);
        public Task<InstallmentDto> AddEdit(InstallmentDto installmentDto, bool isAdd);
        public Task<IEnumerable<InstallmentDto>> GetByLoanId(int loanId);
        public Task<bool> Remove(InstallmentDto installment);
        public  Task<bool> RemoveByPaymentId(int paymentId);
    }
    public class InstallmentService :BaseService<Installment>, IInstallmentService
    {
        private readonly ILogger<InstallmentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DbSet<Installment> _installmentDbSet;
        private readonly IUserServcie _userService;
        private readonly ILoanService _loanService;

        public InstallmentService(IUnitOfWork unitOfWork, ILoanService loanService
            , IMapper mapper, ILogger<InstallmentService> logger,
             IUserServcie userService) : base(unitOfWork,logger)
        {
            _unitOfWork = unitOfWork;
            _installmentDbSet = unitOfWork.Set<Installment>();
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _loanService = loanService;
        }
        public async Task<InstallmentDto> GetById(int installmentId)
        {
            try
            {
                var installment = await _installmentDbSet.FindAsync(installmentId);
                return _mapper.Map<Installment, InstallmentDto>(installment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<InstallmentDto> AddEdit(InstallmentDto installmentDto, bool isAdd)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                if (isAdd)
                {
                    
                    
                    var installment = _mapper.Map<InstallmentDto, Installment>(installmentDto);
                    installment.CreatedDate = DateTime.Now;
                    installment.CreatedBy = currentUser.Id;
                    
                    var addResult = await _installmentDbSet.AddAsync(installment);
                    await _unitOfWork.SaveChangesAsync();

                    var installmentLoan = await _loanService.GetById(installmentDto.LoanId);
                    var loanInstallments = await GetByLoanId(installmentDto.LoanId);
                    if (installmentLoan.NumberOfInstallment == loanInstallments.Count() + 1)
                    {
                        installmentLoan.IsFinished = true;
                        await _loanService.AddEdit(installmentLoan, false);
                    }

                    return _mapper.Map<Installment, InstallmentDto>(installment);


                }
                else
                {
                    var editedInstallment = await _installmentDbSet.FindAsync(installmentDto.Id);
                    var installment = _mapper.Map<InstallmentDto, Installment>(installmentDto, editedInstallment);
                    var editResult = _installmentDbSet.Update(installment);
                    await _unitOfWork.SaveChangesAsync();
                    return _mapper.Map<Installment, InstallmentDto>(editResult.Entity);

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

        }

        public async Task<IEnumerable<InstallmentDto>> GetByLoanId(int loanId)
        {
            try
            {
                var installments = _installmentDbSet.Where(i => i.LoanId == loanId);
                return _mapper.Map<IEnumerable<Installment>, IEnumerable<InstallmentDto>>(installments);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<bool> Remove(InstallmentDto installment)
        {
            try
            {
                var ins = _mapper.Map<InstallmentDto, Installment>(installment);
                _installmentDbSet.Remove(ins);
                await  _unitOfWork.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<bool> RemoveByPaymentId(int paymentId)
        {
            try
            {
                var installments = _installmentDbSet.Where(i => i.PaymentId == paymentId);
                _installmentDbSet.RemoveRange(installments);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                throw;
            }
        }
    }
}
