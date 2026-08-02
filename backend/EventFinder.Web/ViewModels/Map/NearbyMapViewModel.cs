namespace EventFinder.Web.ViewModels.Map
{
    /// <summary>
    /// Bootstraps the nearby-users map page; markers themselves are loaded client-side via AJAX.
    /// </summary>
    public class NearbyMapViewModel
    {
        public double? CurrentLat { get; set; }

        public double? CurrentLng { get; set; }

        public double RadiusKm { get; set; } = 10;
    }
}
