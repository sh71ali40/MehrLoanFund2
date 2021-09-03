using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Models.Objects;
using Services.Components;
using Services.Services;

namespace MehrLoanFund2.Pages.Members.Payment
{
    public partial class AddEdit
    {
        
        [Parameter] public int MemberId { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentDto Payment { get; set; } = new PaymentDto();
        [Inject] private LoadingService loadingService { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        [Inject] public IPaymentService PaymentService { get; set; }
        [Inject] public ILoanService LoanService { get; set; }
        [Inject] public ConfirmService ConfirmService { get; set; }
        private async Task SubmitForm()
        {
            try
            {

                loadingService.Show();
                var loans = (await LoanService.GetByMemberId(MemberId)).Where(l=>!l.IsFinished);

                if (!loans.Any())
                {
                    ErrorMessage = $"هیچ وامی برای این شخص ثبت نشده است";
                    loadingService.Hide();
                    return;
                }
                var totalPayments = loans.Sum(t => t.MemberShipFee.Value + (t.IsLoanAssigned ? t.MonthlyPayment.Value : 0));

                if (totalPayments != Payment.Amount)
                {
                    ErrorMessage = $"مبلغ پرداختی برای اقساط این شخص باید برابر با {totalPayments:N0} باشد";
                    loadingService.Hide();
                    return;
                }

                var confirmMessage = CreateMessage(loans);
                ConfirmService.Show("آیا مبلغ بصورت صحیح بین وام ها تقسیم شده است؟", confirmMessage, () => AddNewPayment(Payment,loans), null);
 
                loadingService.Hide();
            }
            catch (Exception e)
            {
                ToastService.ShowToast("خطایی در ثبت رخ داد، مجددا امتحان کنید", ToastLevel.Error);
                loadingService.Hide();

            }
        }

        private async void AddNewPayment(PaymentDto payment,IEnumerable<LoanDto> loans)
        {
            loadingService.Show();
            payment.UserId = MemberId;
            payment.SubmitDateTime = DateTime.Now;
            var addEditResult = await PaymentService.AddNew(payment,loans);
            if (addEditResult != null)
            {
                ToastService.ShowToast("ثبت با موفقیت انجام شد", ToastLevel.Success);
                loadingService.Hide();
                //NavigationManager.NavigateTo($"members/Loan/Installments/{LoanId}");
            }
            else
            {
                ToastService.ShowToast("ثبت ناموفق بود، لطفا مجددا امتحان کنید", ToastLevel.Error);
                loadingService.Hide();
            }
        }

        private string CreateMessage(IEnumerable<LoanDto> loans)
        {
            var loanInstallmentList = new List<dynamic>();
            var confirmMessage = @" 
                                    <table class='table table-hover'>
                                         <thead>
                                            <th scope='col'>مبلغ وام</th>                                            
                                            <th scope='col'>پیش پرداخت</th>
                                            <th scope='col'>قسط</th>
                                            <th scope='col'>حق عضویت</th>
                                            
                                            <th scope='col'>مبلغ تعیین شده برای قسط</th>
                                        </thead>
                                        <tbody>
                                            {0}
                                        </tbody>
                                    </table>";
            var tableBodyMessage = "";

            foreach (var loanDto in loans)
            {
                loanInstallmentList.Add(new { LoanPrePayment = loanDto.PrePayment, LoanMemberShipFee = loanDto.MemberShipFee, LoanAmount = loanDto.LoanAmount, InstallmentAmount = !loanDto.IsLoanAssigned ? loanDto.MemberShipFee.Value : (loanDto.MemberShipFee.Value + loanDto.MonthlyPayment.Value) });

                var loanAmount = loanDto.LoanAmount != null ? loanDto.LoanAmount.Value : 0;

                var installmentAmount = !loanDto.IsLoanAssigned
                    ? loanDto.MemberShipFee.Value
                    : loanDto.MemberShipFee.Value + loanDto.MonthlyPayment.Value;

                
                tableBodyMessage +=
                    $"<tr><td>{loanAmount:N0}</td><td>{loanDto.PrePayment:N0}</td><td>{(loanDto.MonthlyPayment??0):N0}</td><td>{loanDto.MemberShipFee:N0}</td><td>{installmentAmount:N0}</td></tr>";
            }

            return string.Format(confirmMessage, tableBodyMessage);
        }
    }
}
