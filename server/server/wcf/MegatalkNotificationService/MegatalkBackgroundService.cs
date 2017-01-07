using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;

namespace MegatalkNotificationService
{
    public partial class MegatalkBackgroundService : ServiceBase
    {
        private BackgroundJobServer _backgroundJobServer;

        private string _notificationTemplate;

        public MegatalkBackgroundService()
        {
            _notificationTemplate = MegatalkDBHelper.GetNotificationTemplate();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _notificationTemplate = MegatalkDBHelper.GetNotificationTemplate();
                var storage = new SqlServerStorage(ConfigurationManager.ConnectionStrings["HangfireDBConnection"].ConnectionString);
                var options = new BackgroundJobServerOptions();
                _backgroundJobServer = new BackgroundJobServer(options, storage);
                _backgroundJobServer.Start();
                var jobManager = new RecurringJobManager(storage);
                jobManager.AddOrUpdate("1", Job.FromExpression(() => CheckUsers()), ConfigurationManager.AppSettings["CronExpressionForUsers"]);
                jobManager.AddOrUpdate("2", Job.FromExpression(() => CheckAudioFiles()), ConfigurationManager.AppSettings["CronExpressionForAudioFiles"]);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        protected override void OnStop()
        {
            _backgroundJobServer.Dispose();
        }

        public void CheckUsers()
        {
            try
            {
                if (_notificationTemplate == null)
                {
                    _notificationTemplate = MegatalkDBHelper.GetNotificationTemplate();
                }

                var _xmppClent = new XmppClient();
                MegatalkDBHelper.DeactivateExpiredSaveModes();
                var usersWithExpiredSaveMode = MegatalkDBHelper.GetUsersWithExpiringSaveMode();

                Thread.Sleep(5000);

                if (usersWithExpiredSaveMode.Any())
                {
                    foreach (var user in usersWithExpiredSaveMode)
                    {
                        _xmppClent.SendExpirationNotification(user.Jid, GetMessageBody(user));
                        Thread.Sleep(1000);
                    }
                }

                _xmppClent.Dispose();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public void CheckAudioFiles()
        {
            try
            {
                var expiredfiles = MegatalkDBHelper.CheckAudioPieces();

                foreach (var file in expiredfiles)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        private string GetMessageBody(ReceiverInfo receiverInfo)
        {
            return _notificationTemplate.Replace("%userDisplayName%", receiverInfo.DisplayName)
                    .Replace("%safeModeEndTime%", receiverInfo.SaveModeExpirationTime.ToString("dd.MM.yyyy hh:mm"));
        }
    }
}
