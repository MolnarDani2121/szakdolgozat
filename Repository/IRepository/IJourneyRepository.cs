using System.ComponentModel;
using UtazasNaplozas.Data;

namespace UtazasNaplozas.Repository.IRepository
{
    public interface IJourneyRepository
    {
        public Task<Journey> CreateAsync(Journey journey);
        public Task<Journey> UpdateAsync(Journey journey);
        public Task<bool> DeleteAsync(Guid Id);
        public Task<Journey> GetAsync(Guid Id);
        public Task<IEnumerable<JourneyDto>> GetAllAsync();
    }
}
