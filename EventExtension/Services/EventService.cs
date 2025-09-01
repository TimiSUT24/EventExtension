using EventClassLibrary.DTO;
using EventClassLibrary.Models;
using EventExtension.Mapper;
using EventExtension.Repositories.Interfaces;
using EventExtension.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventExtension.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private List<EventItemDto> _cachedEvents = new(); 

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventItemDto>> GetAllEvents()
        {
            if (_cachedEvents == null || !_cachedEvents.Any())
            {
                await RefreshEvents();
            }

            return _cachedEvents;
           /* var events = await _eventRepository.GetAllAsync();
            if(events == null || !events.Any())
            {
                throw new Exception("No events found.");
            }
            
            var allEvents = events.Select(e => e.MapEventItemDto()).ToList();
            return allEvents;*/
                  
        }

        public async Task RefreshEvents()
        {
            var events = await _eventRepository.GetAllAsync();
            _cachedEvents = events.Select(e => e.MapEventItemDto()).ToList();
        }

        public async Task<IEnumerable<EventItemDto>> RemoveEventsRangeWithId(int id, int id2)
        {
            var events = await _eventRepository.FindAsync(e => e.Id >= id && e.Id <= id2);
            if (events == null || !events.Any())
            {
                throw new Exception("No events found in the specified range.");
            }
            foreach (var eventItem in events)
            {
                await _eventRepository.DeleteAsync(eventItem);
            }
            await _eventRepository.SaveChangesAsync();
            return events.Select(e => new EventItemDto
            {
                Id = e.Id,
            });

        }

        public async Task UploadEvents(IEnumerable<EventItemDto> events)
        {
            if (events == null || !events.Any())
            {
                throw new ArgumentException("Empty event list.");
            }

            await _eventRepository.RemoveRange();
                     
            var eventEntities = events.Select(e => e.MapEventItem()).ToList();
            
            await _eventRepository.AddRangeAsyncEvents(eventEntities);         
            await _eventRepository.SaveChangesAsync();
           
        }
      
    }
}
