namespace UtazasNaplozas.Data
{
	public class MapLocation
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
