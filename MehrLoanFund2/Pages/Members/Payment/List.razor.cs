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

namespace MehrLoanFund2.Pages.Members.Payment
{
    public partial class List
    {
        [Parameter] public int MemberId { get; set; }
        [Inject] public IUserServcie UserServcie { get; set; }

        [Inject] public IPaymentService PaymentService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] public ConfirmService ConfirmService { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        public ObservableCollection<PaymentDto> Payments { get; set; }
        public UserDto Member { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Member = await UserServcie.GetById(MemberId);
            if (Member == null)
            {
                return;
            }
            Payments = new ObservableCollection<PaymentDto>(await PaymentService.GetAllByMemberId(MemberId));

        }

        private async Task OnRemoveClick(GridCommandEventArgs e)
        {
            var payment = (PaymentDto)e.Item;
            ConfirmService.Show("حذف", "آیا از حذف مطمئن هستید؟", () => RemovePayment(payment), null);
        }
        private async void RemovePayment(PaymentDto payment)
        {
            try
            {
                var result = await PaymentService.Remove(payment);

                if (result)
                {
                    Payments.Remove(payment);
                    ToastService.ShowToast("حذف با موفقیت انجام شد", ToastLevel.Success);
                }
                else
                {
                    ToastService.ShowToast("حذف ناموفق بود", ToastLevel.Error);
                }
            }
            catch (Exception e)
            {
                ToastService.ShowToast("خطایی در حذف بوجود آمد، لطفا محددا امتحان نمایید", ToastLevel.Error);
            }

        }
    }
}
