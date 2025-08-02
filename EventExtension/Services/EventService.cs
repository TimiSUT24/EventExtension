using EventClassLibrary.DTO;
using EventClassLibrary.Models;
using EventExtension.Repositories.Interfaces;
using EventExtension.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventExtension.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventItemDto>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllAsync();
            if(events == null || !events.Any())
            {
                throw new Exception("No events found.");
            }
            
            return events.Select(e => new EventItemDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                Link = e.Link,
                Img = e.Img,
                Categories = e.Categories,
                Attendance = e.Attendance,
                Ort = e.Ort,
                Dates = e.EventDates.Select(ed => new EventDatesDto
                {
                    Id = ed.Id,
                    StartDate = ed.StartDate,
                    EndDate = ed.EndDate,
                    Time = ed.Time
                }).ToList()

            });                 
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

            var eventEntities = events.Select(e => new EventItem
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                Link = e.Link,
                Img = e.Img,
                Categories = e.Categories,
                Attendance = e.Attendance,
                Ort = e.Ort,
                EventDates = e.Dates.Select(d => new EventDates
                {
                    Id = d.Id,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    Time = d.Time
                }).ToList()
            }).ToList();

            await _eventRepository.AddRangeAsyncEvents(eventEntities);         
            await _eventRepository.SaveChangesAsync();
           
        }
       
    }
}
