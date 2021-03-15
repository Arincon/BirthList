using System.Collections.Generic;
using System.Threading.Tasks;

namespace BirthList.Shared.Services
{
    public interface ITableService
    {
        Task<WishlistPresent> GetPresent(string partitionKey, string rowKey);
        Task<List<WishlistPresent>> GetAllPresentsPartition(string partitionId);
        Task<List<PurchasedPresent>> GetAllPurchasesPartition(string partitionId);
        Task<WishlistPresent> InsertOrMergePresentAsync(WishlistPresent entity);
        Task<PurchasedPresent> InsertOrMergePresentAsync(PurchasedPresent entity);
    }
}