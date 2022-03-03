using Microsoft.Azure.Cosmos.Table;

namespace LookupTableAdminDemo.Api.Entities;

public class LookupNameValuePairEntity : TableEntity
{
    public string LookupKey { get; set; }

    public string Value { get; set; }
}