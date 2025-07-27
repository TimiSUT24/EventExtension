using EventClassLibrary.DTO;

namespace EventExtension.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventItemDto>> GetAllEvents();
        Task<IEnumerable<EventItemDto>> RemoveEventsRangeWithId(int id, int id2);
    }
}
