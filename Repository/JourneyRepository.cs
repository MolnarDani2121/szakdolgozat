using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using UtazasNaplozas.Repository.IRepository;
using UtazasNaplozas.Data;

namespace UtazasNaplozas.Repository
{
	public class JourneyRepository : IJourneyRepository
	{
		private AuthenticationStateProvider _authStateProvider;
		private readonly ApplicationDbContext _db;

		public JourneyRepository(ApplicationDbContext db, AuthenticationStateProvider authStateProvider)
		{
			_authStateProvider = authStateProvider;
			_db = db;
		}

		public async Task<Journey> CreateAsync(Journey journey)
		{
			await _db.Journey.AddAsync(journey);
			await _db.SaveChangesAsync();
			return journey;
		}

		public async Task<bool> DeleteAsync(Guid Id)
		{
			var obj = await _db.Journey.FirstOrDefaultAsync(x => x.Id == Id);
			if (obj != null)
			{
				_db.Journey.Remove(obj);
				return await _db.SaveChangesAsync() > 0;
			}

			return false;
		}

		public async Task<Journey> GetAsync(Guid Id)
		{
			var obj = await _db.Journey.FirstOrDefaultAsync(x => x.Id == Id);
			if (obj != null)
			{
				return new Journey();
			}

			return obj;
		}

		public async Task<IEnumerable<JourneyDto>> GetAllAsync()
		{
			var userName = _authStateProvider.GetAuthenticationStateAsync().Result.User.Identity.Name;
			var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
			var journeys = await _db.Journey.Where(j => j.UserId == Guid.Parse(user!.Id))
											.Include(j => j.SubJourneys)
											.ThenInclude(sj => sj.Images)
											.ToListAsync();

			return journeys.Select(j => new JourneyDto
										{
											Id = j.Id,
											UserId = j.UserId,
											StartingPoint = j.StartingPoint,
											StartingPointLatitude = j.StartingPointLatitude,
											StartingPointLongitude = j.StartingPointLongitude,
											Destination = j.Destination,
											DestinationLatitude = j.DestinationLatitude,
											DestinationLongitude = j.DestinationLongitude,
											IsMapLoaded = false,
											StartDate = j.StartDate,
											EndDate = j.EndDate,
											SubJourneys = j.SubJourneys.OrderBy(sj => sj.Order).ToList(),
											LatLngs = new List<(double Lat, double Lng)>
											{
												(j.StartingPointLatitude, j.StartingPointLongitude)
											}
											.Concat(j.SubJourneys?
													.OrderBy(sj => sj.Order)
													.Select(sj => (sj.Latitude, sj.Longitude)) ?? Enumerable.Empty<(double, double)>())
											.Append((j.DestinationLatitude, j.DestinationLongitude))
											.ToList()
			});
		}

		public async Task<Journey> UpdateAsync(Journey journey)
		{
			var obj = await _db.Journey.FirstOrDefaultAsync(x => x.Id == journey.Id);
			if (obj != null)
			{
				obj.StartingPoint = journey.StartingPoint;
				obj.Destination = journey.Destination;
				obj.StartDate = journey.StartDate;
				obj.EndDate = journey.EndDate;
				_db.Journey.Update(obj);
				await _db.SaveChangesAsync();
				return obj;
			}

			return journey;
		}
	}
}
