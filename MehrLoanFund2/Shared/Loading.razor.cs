using System;
using Microsoft.AspNetCore.Components;
using Services.Components;

namespace MehrLoanFund2.Shared
{
    public partial class Loading : ComponentBase, IDisposable
    {
        [Inject] LoadingService LoadingService { get; set; }

        protected string Message { get; set; }
        protected bool IsVisible { get; set; }

        protected override void OnInitialized()
        {
            LoadingService.OnShow += ShowLoading;
            LoadingService.OnHide += HideLoaing;
        }

        private void ShowLoading(string message)
        {
            SetMessage(message);
            IsVisible = true;
            StateHasChanged();
        }

        private void SetMessage(string message)
        {
            Message = message;
        }
        private void HideLoaing()
        {
            IsVisible = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            LoadingService.OnShow -= ShowLoading;
        }
    }
}
