namespace EventFinder.Web.Services
{
    /// <summary>
    /// Outcome of an attempt to join an event.
    /// </summary>
    public enum JoinEventResult
    {
        Success,
        EventNotFound,
        AlreadyJoined,
        EventFull
    }
}
