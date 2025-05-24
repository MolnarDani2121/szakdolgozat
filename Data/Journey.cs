using System.ComponentModel.DataAnnotations;

namespace UtazasNaplozas.Data
{
    public class Journey
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Add meg az út kezdő pontját")]
        public string StartingPoint { get; set; }
        public double StartingPointLatitude { get; set; }
        public double StartingPointLongitude { get; set; }
        [Required(ErrorMessage = "Add meg az út végpontját")]
        public string Destination { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
        [Required(ErrorMessage = "Add meg az út kezdésének dátumát")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Add meg az út végének dátumát")]
        public DateTime EndDate { get; set; }
        public virtual ICollection<SubJourney> SubJourneys { get; set; }
    }
}
