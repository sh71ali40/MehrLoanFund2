using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MehrLoanFund2.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Models.Objects;
using Services.Components;
using Services.Services;

namespace MehrLoanFund2.Pages
{
    public partial class Signin
    {
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IUserServcie UserServcie { get; set; }
        [Inject] private LoadingService LoadingServce { get; set; }
        [Inject] private ToastService ToastService { get; set; }
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        public LoginDTO LoginDto { get; set; } = new LoginDTO();
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/Home");
            }
        }

        private async Task SubmitForm()
        {
            try
            {
                LoadingServce.Show();
               
                await Task.Delay(3000);
                var user = await UserServcie.GetByUserNamePassword(LoginDto.UserName, LoginDto.Password);
                if (user != null)
                {
                    var interop = new Interop(JSRuntime);


                    string antiforgerytoken = await interop.GetElementByName("__RequestVerificationToken");
                    var fields = new { __RequestVerificationToken = antiforgerytoken, username = LoginDto.UserName, password = LoginDto.Password, returnurl = "/home" };
                    await interop.SubmitForm($"/Identity/signin", fields);
                    LoadingServce.Hide();
                }
                else
                {
                    // user not exsits
                    ErrorMessage = "کاربری با این مشخصات یافت نشد";
                    LoadingServce.Hide();
                }
            }
            catch (Exception e)
            {
                ToastService.ShowToast("خطایی در فراخوانی رخ داد، لطفا مجددا بررسی نمایید",ToastLevel.Error);
                LoadingServce.Hide();
            }
            
        }

    }
}
