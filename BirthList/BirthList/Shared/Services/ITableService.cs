using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthList.Shared.Services
{
    public interface ITableService
    {
        Task<Present> GetPresent(string partitionKey, string rowKey);
        Task<List<Present>> GetAllPresentsPartition(string partitionId);
        Task<Present> InsertOrMergePresentAsync(Present entity);
    }
}