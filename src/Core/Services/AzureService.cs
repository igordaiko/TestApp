using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImportCoordinator.Contracts;
using ImportCoordinator.Core.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using MyPackage.Http;
using Microsoft.Azure.Databricks.Client;

namespace ImportCoordinator.Core
{
    public class AzureService
    {

        public async Task<MemoryStream> GetBlobZip(bool sendCriticalIfEmpty)
        {
            try
            {
                var container = GetContainerReference(Session.CurrentSource.ToString());
                var outFolder =
                    container.GetDirectoryReference(Settings.Instance.Source[Session.CurrentSource].DirectoriesSettings
                        .OutName);
                var blobs = await outFolder.ListBlobsSegmentedAsync(new BlobContinuationToken());

                if (sendCriticalIfEmpty && !blobs.Results.Any())
                    throw new FileNotFoundException();

                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
                    {
                        foreach (var listBlobItem in blobs.Results)
                        {
                            var blob = (CloudBlob) listBlobItem;
                            ZipArchiveEntry zipArchiveEntry =
                                zipArchive.CreateEntry(blob.Name, CompressionLevel.Optimal);
                            using (var stream = zipArchiveEntry.Open())
                            {
                                var blobStream = await blob.OpenReadAsync();
                                blobStream.CopyTo(stream);
                            }
                        }
                    }
                    return new MemoryStream(memoryStream.ToArray());
                    
                }
            }
            catch (Exception e)
            {
                HttpClientHandler handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                var eventEndpoint = Settings.Instance.Source[Session.CurrentSource].EventSettings.EventEndpoint;
                if (e is FileNotFoundException)
                {
                    var eventModel = new Event
                    {
                        Data = new Contracts.Data
                        {
                            SourceName = Session.CurrentSource,
                            Description = "files not found",
                            Status = EventStatus.Critical
                        },
                        EventTime = DateTime.Now,
                        State = "send email",
                        Action = "unpack",
                        Id = Guid.NewGuid().ToString()
                    };
                    
                    //Post from MyPackage.Http
                    await client.Post(eventEndpoint, eventModel);
                }

            }
            return null;
        }

        private static CloudBlobContainer GetContainerReference(string containerName)
        {
            var azureSettings = Settings.Instance.Azure;

            var credentials = new StorageCredentials(azureSettings.AccountName, azureSettings.AccountKey);
            var account = new CloudStorageAccount(credentials, true);

            var cloudBlobClient = account.CreateCloudBlobClient();

            return cloudBlobClient.GetContainerReference(containerName);
        }

        public async Task GetJobInfo()
        {
            using (var client = DatabricksClient.CreateClient(Settings.Instance.Databricks.BaseUrl, Settings.Instance.Databricks.Token))
            {
                var jobId = await client.Jobs.Get(51);
                var run = await client.Jobs.RunsGet(22);
            }
        }

        public async Task RunDatabricks()
        {
            using (var client = DatabricksClient.CreateClient(Settings.Instance.Databricks.BaseUrl, Settings.Instance.Databricks.Token))
            {
                var jobId = await client.Jobs.RunNow(51, null);
            }
        }
    }
}