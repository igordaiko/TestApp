using System;
using System.Collections.Generic;
using ImportCoordinator.Contracts;

namespace ImportCoordinator.Core
{
    public class Settings
    {
        public static Settings Instance;

        public DatabaseSettings Database { get; set; } = new DatabaseSettings();

        public SecuritySettings Security { get; set; } = new SecuritySettings();

        public Dictionary<Source, SourceSettings> Source = new Dictionary<Source, SourceSettings>();

        public AzureSettings Azure { get; set; }

        public DatabricksSettings Databricks { get; set; }

        public EmailSettings Email{ get; set; }
    }

    public class SecuritySettings
    {
        public string Key
        {
            get => KeyBytes == null ? null : Convert.ToBase64String(KeyBytes);
            set => KeyBytes = value == null ? null : Convert.FromBase64String(value);
        }

        public byte[] KeyBytes { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessTokenName { get; set; }

        public long SessionExpiresIn { get; set; } = 365 * 24 * 3600;

    }

    public class DatabricksSettings
    {
        public string Token { get; set; }

        public string BaseUrl { get; set; }
    }

    public class AzureSettings
    {
        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public string ConfigContainerName { get; set; }

        public string BankConfigFileName { get; set; }

    }


    public class DatabaseSettings
    {
        public string ProviderName { get; set; } = "PostgreSQL.9.3";

        public string ConnectionString { get; set; }
    }

    public class EmailSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FromName { get; set; }

        public string FromAddress { get; set; }
    }

    

}
