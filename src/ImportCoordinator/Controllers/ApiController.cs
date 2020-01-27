using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImportCoordinator.Contracts;
using ImportCoordinator.Core;
using ImportCoordinator.Core.Services;

namespace ImportCoordinator.Controllers
{

    public class ApiController : Controller
    {
        public ApiController(EventService eventService, AzureService azureService)
        {
            _eventService = eventService;
            _azureService = azureService;
        }
        
        [HttpPost]
        [Route("/api/event")]
        public async Task<object> SendEvent([FromBody] Event[] eventModel)
        {
            foreach (var _event in eventModel)
            {
                await _eventService.SaveEvent(_event);

                await _eventService.SendMail(_event);
            }

            return null;
        }


        [HttpPost]
        [Route("/api/merge")]
        public async Task Merge()
        {
            await _azureService.RunDatabricks();
        }


        [HttpGet]
        [Route("/api/job_info")]
        public async Task JobInfo()
        {
            await _azureService.GetJobInfo();
        }

        private readonly EventService _eventService;
        private readonly AzureService _azureService;

    }
}