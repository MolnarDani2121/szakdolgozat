using UtazasNaplozas.Data;

namespace UtazasNaplozas.Repository.IRepository
{
	public interface ISubJourneyRepository
	{
		public Task<ICollection<SubJourney>> CreateAsync(ICollection<SubJourney> subJourneys);
		public Task<SubJourney> UpdateAsync(SubJourney subjourney);
		public Task<bool> DeleteAsync(Guid Id);
		public Task<SubJourney> GetAsync(Guid Id);
		public Task<IEnumerable<SubJourney>> GetAllAsync();
	}
}
