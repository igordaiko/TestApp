using System;
using System.Net.Mail;
using System.Threading.Tasks;
using LinqToDB;
using ImportCoordinator.Contracts;
using Event = ImportCoordinator.Contracts.Event;

namespace ImportCoordinator.Core.Services
{
    public class EventService
    {
        public EventService(Session session, EmailService mailService, AzureService azureService)
        {
            _session = session;
            _mailService = mailService;
            _azureService = azureService;
        }

        
        public async Task<ResponseVoid> SaveEvent(Event model)
        {
            var response = new ResponseVoid();

            using (var work = await _session.BeginWork(model.Data.SourceName))
            {

                var db = work.Database;

                var _event = new Data.Event
                {
                    State = model.State,
                    Status = model.Data.Status.ToString().ToLower(),
                    Uuid = model.Id,
                    Action = model.Action,
                    Description = model.Data.Description,
                    Bank = model.Data.SourceName.ToString()
                };


                db.InsertWithIdentity(_event);

                db.CommitTransaction();
            }
            

            return response;
        }


        public async Task<ResponseVoid> SendMail(Event model)
        {
            ResponseVoid response;

            using (var work = await _session.BeginWork(model.Data.SourceName))
            {
                switch (model.Data.Status)
                {
                    case EventStatus.Critical:
                        response = SendCriticalMail(model);
                        break;
                    case EventStatus.Failed:
                        response = await SendFailedMail(model);
                        break;
                    case EventStatus.Success:
                        response = SendSuccessMail(model);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
            return response;
        }


        private ResponseVoid SendCriticalMail(Event model)
        {
            var response = new ResponseVoid();
            var mailModel = new MailModel
            {
                Body = model.Data.Description,
                FromAddress = Settings.Instance.Email.FromAddress,
                FromName = Settings.Instance.Email.FromName,
                Subject = model.Data.Status.ToString(),
                ToAddress = Settings.Instance.Source[Session.CurrentSource].MailSettings.CriticalMail
            };

            _mailService.Send(mailModel);


            return response;
        }


        private async Task<ResponseVoid> SendFailedMail(Event model)
        {
            var response = new ResponseVoid();

            var mailModel = new MailModel
            {
                Body = model.Data.Description,
                FromAddress = Settings.Instance.Email.FromAddress,
                FromName = Settings.Instance.Email.FromName,
                Subject = model.State + " " + model.Data.Status,
                ToAddress = Settings.Instance.Source[Session.CurrentSource].MailSettings.SourceMail
            };

            if (model.State == "data validation")
            {
                var zipedBlobs = await _azureService.GetBlobZip(true);
                var zipName = Settings.Instance.Source[Session.CurrentSource].DirectoriesSettings.OutName + ".zip";
                var attachment = new Attachment(zipedBlobs, zipName,"application/x-gzip");

                mailModel.Attachments.Add(attachment);
            }

            _mailService.Send(mailModel);

            return response;
        }



        private readonly Session _session;
        private readonly EmailService _mailService;
        private readonly AzureService _azureService;
    }
}
