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

namespace MehrLoanFund2.Pages.Members
{
    public partial class Members
    {
        [Inject] public IUserServcie UserService { get; set; }
        [Inject] public ConfirmService ConfirmService { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        public ObservableCollection<UserDto> Users { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Users = new ObservableCollection<UserDto>(await UserService.GetAll()) ;
        }

        private async Task OnDetailClick(GridCommandEventArgs e)
        {
            var user = (UserDto)e.Item;
            _navigationManager.NavigateTo($"/members/detail/{user.Id}");
        }
        private async Task OnPaymentClick(GridCommandEventArgs e)
        {
            var user = (UserDto)e.Item;
            _navigationManager.NavigateTo($"/members/payment/{user.Id}");
        }
        private async Task OnEditClick(GridCommandEventArgs e)
        {
            var user = (UserDto)e.Item;
            _navigationManager.NavigateTo($"/Members/AddEdit/{user.Id}");
            
        }

        private async Task OnRemoveClick(GridCommandEventArgs e)
        {
            var user = (UserDto)e.Item;
            ConfirmService.Show("حذف","آیا از حذف مطمئن هستید؟",()=>RemoveUser(user) ,null);
        }
        
        private async void RemoveUser(UserDto user)
        {
            var result = await UserService.Remove(user);
           

            if (result)
            {
                Users.Remove(user);
                ToastService.ShowToast("حذف با موفقیت انجام شد",ToastLevel.Success);
            }
            else
            {
                ToastService.ShowToast("حذف ناموفق بود",ToastLevel.Error);
            }
        }
    }
}
