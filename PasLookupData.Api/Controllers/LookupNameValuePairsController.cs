using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PasLookupData.Api.Entities;
using PasLookupData.Api.Models;
using PasLookupData.Api.Repositories;
using PasLookupData.Api.Common;

//ToDo: Move the using above to a shared location

//ToDo: Review this whole file structure

//ToDo: API Version?
//ToDo: Try Catch?


namespace PasLookupData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    //ToDo: Review Controller of the function app I did
    //ToDo: Set up so only my Client can access the API: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis
    //  - Or maybe just set up okta?  

    // GET: api/lookupnamevaluepairs
    //ToDo: What value should really be returned for each method?
    [HttpGet]
    public IEnumerable<LookupNameValuePairModel> Get()
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            //ToDo: Push this processing down on an Orchestrator / Service?
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
        catch (Exception ex)
        {
            //_logger.LogError(ex, $"{logHeader} {MessageConstant.ErrorGettingVpbDelegates}");
            //return StatusCode(StatusCodes.Status500InternalServerError, MessageConstant.ErrorGettingVpbDelegates);

            return null;
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // GET: api/LookupNameValuePairs/partitionKey, rowKey?partitionKey=partitionKeyValue&rowKey=rowKeyValue
    // ToDo:2 Review what the template values below buys me
    [HttpGet("partitionKey, rowKey")]
    public LookupNameValuePairModel Get(string partitionKey, string rowKey)
    {
        var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

        //ToDo: Test if null, and return not found
        //ToDo: What else can be Http code should be returned
        //ToDo: Review some of my other apis

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

    // PUT api/lookupnamevaluepairs
    [HttpPut]
    public void Put(LookupNameValuePairModel model)
    {
        var entity = _lookupNameValuePairRepository.Get(model.PartitionKey, model.RowKey);

        //ToDo: Test if null, and return not found
        
        entity.LookupKey = model.LookupKey;
        entity.Value = model.Value;

        _lookupNameValuePairRepository.Update(entity);
    }

    // DELETE api/LookupNameValuePairs/partitionKey, rowKey?partitionKey=partitionKeyValue&rowKey=rowKeyValue
    [HttpDelete("partitionKey, rowKey")]
    public void Delete(string partitionKey, string rowKey)
    {
        var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);
        //ToDo: Test if null, and return not found
        _lookupNameValuePairRepository.Delete(entity);
    }
}
