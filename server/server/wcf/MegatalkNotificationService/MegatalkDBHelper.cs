using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace MegatalkNotificationService
{
    public class MegatalkDBHelper
    {
        public static IEnumerable<ReceiverInfo> GetUsersWithExpiringSaveMode()
        {
            var result = new List<ReceiverInfo>();

            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MegatalkDBConnection"].ConnectionString))
                {
                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT XmppLogin, Name, SafeModeEndDate FROM [MegatalkDB].[dbo].[ChatUsers] " +
                                        "where (DATEDIFF(\"hour\", CURRENT_TIMESTAMP, SafeModeEndDate) = 48 OR " +
                                        "DATEDIFF(\"hour\", CURRENT_TIMESTAMP, SafeModeEndDate) = 24 OR " +
                                        "DATEDIFF(\"hour\", CURRENT_TIMESTAMP, SafeModeEndDate) = 4) AND " +
                                        "DATEDIFF(\"hour\", CURRENT_TIMESTAMP, SafeModeEndDate) > 0 AND SafeModeActivated = 1";
                    connection.Open();

                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new ReceiverInfo
                            {
                                Jid = dataReader[0].ToString(),
                                DisplayName = dataReader[1].ToString(),
                                SaveModeExpirationTime = dataReader.GetDateTime(2)
                            });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }

            return result;
        }

        public static void DeactivateExpiredSaveModes()
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MegatalkDBConnection"].ConnectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE [dbo].[ChatUsers] SET [SafeModeActivated] = 0 " +
                                            "WHERE DATEDIFF(\"minute\", CURRENT_TIMESTAMP, SafeModeEndDate) < 0 AND [SafeModeActivated] = 1";
                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        public static string GetNotificationTemplate()
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MegatalkDBConnection"].ConnectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT [Message] FROM [MegatalkDB].[dbo].[MessageTemplates] WHERE SpecialTemplate = 4";

                    connection.Open();

                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            return dataReader[0].ToString();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }

            return string.Empty;
        }

        public static IEnumerable<string> CheckAudioPieces()
        {
            var result = new List<string>();

            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MegatalkDBConnection"].ConnectionString))
                {
                    var getExpiredPathscommand = connection.CreateCommand();

                    getExpiredPathscommand.CommandText = "SELECT [Key], PhysicalFileName FROM AudioPieces " +
                        " where DATEDIFF(\"HOUR\" ,CURRENT_TIMESTAMP, ExpirationTime) < 0 And ReadyForRemoving <> 0";
                    connection.Open();

                    using (var dataReader = getExpiredPathscommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            if (result.All(e => e != Path.Combine(ConfigurationManager.AppSettings["AudioFilesPath"], string.Format("{0}.m4a", dataReader[0].ToString()))))
                            {
                                result.Add(Path.Combine(ConfigurationManager.AppSettings["AudioFilesPath"],  string.Format("{0}.m4a", dataReader[0].ToString())));
                            }

                            if (result.All(e => e != dataReader[1].ToString()))
                            {
                                result.Add(dataReader[1].ToString());
                            }
                        }
                    }

                    var getExpiredPiecescommand = connection.CreateCommand();
                    getExpiredPiecescommand.CommandText = "DELETE FROM AudioPieces " +
                        " where DATEDIFF(\"HOUR\" ,CURRENT_TIMESTAMP, ExpirationTime) < 0 And ReadyForRemoving <> 0";
                    getExpiredPiecescommand.ExecuteNonQuery();

                    var setExpiredPiecescommand = connection.CreateCommand();
                    setExpiredPiecescommand.CommandText = "UPDATE AudioPieces SET ReadyForRemoving = 1 " +
                        " where DATEDIFF(\"HOUR\" ,CURRENT_TIMESTAMP, ExpirationTime) < 0 And ReadyForRemoving = 0";
                    setExpiredPiecescommand.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }

            return result;
        }
    }
}
