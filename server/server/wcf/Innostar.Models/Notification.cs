using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Application.Core.Data;

namespace Innostar.Models
{
	public class Notification : ModelBase
	{
		public Notification()
		{
			UsersNotifications = new Collection<UserNotification>();
		}

        public bool HaveBeenRead
        {
            get;
            set;
        }

        public DateTime SendingDate
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

        public ICollection<UserNotification> UsersNotifications
        {
            get;
            set;
        }

        public string Value
		{
			get;
			set;
		}
	}
}