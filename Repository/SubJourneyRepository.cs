using Microsoft.EntityFrameworkCore;
using UtazasNaplozas.Data;
using UtazasNaplozas.Repository.IRepository;

namespace UtazasNaplozas.Repository
{
	public class SubJourneyRepository : ISubJourneyRepository
	{
		private readonly ApplicationDbContext _db;

		public SubJourneyRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<ICollection<SubJourney>> CreateAsync(ICollection<SubJourney> subJourneys)
		{
			foreach(var subJourney in subJourneys)
			{
				await _db.SubJourney.AddAsync(subJourney);
			}
			await _db.SaveChangesAsync();
			return subJourneys;
		}

		public async Task<bool> DeleteAsync(Guid Id)
		{
			var obj = await _db.SubJourney.FirstOrDefaultAsync(x => x.Id == Id);
			if (obj != null)
			{
				_db.SubJourney.Remove(obj);
				return await _db.SaveChangesAsync() > 0;
			}

			return false;
		}

		public async Task<SubJourney> GetAsync(Guid Id)
		{
			var obj =  await _db.SubJourney.FirstOrDefaultAsync(x => x.Id == Id);
			if (obj != null)
			{
				return new SubJourney();
			}

			return obj;
		}

		public async Task<IEnumerable<SubJourney>> GetAllAsync()
		{
			return await _db.SubJourney.ToListAsync();
		}

		public async Task<SubJourney> UpdateAsync(SubJourney subJourney)
		{
			var obj = await _db.SubJourney.FirstOrDefaultAsync(x => x.Id == subJourney.Id);
			if (obj != null)
			{
				obj.Location = subJourney.Location;
				obj.Description = subJourney.Description;
				_db.SubJourney.Update(obj);
				await _db.SaveChangesAsync();
				return obj;
			}

			return subJourney;

		}
	}
}
