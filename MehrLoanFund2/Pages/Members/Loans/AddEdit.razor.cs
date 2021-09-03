using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Models.Objects;
using Services.Components;
using Services.Services;

namespace MehrLoanFund2.Pages.Members.Loans
{
    public partial class AddEdit
    {
        public LoanDto LoanDto { get; set; } = new LoanDto();
        public string ErrorMessage { get; set; }
        
        [Parameter] public int MemberId { get; set; }
        [Parameter] public int? LoanId { get; set; }

        [Inject] private LoadingService LoadingServce { get; set; }
        [Inject] public ILoanService LoanService { get; set; }
        [Inject] private ToastService ToastService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (LoanId != null)
                {
                    var loanDto = await LoanService.GetById(LoanId.Value);
                    
                    if (LoanDto!=null)
                    {
                        if (loanDto.UserId != MemberId)
                        {
                            ErrorMessage = "این وام به این کاربر تعلق ندارد";
                            return;
                        }
                        LoanDto = loanDto;
                    }
                    else
                    {
                        ErrorMessage = "وام پیدا نشد. مجددا امتحان نمایید";
                    }
                }
            }
            catch (Exception e)
            {
                ToastService.ShowToast("در دریافت اطلاعات خطایی رخ داد", ToastLevel.Error);
            }
            
        }

        private async Task SubmitForm()
        {
            try
            {
                if (LoanDto.IsLoanAssigned && (LoanDto.LoanAmount == null || LoanDto.MonthlyPayment == null || LoanDto.LoanPaymentDate == null))
                {
                    ErrorMessage = "در صورت پرداخت وام باید فیلدهای مبلغ وام، قسط و تاریخ پرداخت وام را وارد نمایید";
                    return;
                }

                LoadingServce.Show();
                

                LoanDto.UserId = MemberId;
                var addEditResult =await LoanService.AddEdit(LoanDto, LoanId == null);
                if (addEditResult!=null)
                {
                    ToastService.ShowToast("ثبت با موفقیت انجام شد", ToastLevel.Success);
                    LoadingServce.Hide();
                    NavigationManager.NavigateTo($"/Members/Detail/{MemberId}");
                }
                else
                {
                    ToastService.ShowToast("ثبت ناموفق بود، لطفا مجددا امتحان کنید", ToastLevel.Error);
                    LoadingServce.Hide();
                }
            }
            catch (Exception e)
            {
                ToastService.ShowToast("خطایی در ثبت رخ داد، مجددا امتحان کنید", ToastLevel.Error);
                LoadingServce.Hide();
            }
        }
    }
}
