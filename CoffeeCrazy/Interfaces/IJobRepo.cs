using CoffeeCrazy.Models;

namespace CoffeeCrazy.Interfaces
{
    public interface IJobRepo : ICRUDRepo<Job>
    {
        Task<List<Job>> GetGroupedJobsByFrequencyAsync(int machineId, int frequencyId); 
    }
}
