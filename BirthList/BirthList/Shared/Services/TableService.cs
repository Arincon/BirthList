using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthList.Shared.Services
{
    public class TableService : ITableService
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly CloudTable presentListTable;
        private readonly CloudTable presentHistoryTable;
        private readonly IConfiguration _configuration;


        public TableService(IConfiguration configuration)
        {
            this._configuration = configuration;
            // Retrieve the storage account from the connection string.
            storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("TableStorage"));
            tableClient = storageAccount.CreateCloudTableClient();
            this.presentListTable = tableClient.GetTableReference(_configuration["PresentsTableName"]);
            this.presentHistoryTable = tableClient.GetTableReference(_configuration["HistoryTableName"]);
        }

        public async Task<WishlistPresent> GetPresent(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<WishlistPresent>(partitionKey, rowKey);
                TableResult result = await presentListTable.ExecuteAsync(retrieveOperation);
                WishlistPresent presentItem = result.Result as WishlistPresent;
                if (presentItem != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", presentItem.PartitionKey, presentItem.RowKey, presentItem.Title, presentItem.Description);
                }

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return presentItem;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<List<WishlistPresent>> GetAllPresentsPartition(string partitionId)
        {
            try
            {
                var results = new List<WishlistPresent>();
                var query = new TableQuery<WishlistPresent>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionId));
                var token = new TableContinuationToken();
                var queryResults = await presentListTable.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                if (queryResults != null)
                {
                    results.AddRange(queryResults.Results);
                    while (queryResults.ContinuationToken != null && !string.IsNullOrWhiteSpace(queryResults.ContinuationToken.NextPartitionKey))
                    {
                        queryResults = await presentListTable.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                        results.AddRange(queryResults.Results);
                    }
                }
                // TODO : Track event in application insights, acceso a table storage para leer respuesta
                return results;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<PurchasedPresent>> GetAllPurchasesPartition(string partitionId)
        {
            try
            {
                var results = new List<PurchasedPresent>();
                var query = new TableQuery<PurchasedPresent>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionId));
                var token = new TableContinuationToken();
                var queryResults = await presentHistoryTable.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                if (queryResults != null)
                {
                    results.AddRange(queryResults.Results);
                    while (queryResults.ContinuationToken != null && !string.IsNullOrWhiteSpace(queryResults.ContinuationToken.NextPartitionKey))
                    {
                        queryResults = await presentHistoryTable.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                        results.AddRange(queryResults.Results);
                    }
                }
                // TODO : Track event in application insights, acceso a table storage para leer respuesta
                return results;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<WishlistPresent> InsertOrMergePresentAsync(WishlistPresent entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await this.presentListTable.ExecuteAsync(insertOrMergeOperation);
                WishlistPresent insertedPresent = result.Result as WishlistPresent;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedPresent;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<PurchasedPresent> InsertOrMergePresentAsync(PurchasedPresent entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await this.presentHistoryTable.ExecuteAsync(insertOrMergeOperation);
                PurchasedPresent insertedPurchase = result.Result as PurchasedPresent;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedPurchase;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}

