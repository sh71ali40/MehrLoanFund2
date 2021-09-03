using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Models.Objects;
using Services.Components;
using Services.Services;

namespace MehrLoanFund2.Pages.Members.Loans.Installment
{
    public partial class AddEdit
    {
        [Parameter] public int? InstallmentId { get; set; }
        [Parameter] public int LoanId { get; set; }

        public InstallmentDto InstallmentDto { get; set; } = new InstallmentDto();
        public string ErrorMessage { get; set; } = "";

        [Inject] private IInstallmentService InstallmentService { get; set; }
        [Inject] private LoadingService LoadingServce { get; set; }
        [Inject] private ToastService ToastService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (InstallmentId!=null)
                {
                    var installmentDto =await InstallmentService.GetById(InstallmentId.Value);

                    if (installmentDto!=null)
                    {
                        InstallmentDto = installmentDto;
                    }
                    else
                    {
                        ErrorMessage = "قسط پیدا نشد. مجددا امتحان نمایید";
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
                LoadingServce.Show();
                InstallmentDto.LoanId= LoanId;
                var addEditResult = await InstallmentService.AddEdit(InstallmentDto, InstallmentId == null);
                if (addEditResult != null)
                {
                    ToastService.ShowToast("ثبت با موفقیت انجام شد", ToastLevel.Success);
                    LoadingServce.Hide();
                    NavigationManager.NavigateTo($"members/Loan/Installments/{LoanId}");
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
