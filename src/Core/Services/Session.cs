using System;
using System.IO;
using ImportCoordinator.Core.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using ImportCoordinator.Contracts;

namespace ImportCoordinator.Core.Services
{
    public class Session
    {
        public Session(AzureService azureService)
        {
            _azureService = azureService;
        }

        public string Identity { get; private set; }

        internal ServiceModel Service { get; private set; }

        internal Privileges Can { get; private set; } = Privileges.Empty;

        public static Source CurrentSource { get; set; }

        public bool LogIn(string serviceId)
        {

            using (var db = Database.Open())
            {
                var service = db.Services.SingleOrDefault(s => s.ServiceId == serviceId);

                if (service == null)
                    return false;

                SetService(service);

                return true;

            }

        }

        internal void SetService(ServiceModel model)
        {
            Identity = model.Id.ToString();

            Service = model;

            Can = Privileges.Create((long)model.Roles);
        }

        public async Task<UnitOfWork> BeginWork(Source source, bool transactional = true)
        {
            CurrentSource = source;


            if (_unitOfWork != null)
                return new UnitOfWork(_unitOfWork, transactional);

            _unitOfWork = new UnitOfWork(transactional, () => _unitOfWork = null);

            return _unitOfWork;
        }


        public async Task SetSourceSettings()
        {
            var azureSettings = Settings.Instance.Azure;

            var credentials = new StorageCredentials(azureSettings.AccountName, azureSettings.AccountKey);
            var account = new CloudStorageAccount(credentials, true);

            var cloudBlobClient = account.CreateCloudBlobClient();

            var container = cloudBlobClient.GetContainerReference(azureSettings.ConfigContainerName);

            var blob = container.GetBlobReference($"{CurrentSource.ToString()}/{azureSettings.BankConfigFileName}");

            using (var reader = new StreamReader(await blob.OpenReadAsync()))
            {
                var settings = JsonConvert.DeserializeObject<SourceSettings>(await reader.ReadToEndAsync());
                settings.ExpiresIn = DateTime.Now.AddDays(1); 

                if (Settings.Instance.Source.ContainsKey(CurrentSource))
                    Settings.Instance.Source[CurrentSource] = settings;
                else
                    Settings.Instance.Source.Add(CurrentSource, settings);

            }

        }

        private readonly AzureService _azureService;
        private UnitOfWork _unitOfWork;
    }
}
