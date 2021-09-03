using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Components
{
    public enum ConfirmButton
    {
        Yes,
        No,
        Cancel
    }
    
    public class ConfirmService
    {
        public event Action<string,string,Action,Action> OnShow;
        public event Action OnHide;

        public void Show(string title, string message,Action onYesClick,Action onNoClick)
        {
            OnShow?.Invoke(title, message,onYesClick,onNoClick);
        }

        //private void Hide( )
        //{
        //    OnHide?.Invoke();
        //}

    }
}
