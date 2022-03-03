﻿using LookupTableAdminDemo.Api.Entities;
using LookupTableAdminDemo.Api.Models;
using Microsoft.Azure.Cosmos.Table;

namespace LookupTableAdminDemo.Api.Repositories;

public class LookupNameValuePairRepository
{
    private CloudTable _lookupNameValuePairTable = null;

    public LookupNameValuePairRepository(string cnnStr)
    {

        var storageAccount = CloudStorageAccount.Parse(cnnStr);

        var tableClient = storageAccount.CreateCloudTableClient();

        _lookupNameValuePairTable = tableClient.GetTableReference("LookupNameValuePair");
    }

    public IEnumerable<LookupNameValuePairEntity> All()
    {
        var query = new TableQuery<LookupNameValuePairEntity>();
            //.Where(TableQuery.GenerateFilterConditionForBool(nameof(LookupNameValuePairEntity.Completed),
            //    QueryComparisons.Equal,
            //    false));

        return _lookupNameValuePairTable.ExecuteQuery(query);
    }

    public void Insert(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Insert(entity);

        _lookupNameValuePairTable.Execute(operation);
    }

    public void Update(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Replace(entity);

        _lookupNameValuePairTable.Execute(operation);
    }

    //Note: You can have CreateOrUpdate
    public void CreateOrUpdate(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.InsertOrReplace(entity);

        _lookupNameValuePairTable.Execute(operation);
    }

    public void Delete(LookupNameValuePairEntity entity)
    {
        var operation = TableOperation.Delete(entity);

        _lookupNameValuePairTable.Execute(operation);
    }

    public LookupNameValuePairEntity Get(string partitionKey, string rowKey)
    {
        var operation = TableOperation.Retrieve<LookupNameValuePairEntity>(partitionKey, rowKey);

        var result = _lookupNameValuePairTable.Execute(operation);

        return result.Result as LookupNameValuePairEntity;
    }
}