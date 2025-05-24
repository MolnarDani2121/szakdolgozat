using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UtazasNaplozas.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<Journey> Journey { get; set; }
	public DbSet<SubJourney> SubJourney { get; set; }
	public DbSet<JourneyCoverImage> CoverImage { get; set; }
	public DbSet<SubJourneyImage> Image { get; set; }
}
