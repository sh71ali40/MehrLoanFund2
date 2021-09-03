using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.Objects;
using Services.Components;
using Services.Services;

namespace MehrLoanFund2.Pages.Members
{
    public partial class AddEdit
    {
        [Parameter] public int? MemberId { get; set; }
        [Inject] public IUserServcie UserService { get; set; }
        [Inject] private LoadingService LoadingServce { get; set; }
        [Inject] private ToastService ToastService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        public UserDto UserDto { get; set; } = new UserDto();
        public string ErrorMessage { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (MemberId.HasValue)
                {
                    var userDto = await UserService.GetById(MemberId.Value);
                    if (userDto != null)
                    {
                        UserDto = userDto;
                    }
                    else
                    {
                        ErrorMessage = "کاربری با این مشخصات یافت نشد";
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
                if (MemberId == null)
                {
                    var userWithSameCode = await UserService.GetByNationalCode(UserDto.NationalNum);
                    if (userWithSameCode != null)
                    {
                        ToastService.ShowToast("کد ملی وارد شده تکراری است", ToastLevel.Info);
                        LoadingServce.Hide();
                        return;
                    }
                }

                var userAddEditResult = await UserService.AddEdit(UserDto, MemberId == null);
                if (userAddEditResult != null)
                {
                    ToastService.ShowToast("ثبت با موفقیت انجام شد", ToastLevel.Success);
                    LoadingServce.Hide();
                    NavigationManager.NavigateTo("Members");
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
            }


        }
    }
}
