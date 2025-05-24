namespace UtazasNaplozas.Data
{
	public class JourneyDto
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string StartingPoint { get; set; }
		public double StartingPointLatitude { get; set; }
		public double StartingPointLongitude { get; set; }
		public string Destination { get; set; }
		public double DestinationLatitude { get; set; }
		public double DestinationLongitude { get; set; }
		public bool IsMapLoaded { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public virtual ICollection<SubJourney> SubJourneys { get; set; }
		public virtual ICollection<(double Lat, double Lng)> LatLngs { get; set; }
	}
}
