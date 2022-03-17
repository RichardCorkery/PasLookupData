namespace PasLookupData.Api.Controllers.DataTransformObjects;

public class LookupNameValuePairDto 
{
    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public string LookupKey { get; set; }

    public string Value { get; set; }
}