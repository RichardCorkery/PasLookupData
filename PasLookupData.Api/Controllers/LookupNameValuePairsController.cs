//ToDo: Review this whole file structure

//ToDo: API Version?
//ToDo: Review API PS Class Notes
//ToDo: Review Work API: Returns???

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
            if (!entities.Any()) return NotFound("No LookupNameValuePairs found");

            var models = entities.Select(x => new LookupNameValuePairModel
            {
                RowKey = x.RowKey,
                PartitionKey = x.PartitionKey,
                LookupKey = x.LookupKey,
                Value = x.Value
            });

            return Ok(models.ToArray());
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
    //  ToDo: See above, does it make the values required?
    [HttpGet("partitionKey, rowKey")]
    public IActionResult Get(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            //ToDo: Log the parameters?
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");
            
            //ToDo: What else can be Http code should be returned
            //ToDo: Review some of my other apis

            var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found");

            var model = new LookupNameValuePairModel
            {
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                LookupKey = entity.LookupKey,
                Value = entity.Value
            };

            return Ok(model);
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
    //ToDo: Should the whole model returned, or just the Row Id?
    [HttpPost]
    public IActionResult Post(LookupNameValuePairModel model)
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

            return Created(new Uri($"{Request.Path}/partitionKey, rowKey?partitionKey={model.PartitionKey}&rowKey={model.RowKey}", UriKind.Relative), model);
        }
        catch (Exception ex)
        {
            //ToDo: Add info about which entity had the issue?
            var message = "An error occurred while creating the LookupNameValuePair";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // PUT api/lookupnamevaluepairs
    [HttpPut]
    public IActionResult Put(LookupNameValuePairModel model)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = _lookupNameValuePairRepository.Get(model.PartitionKey, model.RowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found"); ;

            entity.LookupKey = model.LookupKey;
            entity.Value = model.Value;

            _lookupNameValuePairRepository.Update(entity);

            return Ok();
        }
        catch (Exception ex)
        {
            //ToDo: Add info about which entity had the issue?
            var message = "An error occurred while updating the LookupNameValuePair";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // DELETE api/LookupNameValuePairs/partitionKey, rowKey?partitionKey=partitionKeyValue&rowKey=rowKeyValue
    [HttpDelete("partitionKey, rowKey")]
    public IActionResult Delete(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found"); 

            _lookupNameValuePairRepository.Delete(entity);

            return Ok();
        }
        catch (Exception ex)
        {
            //ToDo: Add info about which entity had the issue?
            var message = "An error occurred while deleting  the LookupNameValuePair";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }
}
