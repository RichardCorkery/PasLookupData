using Microsoft.AspNetCore.Mvc;
using PasLookupData.Api.Entities;
using PasLookupData.Api.Models;
using PasLookupData.Api.Repositories;

//ToDo: Review this whole file structure

namespace PasLookupData.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LookupNameValuePairsController : ControllerBase
{
    private readonly ILogger<LookupNameValuePairsController> _logger;
    private readonly ILookupNameValuePairRepository _lookupNameValuePairRepository;

    //ToDo: Add Logging

    //ToDo: Unit Tests 
    public LookupNameValuePairsController(ILogger<LookupNameValuePairsController> logger, ILookupNameValuePairRepository lookupNameValuePairRepository)
    {
        _logger = logger;
        _lookupNameValuePairRepository = lookupNameValuePairRepository;
    }

    // GET: api/vpb-delegates
    //ToDo: Review PS ToDo App
    //ToDo: Confirm Delete
    //ToDo: Review Controller of the function app I did
    //ToDo: Set up so only my Client can access the API: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis
    //  - Or maybe just set up okta?  

    // GET: api/lookupnamevaluepairs
    [HttpGet]
    public IEnumerable<LookupNameValuePairModel> Get()
    {
        var entities = _lookupNameValuePairRepository.All();

        var models = entities.Select(x => new LookupNameValuePairModel
        {
            RowKey = x.RowKey,
            PartitionKey = x.PartitionKey,
            LookupKey = x.LookupKey,
            Value = x.Value
        });

        return models.ToArray();
    }

    // GET: api/lookupnamevaluepairs/00000
    [HttpGet("partitionKey, rowKey")]
    public LookupNameValuePairModel Get(string partitionKey, string rowKey)
    {
        var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

        var model = new LookupNameValuePairModel
        {
            RowKey = entity.RowKey,
            PartitionKey = entity.PartitionKey,
            LookupKey = entity.LookupKey,
            Value = entity.Value
        };

        return model;

    }

    // POST api/lookupnamevaluepairs
    // ToDo: what is normally returned?
    //ToDo Return Create Code 202?
    [HttpPost]
    public LookupNameValuePairModel Post(LookupNameValuePairModel model)
    {

        var entity = new LookupNameValuePairEntity
        {
            PartitionKey = model.PartitionKey,
            RowKey = Guid.NewGuid().ToString(),
            LookupKey = model.LookupKey,
            Value = model.Value
        };

        _lookupNameValuePairRepository.Insert(entity);

        model.RowKey = entity.RowKey;

        return model;
    }

    // PUT api/lookupnamevaluepairs/???
    [HttpPut]
    public void Put(LookupNameValuePairModel model)
    {
        var entity = _lookupNameValuePairRepository.Get(model.PartitionKey, model.RowKey);
        
        entity.LookupKey = model.LookupKey;
        entity.Value = model.Value;

        _lookupNameValuePairRepository.Update(entity);
    }

    // PUT api/lookupnamevaluepairs/???
    [HttpDelete]
    public void Delete(string partitionKey, string rowKey)
    {
        var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

        _lookupNameValuePairRepository.Delete(entity);
    }
}
