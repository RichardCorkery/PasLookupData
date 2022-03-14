//ToDo: Review this whole file structure

//ToDo: API Version?

namespace PasLookupData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupNameValuePairsController : ControllerBase
{
    private readonly ILogger<LookupNameValuePairsController> _logger;
    private readonly ILookupNameValuePairRepository _lookupNameValuePairRepository;

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
    //ToDo: What can be made: Task<IActionResult>
    [HttpGet]
    public IActionResult Get()
    //public IEnumerable<LookupNameValuePairModel> Get()
    {

        //ToDo 2: Use Decorator DP for logging?
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            //ToDo: Push this processing down on an Orchestrator / Service?
            var entities = _lookupNameValuePairRepository.All();
            //ToDo: Test for entities == null, then return not found


            var models = entities.Select(x => new LookupNameValuePairModel
            {
                RowKey = x.RowKey,
                PartitionKey = x.PartitionKey,
                LookupKey = x.LookupKey,
                Value = x.Value
            });

            return (IActionResult) new OkObjectResult(models.ToArray());
        }
        catch (Exception ex)
        {
            var message = "An error occurred while getting the LookupNameValuePairs";
            _logger.LogError(ex,  $"{logHeader} {message} ");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // GET: api/LookupNameValuePairs/partitionKey, rowKey?partitionKey=partitionKeyValue&rowKey=rowKeyValue
    // ToDo 2: Review what the template values below buys me
    [HttpGet("partitionKey, rowKey")]
    public IActionResult Get(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            //ToDo: Test if null, and return not found
            //ToDo: What else can be Http code should be returned
            //ToDo: Review some of my other apis

            var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            //ToDo: Test entity == null, return not found
            var model = new LookupNameValuePairModel
            {
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                LookupKey = entity.LookupKey,
                Value = entity.Value
            };

            return (IActionResult)new OkObjectResult(model);
        }
        catch (Exception ex)
        {
            var message = "An error occurred while getting the LookupNameValuePair";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // POST api/lookupnamevaluepairs
    // ToDo: what is normally returned?
    //ToDo Return Create Code 202?
    [HttpPost]
    public LookupNameValuePairModel Post(LookupNameValuePairModel model)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

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
        catch (Exception ex)
        {
            //ToDo: What do I want to return here?
            return null;
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // PUT api/lookupnamevaluepairs
    [HttpPut]
    public void Put(LookupNameValuePairModel model)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = _lookupNameValuePairRepository.Get(model.PartitionKey, model.RowKey);

            //ToDo: Test if null, and return not found

            entity.LookupKey = model.LookupKey;
            entity.Value = model.Value;

            _lookupNameValuePairRepository.Update(entity);
        }
        catch (Exception ex)
        {
            //ToDo: What do I want to return here?
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }

    }

    // DELETE api/LookupNameValuePairs/partitionKey, rowKey?partitionKey=partitionKeyValue&rowKey=rowKeyValue
    [HttpDelete("partitionKey, rowKey")]
    public void Delete(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);
            //ToDo: Test if null, and return not found
            _lookupNameValuePairRepository.Delete(entity);
        }
        catch (Exception ex)
        {
            //ToDo: What do I want to return here?
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }
}
