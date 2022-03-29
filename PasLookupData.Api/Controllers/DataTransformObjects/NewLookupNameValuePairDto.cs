namespace PasLookupData.Api.Controllers.DataTransformObjects;

public class NewLookupNameValuePairDto 
{
    public string PartitionKey { get; set; }

    public string LookupKey { get; set; }

    public string Value { get; set; }
}