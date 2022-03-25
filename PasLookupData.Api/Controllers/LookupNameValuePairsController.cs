//ToDo: XML Comments: Add
//ToDo: What can we use to show the XML Comments
//ToDo: Add bad request?
//  - See PS Class 1: Error Handling Demo
//  - See PS Class 2: Add Model Validation Basic?
//  - See PS Class 2: POST a new Talk

using PasLookupData.Api.Controllers.DataTransformObjects;
using PasLookupData.Api.Repositories.Entities;

namespace PasLookupData.Api.Controllers;

/// <summary>
/// PAS Lookup Data Operations
/// </summary>

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]")]
public class LookupNameValuePairsController : ControllerBase
{
    private readonly ILogger<LookupNameValuePairsController> _logger;
    private readonly ILookupNameValuePairRepository _lookupNameValuePairRepository;

    //ToDo: Unit Tests 

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupNameValuePairsController"/> class.
    /// </summary>
    /// <param name="logger">The logger</param> 
    /// <param name="lookupNameValuePairRepository">The Lookup Name Value Pair Repository</param>
    public LookupNameValuePairsController(ILogger<LookupNameValuePairsController> logger, ILookupNameValuePairRepository lookupNameValuePairRepository)
    {
        _logger = logger;
        _lookupNameValuePairRepository = lookupNameValuePairRepository;
    }

    //ToDo: Review Controller of the function app I did
    //ToDo: Set up so only my Client can access the API: https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis
    //  - Or maybe just set up okta?  

    //ToDo: Any way to make this async?
    // GET: api/lookupnamevaluepairs
    /// <summary>
    /// Get all of the Lookup Name Value Pairs
    /// </summary>
    /// <returns>The a list of LookupNameValuePairs</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LookupNameValuePairDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get()
    {
        //ToDo 2: Use Decorator DP for logging?
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            //ToDo: Push this processing down on an Orchestrator / Service?
            var entities = _lookupNameValuePairRepository.All();
            if (!entities.Any()) return NotFound("No LookupNameValuePairs found");

            var lookupNameValuePairDtos = entities.Select(e => new LookupNameValuePairDto
            {
                RowKey = e.RowKey,
                PartitionKey = e.PartitionKey,
                LookupKey = e.LookupKey,
                Value = e.Value
            });

            Response.Headers.Add("total-count", lookupNameValuePairDtos.Count().ToString());

            return Ok(lookupNameValuePairDtos.ToArray());
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

    // GET: api/LookupNameValuePairs/partitionKeyValue/rowKeyValue
    /// <summary>
    /// Get a NameValuePair by partitionKey and rowKey
    /// </summary>
    /// <param name="partitionKey">The LookupNameValuePair Partition Key</param>
    /// <param name="rowKey">The LookupNameValuePair Row Key</param>
    /// <returns>The selected LookupNameValuePair </returns>

    // ToDo: Change rowKey to guid?
    [HttpGet("{partitionKey}/{rowKey}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LookupNameValuePairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            //ToDo: Log the parameters?
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");
            
            //ToDo: Review some of my other apis

            var entity = await _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found");

            var lookupNameValuePairDto = new LookupNameValuePairDto
            {
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                LookupKey = entity.LookupKey,
                Value = entity.Value
            };

            return Ok(lookupNameValuePairDto);
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
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LookupNameValuePairDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(LookupNameValuePairDto lookupNameValuePairDto)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            //ToDo: try and do a get first???  
            var entity = new LookupNameValuePairEntity
            {
                PartitionKey = lookupNameValuePairDto.PartitionKey,
                RowKey = Guid.NewGuid().ToString(),
                LookupKey = lookupNameValuePairDto.LookupKey,
                Value = lookupNameValuePairDto.Value
            };

            await _lookupNameValuePairRepository.Insert(entity);

            lookupNameValuePairDto.RowKey = entity.RowKey;

            return Created(new Uri($"{Request.Path}/{lookupNameValuePairDto.PartitionKey}/{lookupNameValuePairDto.RowKey}", UriKind.Relative), lookupNameValuePairDto);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(LookupNameValuePairDto lookupNameValuePairDto)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = await _lookupNameValuePairRepository.Get(lookupNameValuePairDto.PartitionKey, lookupNameValuePairDto.RowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found"); ;

            entity.LookupKey = lookupNameValuePairDto.LookupKey;
            entity.Value = lookupNameValuePairDto.Value;

            await _lookupNameValuePairRepository.Update(entity);

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

    // DELETE api/LookupNameValuePairs/partitionKeyValue/rowKeyValue
    [HttpDelete("{partitionKey}/{rowKey}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string partitionKey, string rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");

            var entity = await _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            //ToDo: Include additoinal info (Parameters)?
            //  - Log it?
            if (entity == null) return NotFound("No LookupNameValuePair found"); 

            await _lookupNameValuePairRepository.Delete(entity);

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
