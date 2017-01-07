using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Innostar.UI.Helpers;

namespace Innostar.UI.Service
{
    #region Generic data containers

    [DataContract]
    public class CommonDataContract
    {
        [DataMember(EmitDefaultValue = false)]
        public string code { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string response_date { get; set; }
    }

    [Serializable]
    public class JsonDictionary<TKey, TValue> : CommonDataContract, ISerializable
    {
        private Dictionary<TKey, TValue> _Dictionary;

        public JsonDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public JsonDictionary(SerializationInfo info, StreamingContext context)
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public TValue this[TKey key]
        {
            get { return _Dictionary[key]; }
            set { _Dictionary[key] = value; }
        }

        public void Add(TKey key, TValue value)
        {
            _Dictionary.Add(key, value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (TKey key in _Dictionary.Keys)
            {
                if (_Dictionary[key] is IEnumerable<int>)
                {
                    IEnumerable<int> obj = (IEnumerable<int>)_Dictionary[key];
                    info.AddValue(key.ToString(), obj.ToDelimitedString(","));
                }
                else
                {
                    info.AddValue(key.ToString(), _Dictionary[key]);
                }
            }

            info.AddValue("code", code);
            info.AddValue("message", message);
            if (!string.IsNullOrEmpty(response_date))
            {
                info.AddValue("response_date", response_date);
            }
        }
    }

    #endregion

    #region Authentication

    [DataContract]
    public class AuthData : CommonDataContract
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string xmpp_password { get; set; }

        [DataMember]
        public ChatUserData user_data { get; set; }

        [DataMember]
        public string admin_jid { get; set; }

        [DataMember]
        public string current_time { get; set; }

        [DataMember]
        public CommonDataContract mail_data { get; set; }
    }

    [DataContract]
    public class BoolData
    {
        [DataMember(Name = "value")]
        public bool Value { get; set; }
    }

    #endregion

    #region User Data

    [DataContract]
    public class ChatUsersData : CommonDataContract
    {
        [DataMember]
        public List<ChatUserData> users { get; set; }
    }

    public class ChatUserData : CommonDataContract
    {
        public int id { get; set; }
        public string login { get; set; }
        public string xmpp_login { get; set; }
        public string uuid { get; set; }
        public int safe_mode_activated { get; set; }
        public int? MessageStorageType { get; set; }
        public bool? IsMessagesArchived { get; set; }
        public int? TimeoutTypeClosedConference { get; set; }
        public bool? IsConfirmedDelivery { get; set; }
        public bool IsPasswordDeleteAllEnabled { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string LastActivityTime { get; set; }
        public int? image_id { get; set; }
    }

    [DataContract]
    public class UserSettingsData
    {
        [DataMember]
        public bool? IsMessagesArchived { get; set; }

        [DataMember]
        public int? TimeoutTypeClosedConference { get; set; }

        [DataMember]
        public bool? IsConfirmedDelivery { get; set; }

        [DataMember]
        public int? MessageStorageType { get; set; }
    }

    [DataContract]
    public class RecoverPasswordData : CommonDataContract
    {
        public int IsLogin { get; set; }
        public int ExistLogin { get; set; }
        public int TypeQuestion { get; set; }
        public int IsEmail { get; set; }
        public int ExistEmail { get; set; }
        public int TypeEmail { get; set; }
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string question { get; set; }
    }

    [DataContract]
    public class StringData
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    #endregion

    #region Data
    [DataContract]
    public class UploadData : CommonDataContract
    {
        [DataMember]
        public int id { get; set; }
    }

    [DataContract]
    public class ConferenceConfigurationData : CommonDataContract
    {
        [DataMember]
        public int IsMessagesArchivedAtServer { get; set; }

        [DataMember]
        public int IsCleanedByPasswordDeleteAll { get; set; }
    }

    #endregion
}