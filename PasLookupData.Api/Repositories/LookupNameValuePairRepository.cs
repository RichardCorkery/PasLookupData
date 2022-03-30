using PasLookupData.Api.Repositories.Entities;

namespace PasLookupData.Api.Repositories;

public interface ILookupNameValuePairRepository
{
    IEnumerable<LookupNameValuePairEntity> All();
    Task<LookupNameValuePairEntity?> Get(string partitionKey, Guid rowKey);
    Task<LookupNameValuePairEntity?> GetByLookupKey(string partitionKey, string lookupKey);
    Task Insert(LookupNameValuePairEntity entity);
    Task Update(LookupNameValuePairEntity entity);
    Task CreateOrUpdate(LookupNameValuePairEntity entity);
    Task Delete(LookupNameValuePairEntity entity);
}

//ToDo2: Refactor with Generic Repo?
public class LookupNameValuePairRepository : ILookupNameValuePairRepository
{
    private readonly CloudTable _lookupNameValuePairTable;

    public LookupNameValuePairRepository(string cnnStr)
    {
        var storageAccount = CloudStorageAccount.Parse(cnnStr);
        var tableClient = storageAccount.CreateCloudTableClient();
        _lookupNameValuePairTable = tableClient.GetTableReference("LookupNameValuePair");
    }

    //ToDo: Return array?
    public IEnumerable<LookupNameValuePairEntity> All()
    {
        //ToDo: Review commented out code below
        var query = new TableQuery<LookupNameValuePairEntity>();
            //.Where(TableQuery.GenerateFilterConditionForBool(nameof(LookupNameValuePairEntity.Completed),
            //    QueryComparisons.Equal,
            //    false));

        return _lookupNameValuePairTable.ExecuteQuery(query);
    }

    //ToDo: Is Get a good name?
    public async Task<LookupNameValuePairEntity?> Get(string partitionKey, Guid rowKey)
    {
        var operation = TableOperation.Retrieve<LookupNameValuePairEntity>(partitionKey, rowKey.ToString());
        var result = await _lookupNameValuePairTable.ExecuteAsync(operation);
        return result.Result as LookupNameValuePairEntity;
    }
    
    public async Task<LookupNameValuePairEntity?> GetByLookupKey(string partitionKey, string lookupKey)
    {
        var partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

        var lookUpKeyFilter = TableQuery.GenerateFilterCondition("LookupKey", QueryComparisons.Equal, lookupKey);

        var query = new TableQuery<LookupNameValuePairEntity>().Where(TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, lookUpKeyFilter));

        var result = await _lookupNameValuePairTable.ExecuteQuerySegmentedAsync<LookupNameValuePairEntity>(query, null);

        //ToDo: FirstOrDefault vs SingleOrDefault
        return result.FirstOrDefault();
    }

    public async Task Insert(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Insert(entity);
        await _lookupNameValuePairTable.ExecuteAsync(operation);
    }

    public async Task Update(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Replace(entity);
        await _lookupNameValuePairTable.ExecuteAsync(operation);
    }

    //Note: You can have CreateOrUpdate
    public async Task CreateOrUpdate(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.InsertOrReplace(entity);
        await _lookupNameValuePairTable.ExecuteAsync(operation);
    }

    public async Task Delete(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Delete(entity);
        await _lookupNameValuePairTable.ExecuteAsync(operation);
    }
}