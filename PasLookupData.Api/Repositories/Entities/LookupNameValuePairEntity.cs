namespace PasLookupData.Api.Repositories.Entities;

public class LookupNameValuePairEntity : TableEntity
{
    public string LookupKey { get; set; }

    public string Value { get; set; }
}