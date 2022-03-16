namespace PasLookupData.Api.Repositories;

public interface ILookupNameValuePairRepository
{
    IEnumerable<LookupNameValuePairEntity> All();
    Task<LookupNameValuePairEntity?> Get(string partitionKey, string rowKey);
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

    public IEnumerable<LookupNameValuePairEntity> All()
    {
        //ToDo: Review commented out code below
        var query = new TableQuery<LookupNameValuePairEntity>();
            //.Where(TableQuery.GenerateFilterConditionForBool(nameof(LookupNameValuePairEntity.Completed),
            //    QueryComparisons.Equal,
            //    false));

        return _lookupNameValuePairTable.ExecuteQuery(query);
    }

    public async Task<LookupNameValuePairEntity?> Get(string partitionKey, string rowKey)
    {
        var operation = TableOperation.Retrieve<LookupNameValuePairEntity>(partitionKey, rowKey);
        var result = await _lookupNameValuePairTable.ExecuteAsync(operation);
        return result.Result as LookupNameValuePairEntity;
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