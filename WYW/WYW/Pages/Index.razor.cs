using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using WYW;

namespace WYW.Pages
{
    public partial class Index
    {
        [Inject]
        ILogger<inputModel> Logger {get; set;}
        private inputModel inputfdModel = new inputModel();
        [Inject]
        private ApiResponseService ApiService { get; set; }
        private string testApiString = "";

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");
            testApiString = ApiService.RecentResponse.LastResponse[0].airline.name;
        }
    }
}