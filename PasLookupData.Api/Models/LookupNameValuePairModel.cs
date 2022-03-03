namespace LookupTableAdminDemo.Api.Models;

public class LookupNameValuePairModel 
{
    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public string LookupKey { get; set; }

    public string Value { get; set; }
}