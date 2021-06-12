using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace WYW.Pages
{
    public partial class Index
    {
    [Inject]
    ILogger<inputModel> Logger {get; set;}
    private inputModel inputfdModel = new inputModel();

        private void HandleValidSubmit()
            {
            Logger.LogInformation("HandleValidSubmit called");

            // Process the valid form
        }
    }
}