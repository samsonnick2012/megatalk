using Application.Core.Data;

namespace Innostar.Models
{
	public class UserNotification : ModelBase
	{
        public Notification Notification
        {
            get;
            set;
        }

        public int NotificationId
		{
			get;
			set;
		}

        public User ResipientUser
        {
            get;
            set;
        }

        public int ResipientUserId
		{
			get;
			set;
		}
	}
}