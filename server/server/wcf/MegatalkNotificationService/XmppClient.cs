using System;
using System.Configuration;
using System.Globalization;

using agsXMPP;
using agsXMPP.Net;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace MegatalkNotificationService
{
    public class XmppClient : IDisposable
    {
        private XmppClientConnection _xmppClient;
        public XmppClient()
        {
            try
            {
                _xmppClient = new XmppClientConnection(ConfigurationManager.AppSettings["XmppDomain"], Convert.ToInt32(ConfigurationManager.AppSettings["XmppPort"]));
                _xmppClient.Username = ConfigurationManager.AppSettings["XmppAdminLogin"];
                _xmppClient.Password = ConfigurationManager.AppSettings["XmppAdminPassword"];
                _xmppClient.Show = ShowType.chat;
                _xmppClient.AutoResolveConnectServer = false;
                _xmppClient.ConnectServer = ConfigurationManager.AppSettings["XmppHttpBindPath"];
                _xmppClient.ClientSocket.ConnectTimeout = 15000;
                _xmppClient.AutoRoster = false;
                _xmppClient.KeepAlive = true;
                _xmppClient.SocketConnectionType = SocketConnectionType.Bosh;

                _xmppClient.OnAuthError += XmppClientOnOnAuthError;
                _xmppClient.OnError += XmppClientOnOnError;
                _xmppClient.OnSocketError += XmppClientOnOnSocketError;
                _xmppClient.OnStreamError += XmppClientOnOnStreamError;

                _xmppClient.Open();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public void SendExpirationNotification(string to, string body)
        {
            try
            {
                var message = new Message(new Jid(string.Format("{0}@{1}", to, _xmppClient.Server)), body);
                message.Attributes.Add("sentDate", GetLongFromDate(DateTime.Now).ToString(CultureInfo.InvariantCulture));
                message.Attributes.Add("archive", "true");
                message.Attributes.Add("push", "true");
                message.Attributes.Add("messagetype", "text");
                message.Attributes.Add("uuid", Guid.NewGuid().ToString("N"));

                _xmppClient.Send(message);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public static long GetLongFromDate(DateTime date)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            var end = date.Ticks / 10000;

            return end - start;
        }

        private void XmppClientOnOnStreamError(object sender, Element e)
        {
            Logger.LogMessage("stream error");
            Logger.LogMessage(e.ToString());
        }

        private void XmppClientOnOnSocketError(object sender, Exception ex)
        {
            Logger.LogMessage("socket error");
            Logger.LogMessage(ex.Message);
        }

        private void XmppClientOnOnError(object sender, Exception ex)
        {
            Logger.LogMessage("error");
            Logger.LogMessage(ex.Message);
        }

        private void XmppClientOnOnAuthError(object sender, Element e)
        {
            Logger.LogMessage("auth error");
            Logger.LogMessage(e.ToString());
        }

        public void Dispose()
        {
            _xmppClient.Close();
        }
    }
}
