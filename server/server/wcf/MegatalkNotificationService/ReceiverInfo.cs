using System;

namespace MegatalkNotificationService
{
    public class ReceiverInfo
    {
        public string DisplayName { get; set; }

        public string Jid { get; set; }

        public DateTime SaveModeExpirationTime { get; set; }
    }
}
