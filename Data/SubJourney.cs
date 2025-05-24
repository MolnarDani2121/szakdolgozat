using System.ComponentModel.DataAnnotations;

namespace UtazasNaplozas.Data
{
	public class SubJourney
	{
		public Guid Id { get; set; }
		public Guid JourneyId { get; set; }
		[Required(ErrorMessage = "Add meg az esemény helyét!")]
		public string Location { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public int Order { get; set; }
		public virtual ICollection<SubJourneyImage> Images { get; set; }
	}
}
