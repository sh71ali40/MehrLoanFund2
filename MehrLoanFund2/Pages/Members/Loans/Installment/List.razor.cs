using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Models.Objects;
using Services.Components;
using Services.Services;
using Telerik.Blazor.Components;

namespace MehrLoanFund2.Pages.Members.Loans.Installment
{
    public partial class List
    {
        [Parameter] public int LoanId { get; set; }
        [Inject] public ILoanService LoanService { get; set; }
        [Inject] public IUserServcie UserServcie { get; set; }
        [Inject] public IInstallmentService InstallmentService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public ConfirmService ConfirmService { get; set; }
        [Inject] public ToastService ToastService { get; set; }


        public UserDto Member { get; set; } = new UserDto();
        public LoanDto LoanDto { get; set; }


        public long TotalPayment { get; set; }
        public long TotalPayedMemeberShipFee { get; set; }
        public long TotalPayedInstallment { get; set; }

        public ObservableCollection<InstallmentDto> Installments { get; set; }
        protected override async Task OnInitializedAsync()
        {
            LoanDto = await LoanService.GetById(LoanId);
            if (LoanDto==null)
            {
                return;
            }
            Member = await UserServcie.GetById(LoanDto.UserId);
            var installments = await InstallmentService.GetByLoanId(LoanId);
            TotalPayedInstallment = installments.Sum(i => i.MonthlyPayment);
            TotalPayedMemeberShipFee = installments.Sum(i => i.MonthlyPayment);
            TotalPayment = installments.Sum(i => i.MonthlyPayment + i.MonthlyPayment);

            Installments = new ObservableCollection<InstallmentDto>(installments);

        }

       

        private async Task OnEditClick(GridCommandEventArgs e)
        {
            var installment = (InstallmentDto)e.Item;
            //NavigationManager.NavigateTo($"Members/Loan/AddEdit/{MemberId}/{loan.Id}");

        }

        private async Task OnRemoveClick(GridCommandEventArgs e)
        {
            var installment = (InstallmentDto)e.Item;
            ConfirmService.Show("حذف", "آیا از حذف مطمئن هستید؟", () => RemoveInstallment(installment), null);
        }

        private async void RemoveInstallment(InstallmentDto installmentDto)
        {
            var result = await InstallmentService.Remove(installmentDto);


            if (result)
            {
                Installments.Remove(installmentDto);
                ToastService.ShowToast("حذف با موفقیت انجام شد", ToastLevel.Success);
            }
            else
            {
                ToastService.ShowToast("حذف ناموفق بود", ToastLevel.Error);
            }
        }
    }
}
