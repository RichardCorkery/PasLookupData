//ToDo 2: Secure API from Unauthorized access using Azure API Management API Key and / or Certificate

//ToDo 2: Use a DocFx site to show Header Comments
//  https://docs.microsoft.com/en-us/shows/on-net/intro-to-docfx?msclkid=143bae25bcf811eca4728a58a0c8ed86



//ToDo: XML Comments: Add / Better 

//ToDo: Add bad request?
//  - Flag this as a talking point for the API discussion
//  - See PS Class 1: Error Handling Demo
//  - See PS Class 2: Add Model Validation Basic?
//  - See PS Class 2: POST a new Talk

using Newtonsoft.Json;
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

            //ToDo 2: Push this processing down on an Orchestrator / Service?
            var entities = _lookupNameValuePairRepository.All();
            if (!entities.Any()) return NotFound("No LookupNameValuePairs found");

            var lookupNameValuePairDtos = entities.Select(e => new LookupNameValuePairDto
            {
                RowKey =  Guid.Parse(e.RowKey),
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
    /// Get a LookupNameValuePair by partitionKey and rowKey
    /// </summary>
    /// <param name="partitionKey">The LookupNameValuePair Partition Key</param>
    /// <param name="rowKey">The LookupNameValuePair Row Key</param>
    /// <returns>The selected LookupNameValuePair </returns>
    
    [HttpGet("{partitionKey}/{rowKey}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LookupNameValuePairDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string partitionKey, Guid rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");
            _logger.LogInformation($"{logHeader} Argument(s)- partitionKey: {partitionKey}, rowKey: {rowKey}");

            var entity = await _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            if (entity == null) return NotFound($"No LookupNameValuePair found for Partition Key: {partitionKey} and Row Key: {rowKey} ");

            var lookupNameValuePairDto = new LookupNameValuePairDto
            {
                RowKey = Guid.Parse(entity.RowKey),
                PartitionKey = entity.PartitionKey,
                LookupKey = entity.LookupKey,
                Value = entity.Value
            };

            return Ok(lookupNameValuePairDto);
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while getting the LookupNameValuePair for Partition Key: {partitionKey} and Row Key: {rowKey}";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // POST api/lookupnamevaluepairs

    /// <summary>
    /// Create a LookupNameValuePair 
    /// </summary>
    /// <param name="newLookupNameValuePairDto">The LookupNameValuePair to add</param>
    /// <returns>The newly created LookupNameValuePair </returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LookupNameValuePairDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(NewLookupNameValuePairDto newLookupNameValuePairDto)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");
            _logger.LogInformation($"{logHeader} Argument(s)- newLookupNameValuePairDto: {JsonConvert.SerializeObject(newLookupNameValuePairDto)}");

            var existingEntity = await _lookupNameValuePairRepository.GetByLookupKey(newLookupNameValuePairDto.PartitionKey, newLookupNameValuePairDto.LookupKey);
            if (existingEntity is not null)
            {
                return Conflict($"The LookupNameValuePair entity already exists for Partition Key: {newLookupNameValuePairDto.PartitionKey} and Row Key: {newLookupNameValuePairDto.LookupKey}");
            }
            
            var newEntity = new LookupNameValuePairEntity
            {
                PartitionKey = newLookupNameValuePairDto.PartitionKey,
                RowKey = Guid.NewGuid().ToString(),
                LookupKey = newLookupNameValuePairDto.LookupKey,
                Value = newLookupNameValuePairDto.Value
            };

            await _lookupNameValuePairRepository.Insert(newEntity);

            var lookupNameValuePairDto = new LookupNameValuePairDto
            {
                PartitionKey = newLookupNameValuePairDto.PartitionKey,
                RowKey = Guid.Parse(newEntity.RowKey),
                LookupKey = newLookupNameValuePairDto.LookupKey,
                Value = newLookupNameValuePairDto.Value
            };

            return Created(new Uri($"{Request.Path}/{lookupNameValuePairDto.PartitionKey}/{lookupNameValuePairDto.RowKey}", UriKind.Relative), lookupNameValuePairDto);
        }
        catch (Exception ex)
        {
            var message = "An error occurred while creating the new LookupNameValuePair for newLookupNameValuePairDto: {JsonConvert.SerializeObject(newLookupNameValuePairDto)}";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // PUT api/lookupnamevaluepairs

    /// <summary>
    /// Update a LookupNameValuePair
    /// </summary>
    /// <param name="lookupNameValuePairDto">The LookupNameValuePair to be updated</param>
    /// <returns>StatusCodes.Status200OK</returns>
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
            _logger.LogInformation($"{logHeader} Argument(s)- lookupNameValuePairDto: {JsonConvert.SerializeObject(lookupNameValuePairDto)}");

            var entity = await _lookupNameValuePairRepository.Get(lookupNameValuePairDto.PartitionKey, lookupNameValuePairDto.RowKey);
            
            if (entity == null) return NotFound($"No LookupNameValuePair found for Partition Key: {lookupNameValuePairDto.PartitionKey} and Row Key: {lookupNameValuePairDto.RowKey} ");

            entity.LookupKey = lookupNameValuePairDto.LookupKey;
            entity.Value = lookupNameValuePairDto.Value;

            await _lookupNameValuePairRepository.Update(entity);

            return Ok();
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while updating the LookupNameValuePair for lookupNameValuePairDto: {JsonConvert.SerializeObject(lookupNameValuePairDto)}";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }

    // DELETE api/LookupNameValuePairs/partitionKeyValue/rowKeyValue
    /// <summary>
    /// Delete a LookupNameValuePair 
    /// </summary>
    /// <param name="partitionKey">The LookupNameValuePair Partition Key to be deleted</param>
    /// <param name="rowKey">The LookupNameValuePair Row Key to be deleted</param>
    /// <returns>StatusCodes.Status200OK</returns>
    
    //ToDo XML Comment: Is the return value correct?  Should it be something else? 
    [HttpDelete("{partitionKey}/{rowKey}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string partitionKey, Guid rowKey)
    {
        var logHeader = $"[{GetType().Name}: {Guid.NewGuid()}]";

        try
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Started}");
            _logger.LogInformation($"{logHeader} Argument(s)- partitionKey: {partitionKey}, rowKey: {rowKey}");

            var entity = await _lookupNameValuePairRepository.Get(partitionKey, rowKey);

            if (entity == null) return NotFound($"No LookupNameValuePair found for Partition Key: {partitionKey} and Row Key: {rowKey} ");

            await _lookupNameValuePairRepository.Delete(entity);

            return Ok();
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while deleting the LookupNameValuePair for Partition Key: {partitionKey} and Row Key: {rowKey}";
            _logger.LogError(ex, $"{logHeader} {message}");
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
        finally
        {
            _logger.LogInformation($"{logHeader} {Constants.Tracing.Ended}");
        }
    }
}
