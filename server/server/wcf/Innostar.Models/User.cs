using System.Collections.Generic;
using Application.Core.Data;

namespace Innostar.Models
{
	public class User : ModelBase
	{
        public string Email
        {
            get;
            set;
        }

        public Membership Membership
        {
            get;
            set;
        }

        public string UserName
		{
			get;
			set;
		}

        public ICollection<UserNotification> UsersNotifications
        {
            get;
            set;
        }
	}
}
