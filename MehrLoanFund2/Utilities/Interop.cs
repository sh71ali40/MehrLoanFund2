using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MehrLoanFund2.Utilities
{
    public class Interop
    {
        private readonly IJSRuntime _jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public ValueTask<string> GetElementByName(string name)
        {
            try
            {
                return _jsRuntime.InvokeAsync<string>(
                    "Interop.getElementByName",
                    name);
            }
            catch
            {
                return new ValueTask<string>(Task.FromResult(string.Empty));
            }
        }
        public Task SubmitForm(string path, object fields)
        {
            try
            {
                _jsRuntime.InvokeVoidAsync(
                    "Interop.submitForm",
                    path, fields);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }
    }
}
