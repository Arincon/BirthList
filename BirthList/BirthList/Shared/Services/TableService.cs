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
        private readonly CloudTable table;
        private readonly IConfiguration _configuration;


        public TableService(IConfiguration configuration)
        {
            this._configuration = configuration;
            // Retrieve the storage account from the connection string.
            storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("TableStorage"));
            tableClient = storageAccount.CreateCloudTableClient();
            this.table = tableClient.GetTableReference(_configuration["PresentsTableName"]);
        }

        public async Task<Present> GetPresent(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Present>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                Present presentItem = result.Result as Present;
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
        public async Task<List<Present>> GetAllPresentsPartition(string partitionId)
        {
            try
            {
                var results = new List<Present>();
                var query = new TableQuery<Present>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionId));
                var token = new TableContinuationToken();
                var queryResults = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                if (queryResults != null)
                {
                    results.AddRange(queryResults.Results);
                    while (queryResults.ContinuationToken != null && !string.IsNullOrWhiteSpace(queryResults.ContinuationToken.NextPartitionKey))
                    {
                        queryResults = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
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

        public async Task<Present> InsertOrMergePresentAsync(Present entity)
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
                TableResult result = await this.table.ExecuteAsync(insertOrMergeOperation);
                Present insertedPresent = result.Result as Present;

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
    }
}

