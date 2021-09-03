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

namespace MehrLoanFund2.Pages.Members.Loans
{
    public partial class List
    {
        [Parameter] public int MemberId { get; set; }
        [Inject] public IUserServcie UserServcie { get; set; }
        [Inject] public ILoanService LoanService{ get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public ConfirmService ConfirmService { get; set; }
        [Inject] public ToastService ToastService { get; set; }

        public UserDto Member { get; set; }
        public ObservableCollection<LoanDto> Loans{ get; set; }
        protected override async Task OnInitializedAsync()
        {
            Member = await UserServcie.GetById(MemberId);
            if (Member==null)
            {
                return;
            }

            Loans = new ObservableCollection<LoanDto>(await LoanService.GetByMemberId(MemberId));
        }


        private async Task OnDetailClick(GridCommandEventArgs e)
        {
            var loan = (LoanDto)e.Item;
            NavigationManager.NavigateTo($"/Members/Loan/Installments/{loan.Id}");
            
        }
        private async Task OnEditClick(GridCommandEventArgs e)
        {
            var loan = (LoanDto)e.Item;
            NavigationManager.NavigateTo($"Members/Loan/AddEdit/{MemberId}/{loan.Id}");

        }

        private async Task OnRemoveClick(GridCommandEventArgs e)
        {
            var loan = (LoanDto)e.Item;
            ConfirmService.Show("حذف", "آیا از حذف مطمئن هستید؟", () => RemoveLoan(loan), null);
        }

        private async void RemoveLoan(LoanDto loan)
        {
            var result = await LoanService.Remove(loan);


            if (result)
            {
                Loans.Remove(loan);
                ToastService.ShowToast("حذف با موفقیت انجام شد", ToastLevel.Success);
            }
            else
            {
                ToastService.ShowToast("حذف ناموفق بود", ToastLevel.Error);
            }
        }
    }
}
