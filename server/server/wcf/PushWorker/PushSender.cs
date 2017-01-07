using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Server;
using PushSharp;
using PushSharp.Apple;

namespace PushWorker
{
    public class PushSender
    {
        [SqlTrigger(Name = @"MessageTrigger", Target = "[dbo].[messageArchive]", Event = "FOR INSERT")]
        public static void SendNotification()
        {
            var logMessage = @"-0 -";
            try
            {
                var jidSenderList = new List<string>();
                var jidReceiverList = new List<string>();
                var sentDateList = new List<long>();
                var bodyList = new List<string>();

                SqlTriggerContext triggContext = SqlContext.TriggerContext;
                using (var openfireConn = new SqlConnection("context connection=true "))
                {
                    openfireConn.Open();
                    var sqlJidcomm = openfireConn.CreateCommand();
                    sqlJidcomm.CommandText = "select fromjid, tojid, sentdate from inserted";

                    using (var jidDataReader = sqlJidcomm.ExecuteReader())
                    {
                        while (jidDataReader.Read())
                        {
                            jidSenderList.Add(jidDataReader[0].ToString());
                            jidReceiverList.Add(jidDataReader[1].ToString());
                            sentDateList.Add(Convert.ToInt64(jidDataReader[2].ToString()));
                            logMessage += string.Format(@"- 1: from {0} to {1} at {2} -",
                                jidDataReader[0],
                                jidDataReader[1],
                                jidDataReader[2]);
                        }
                    }

                    var sqlMessageComm = openfireConn.CreateCommand();
                    sqlMessageComm.CommandText =
                        "select message from messageArchive where fromjid = @fromjid AND tojid = @tojid AND sentdate = @sentdate";
                    sqlMessageComm.Parameters.Add("@fromjid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlMessageComm.Parameters.Add("@tojid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlMessageComm.Parameters.Add("@sentdate", SqlDbType.BigInt).Value = 0;
                    sqlMessageComm.Prepare();

                    foreach (var date in sentDateList)
                    {
                        sqlMessageComm.Parameters["@fromjid"].Value = jidSenderList[sentDateList.IndexOf(date)];
                        sqlMessageComm.Parameters["@tojid"].Value = jidReceiverList[sentDateList.IndexOf(date)];
                        sqlMessageComm.Parameters["@sentdate"].Value = date;                        

                        using (var mDataReader = sqlMessageComm.ExecuteReader())
                        {
                            mDataReader.Read();
                            bodyList.Add(mDataReader[0].ToString());
                        }
                    }

                    openfireConn.Close();
                }

                using (var openfireConn = new SqlConnection("context connection=true "))
                {
                    openfireConn.Open();

                    var sqlPushSettingsComm = openfireConn.CreateCommand();
                    sqlPushSettingsComm.CommandText = "SELECT * FROM PushNotificationsSettings";
                    bool useSandbox;
                    string locationP12File;
                    string passwordP12File;
                    int badge;
                    string sound;
                    string userDomain;
                    string conferenceDomain;
                    using (var stpnDataReader = sqlPushSettingsComm.ExecuteReader())
                    {
                        stpnDataReader.Read();
                        useSandbox = stpnDataReader[0].ToString() == "1";
                        locationP12File = stpnDataReader[1].ToString();
                        passwordP12File = stpnDataReader[2].ToString();
                        badge = Convert.ToInt32(stpnDataReader[3].ToString());
                        sound = stpnDataReader[4].ToString();
                        userDomain = stpnDataReader[5].ToString();
                        conferenceDomain = stpnDataReader[6].ToString();
                        logMessage += string.Format(
                            @"- 2.0:  useSandbox: {0}, locationP12File: {1}, passwordP12File: {2}, badge: {3}, sound: {4}, userDomain: {5}, conferenceDomain: {6} -",
                            useSandbox,
                            locationP12File,
                            passwordP12File,
                            badge,
                            sound,
                            userDomain,
                            conferenceDomain);
                    }

                    var sqlSenderNameComm = openfireConn.CreateCommand();
                    //sqlSenderNameComm.CommandText = "if exists(" +
                    //                                " select u.username" +
                    //                                " from ofUser u left join ofRoster r" +
                    //                                " on u.username = r.username" +
                    //                                " where r.username = @receiverUsername and r.jid = @jid)" +
                    //                                " select isnull(isnull(r.nick, u.name), u.username), isnull(dt.devicetoken, '0')" +
                    //                                " from ofUser u left outer join devicetokens dt on dt.username = u.username left join ofRoster r on u.username = r.username" +
                    //                                " where r.username = @receiverUsername and r.jid = @jid" +
                    //                                " else" +
                    //                                " select isnull(u2.name, u2.username), isnull(dt.devicetoken, '0')" +
                    //                                " from ofUser u1 left outer join devicetokens dt on dt.username = u1.username, ofUser u2" +
                    //                                " where u1.username = @receiverUsername and u2.username = @senderUsername";

                    sqlSenderNameComm.CommandText = "if exists(" +
                                                    " select r.nick" +
                                                    " from ofRoster r" +
                                                    " where r.username = @receiverUsername and r.jid = @jid)" +
                                                    " select isnull(isnull(r.nick, u.name), u.username), isnull(dt.devicetoken, '0')" +
                                                    " from ofUser u" +
                                                    " inner join devicetokens dt on dt.username = @receiverUsername" +
                                                    " full outer join ofRoster r on u.username = @senderUsername" +
                                                    " where r.username = @receiverUsername and r.jid = @jid" +
                                                    " else" +
                                                    " select isnull(u.name, u.username), isnull(dt.devicetoken, '0')" +
                                                    " from ofUser u full outer join devicetokens dt on dt.username = @receiverUsername" +
                                                    " where  u.username = @senderUsername";

                    sqlSenderNameComm.Parameters.Add("@senderUsername", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlSenderNameComm.Parameters.Add("@receiverUsername", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlSenderNameComm.Parameters.Add("@jid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlSenderNameComm.Prepare();

                    var sqlConfSenderNameComm = openfireConn.CreateCommand();
                    sqlConfSenderNameComm.CommandText =
                        "SELECT isnull(isnull(r.nick, u1.name), u1.username), isnull(r1.nick, ' '), isnull(dt.devicetoken, 0), isnull(dt.username, '0')" +
                        " from ofUser u" +
                        " left outer join devicetokens dt on dt.username = u.username" +
                        " left outer join ofRoster r on u.username = r.username and r.jid = @senderJid and r.username != @senderUsername" +
                        " left outer join ofRoster r1 on u.username = r1.username and r1.jid = @receiverJid" +
                        " left outer join ofUser u1 on u1.username = @senderUsername" +
                        " where u.username in (select SUBSTRING(mca.jid, 0, CHARINDEX('@', mca.jid))" +
                        " from ofMucAffiliation mca" +
                        " inner join ofMucRoom mcr on mcr.roomID = mca.roomID" +
                        " where mcr.name = SUBSTRING(@receiverJid, 0, CHARINDEX('@', @receiverJid))" +
                        " union all" +
                        " select SUBSTRING(mcm.jid, 0, CHARINDEX('@', mcm.jid))" +
                        " from ofMucMember mcm" +
                        " inner join ofMucRoom mcr on mcr.roomID = mcm.roomID" +
                        " where mcr.name = SUBSTRING(@receiverJid, 0, CHARINDEX('@', @receiverJid)))" +
                        " and u.username != @senderUsername" +
                        " and dt.devicetoken != '0'";
                    sqlConfSenderNameComm.Parameters.Add("@senderUsername", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlConfSenderNameComm.Parameters.Add("@senderJid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlConfSenderNameComm.Parameters.Add("@receiverJid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlConfSenderNameComm.Prepare();

                    var sqlCountMessagesComm = openfireConn.CreateCommand();

                    sqlCountMessagesComm.CommandText =
                        "select sum(n) from (select count(*) n from messageArchive ma" +
                        " left join ofLastMessageTime lmt on ma.fromJID = lmt.receiverJid and ma.toJID = lmt.senderJid" +
                        " where (ma.tojid = @receiverJid and SUBSTRING(ma.tojid, CHARINDEX('@',ma.tojid) + 1, Len(ma.tojid)) != @conferenceDomain and not(ma.message like '%1C3A8527694F472E97681C14E40EF885%') and (lmt.lastReadTime is null or ma.sentDate > lmt.lastReadTime))" +
                        " union all" +
                        " select count(*) n from messageArchive ma" +
                        " left join ofLastMessageTime lmt on ma.toJID = lmt.receiverJid and lmt.senderJID = @receiverJid" +
                        " where (ma.tojid in (select mcr.name + '@' + @conferenceDomain from ofMucRoom mcr" +
                        " left join ofMucAffiliation mca on mca.roomID = mcr.roomID" +
                        " left join ofMucMember mcm on mcm.roomID = mcr.roomID" +
                        " where mca.jid = @receiverJid and mcm.jid is not null" +
                        " union all" +
                        " select mcr.name + '@' + @conferenceDomain from ofMucRoom mcr" +
                        " left join ofMucAffiliation mca on mca.roomID = mcr.roomID" +
                        " left join ofMucMember mcm on mcm.roomID = mcr.roomID" +
                        " where mcm.jid = @receiverJid and mca.jid is not null" +
                        " )" +
                        " and not(ma.message like '%1C3A8527694F472E97681C14E40EF885%')" +
                        " and (lmt.lastReadTime is null or ma.sentDate > lmt.lastReadTime))) s";
                    sqlCountMessagesComm.Parameters.Add("@receiverJid", SqlDbType.NVarChar, 1024).Value = "param";
                    sqlCountMessagesComm.Parameters.Add("@conferenceDomain", SqlDbType.NVarChar, 1024).Value = conferenceDomain;
                    
                    sqlCountMessagesComm.Prepare();

                    foreach (var date in sentDateList)
                    {
                        var bodyMessage = bodyList[sentDateList.IndexOf(date)];
                        logMessage += string.Format(@"- 2.1.0: bodyMessage: {0} -", bodyMessage);

                        if (!bodyMessage.Contains(
                                "5FB1CDA54FF54D659B58523B77A4599SYSTEMMESSAGEAEF6AA9FD4C74EE48AA4A5A66F2CF58B"))
                        {
                            string jidSender = jidSenderList[sentDateList.IndexOf(date)];
                            string jidReceiver = jidReceiverList[sentDateList.IndexOf(date)];

                            var senderStrings = jidSender.Split('@');
                            var receiverStrings = jidReceiver.Split('@');
                            var receiverUsername = receiverStrings[0];
                            var senderUsername = senderStrings[0];
                            var senderNameList = new List<string>();
                            var receiverJidList = new List<string>();
                            var deviceTokenList = new List<string>();

                            logMessage += string.Format(@"- 3.0.0: source-type: {0} -", receiverStrings[1]);
                            if (receiverStrings[1] == userDomain)
                            {
                                sqlSenderNameComm.Parameters["@receiverUsername"].Value = receiverUsername;
                                sqlSenderNameComm.Parameters["@senderUsername"].Value = senderUsername;
                                sqlSenderNameComm.Parameters["@jid"].Value = jidSender;

                                using (var snDataReader = sqlSenderNameComm.ExecuteReader())
                                {
                                    snDataReader.Read();
                                    deviceTokenList = new List<string> { snDataReader[1].ToString() };
                                    if (bodyMessage.Contains("57E0B049A79C348ACB863D22E1264B6C25AE4181222B5E64000AFC0C7B589C6695F"))
                                    {
                                        senderNameList.Add(String.Format("Пользователь {0} хочет добавить вас в список контактов", snDataReader[0]));
                                    }
                                    else if (bodyMessage.Contains("B257892FA351412C91B6B3348D39DE7C"))
                                    {
                                        senderNameList.Add(String.Format("Пользователь {0} предоставил вам права доступа", snDataReader[0]));
                                    }
                                    else
                                    {
                                        senderNameList.Add(snDataReader[0].ToString());
                                    }
                                    receiverJidList.Add(jidReceiver);
                                }
                            }
                            else if (receiverStrings[1] == conferenceDomain)
                            {
                                sqlConfSenderNameComm.Parameters["@senderUsername"].Value = senderUsername;
                                sqlConfSenderNameComm.Parameters["@senderJid"].Value = jidSender;
                                sqlConfSenderNameComm.Parameters["@receiverJid"].Value = jidReceiver;
                                using (var snDataReader = sqlConfSenderNameComm.ExecuteReader())
                                {
                                    while (snDataReader.Read())
                                    {
                                        if (bodyMessage.Contains("57E0B049A79C348ACB863D22E1264B6C25AE4181222B5E64000AFC0C7B589C6695F"))
                                        {
                                            senderNameList.Add(
                                                String.Format("Пользователь {0} хочет добавить вас в конференцию {1}",
                                                    snDataReader[0], snDataReader[1]));
                                        }
                                        else if (bodyMessage.Contains("B257892FA351412C91B6B3348D39DE7C"))
                                        {
                                            senderNameList.Add(
                                                String.Format("Пользователь {0} согласился участвовать в конференции {1}",
                                                    snDataReader[0], snDataReader[1]));
                                        }
                                        else
                                        {
                                            senderNameList.Add(string.Format("{0} - {1}", snDataReader[1],
                                                snDataReader[0]));
                                        }
                                        deviceTokenList.Add(snDataReader[2].ToString());
                                        receiverJidList.Add(string.Format("{0}@{1}", snDataReader[3], userDomain));
                                    }
                                }
                            }


                            var pushJidSender = jidSender;
                            if (receiverStrings[1] == conferenceDomain)
                            {
                                pushJidSender = jidReceiver;
                            }

                            var badgeList = new List<int>();

                            foreach (var jid in receiverJidList)
                            {
                                sqlCountMessagesComm.Parameters["@receiverJid"].Value = jid;
                                //sqlCountMessagesComm.Parameters["@conferenceJid"].Value = receiverStrings[1] == conferenceDomain ? jidReceiver : "0";
                                using (var countDataReader = sqlCountMessagesComm.ExecuteReader())
                                {
                                    countDataReader.Read();
                                    badgeList.Add(Convert.ToInt32(countDataReader[0]));
                                }
                            }

                            foreach (var deviceToken in deviceTokenList)
                            {
                                logMessage += string.Format(@"- 3: deviceToken: {0} -", deviceToken);
                                logMessage += string.Format(@"- 3.1: sendername: {0} -",
                                    senderNameList[deviceTokenList.IndexOf(deviceToken)]);
                                logMessage += string.Format(@"- 3.2: receiverJid: {0} -",
                                    receiverJidList[deviceTokenList.IndexOf(deviceToken)]);
                                logMessage += string.Format(@"- 3.3: badge: {0} -",
                                    badgeList[deviceTokenList.IndexOf(deviceToken)]);
                            }

                            //sqlCountMessagesComm.Parameters["@receiverJid"].Value = jidReceiver;
                            //int calcBadge = 0;

                            //using (var countDataReader = sqlCountMessagesComm.ExecuteReader())
                            //{
                            //    countDataReader.Read();
                            //    calcBadge = Convert.ToInt32(countDataReader[0]);
                            //}

                            foreach (var deviceToken in deviceTokenList)
                            {
                                var push = new PushBroker();

                                var appleCert = File.ReadAllBytes(locationP12File);
                                push.RegisterAppleService(new ApplePushChannelSettings(appleCert, passwordP12File));
                                push.QueueNotification(new AppleNotification()
                                                           .ForDeviceToken(deviceToken)
                                                           .WithAlert(senderNameList[deviceTokenList.IndexOf(deviceToken)])
                                                           .WithBadge(badgeList[deviceTokenList.IndexOf(deviceToken)])
                                                           .WithCustomItem("jidSender", pushJidSender)
                                                           .WithSound(sound));
                            }

                        }
                    }
                    var resultLogmessage = logMessage.Insert(0, "result: success -");
                    var insertUsernameConferenceCommand = openfireConn.CreateCommand();
                    insertUsernameConferenceCommand.CommandText = string.Format("INSERT INTO pushLog VALUES ('{0}', CURRENT_TIMESTAMP)", resultLogmessage);
                    insertUsernameConferenceCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //sqlP.Send(string.Format("error: {0}", ex.Message));
                using (var openfireConn = new SqlConnection("context connection=true "))
                {
                    logMessage = logMessage.Insert(0, "result: error -");
                    var resultLogmessage = string.Format("{0} exception: {1}", logMessage, ex.Message);
                    openfireConn.Open();
                    var insertUsernameConferenceCommand = openfireConn.CreateCommand();
                    insertUsernameConferenceCommand.CommandText =
                        string.Format("INSERT INTO pushLog VALUES ('{0}', CURRENT_TIMESTAMP)", resultLogmessage);
                    insertUsernameConferenceCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
