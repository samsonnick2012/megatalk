using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using XChat.Helpers;

namespace XChat.Service
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
                    IEnumerable<int> obj = (IEnumerable<int>) _Dictionary[key];
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

        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }       

        public int? image_id { get; set; } 
    }

    #endregion

#region Data
    [DataContract]
    public class UploadData : CommonDataContract
    {
        [DataMember]
        public int id { get; set; }
    }

#endregion

}