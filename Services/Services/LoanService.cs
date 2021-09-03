using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Context;
using DataLayer.Context.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Objects;

namespace Services.Services
{
    public interface ILoanService
    {
        public Task<LoanDto> GetById(int loanId);
        public Task<LoanDto> AddEdit(LoanDto loanDto, bool isAdd);
        public Task<IEnumerable<LoanDto>> GetByMemberId(int memberId);
        public Task<bool> Remove(LoanDto loan);
        
    }
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<Loan> _loanDbSet;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IUserServcie _userService;


        public LoanService(IUnitOfWork unitOfWork
            , IMapper mapper, ILogger<UserService> logger, IUserServcie userService)
        {
            _unitOfWork = unitOfWork;
            _loanDbSet = unitOfWork.Set<Loan>();
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
        }
        public async Task<LoanDto> GetById(int loanId)
        {
            try
            {
                var loan = await _loanDbSet.FindAsync(loanId);
                var loanDto = _mapper.Map<Loan, LoanDto>(loan);
                return loanDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<LoanDto> AddEdit(LoanDto loanDto, bool isAdd)
        {

            try
            {
                var currentUser = await _userService.GetCurrentUser();
                if (isAdd)
                {

                    var loan = _mapper.Map<LoanDto, Loan>(loanDto);
                    loan.CreateDate = DateTime.Now;
                    loan.CreatedBy = currentUser.Id;
                    loan.CreatorFullName = $"{currentUser.FirstName} {currentUser.LastName}";
                    var addResult = await _loanDbSet.AddAsync(loan);
                    await _unitOfWork.SaveChangesAsync();
                    return _mapper.Map<Loan, LoanDto>(loan);


                }
                else
                {
                    var editedLoan = await _loanDbSet.FindAsync(loanDto.Id);
                    var loan = _mapper.Map<LoanDto, Loan>(loanDto, editedLoan);
                    var editResult = _loanDbSet.Update(loan);
                    await _unitOfWork.SaveChangesAsync();
                    return _mapper.Map<Loan, LoanDto>(editResult.Entity);

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<LoanDto>> GetByMemberId(int memberId)
        {

            try
            {
                var loans = _loanDbSet.OrderByDescending(l => l.Id).Where(l=>l.UserId==memberId && !l.IsDeleted);
                var loansDtoList = _mapper.Map<IEnumerable<Loan>, IEnumerable<LoanDto>>(loans);
                return loansDtoList;

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<bool> Remove(LoanDto loan)
        {
            try
            {
                loan.IsDeleted = true;
                var deleteResult = await AddEdit(loan, false);
                return deleteResult != null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
