
using EventExtension.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventExtension.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {   
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetAllEvents();
            if (events == null)
            {
                return NotFound("No events found.");
            }
            return Ok(events); 
        }

        [HttpDelete("RemoveEventsRangeWithId/{id}/{id2}")]
        public async Task<IActionResult> DeleteEventsWithinRange(int id, int id2)
        {
            var events = await _eventService.RemoveEventsRangeWithId(id, id2);
            if(events == null)
            {
                return NotFound("No events found in the specified range.");
            }
            return Ok(events);
        }

    }
}
