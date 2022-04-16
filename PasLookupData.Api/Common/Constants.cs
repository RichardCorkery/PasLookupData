namespace PasLookupData.Api.Common;

public class Constants
{
    public class AppSettingsKey
    {
        public const string Environment = "Environment";
        public const string PasStorageConnectionString = "Data:PasStorage:ConnectionString";
    }

    public class Tracing
    {
        public const string Started = "Trace Started";
        public const string Ended = "Trace Ended";
    }

    public class Environment
    {
        public const string Development = "development";
        public const string SysTest = "systest";
        public const string Uat = "uat";
        public const string Production = "production";
    }
}