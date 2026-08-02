namespace EventFinder.Web.Models
{
    /// <summary>
    /// Event categories used for filtering. Kept as an enum for simplicity,
    /// can be normalized into a separate table later if needed.
    /// </summary>
    public enum EventCategory
    {
        Music,
        Sport,
        Technology,
        Education,
        Art,
        Business,
        Food,
        Charity,
        Other
    }
}
