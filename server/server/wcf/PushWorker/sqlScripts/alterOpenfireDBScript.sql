SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ofLastMessageTime](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[receiverJid] [nvarchar](max) NOT NULL,
	[senderJid] [nvarchar](max) NOT NULL,
	[lastDeliveredTime] [bigint] NOT NULL,
	[lastReadTime] [bigint] NOT NULL
 CONSTRAINT [ofLastMessageTime_pk] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[PushNotificationsSettings](
	[UseSandBox] [int] NOT NULL,
	[LocationP12File] [nvarchar](max) NOT NULL,
	[PasswordP12File] [nvarchar](max) NOT NULL,
	[badge] [int] NOT NULL,
	[sound] [nvarchar](max) NOT NULL,
	[userDomain] [nvarchar](max) NOT NULL,
	[conferenceDomain] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[devicetokens](
	[username] [nvarchar](max) NOT NULL,
	[devicetoken] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

insert into [dbo].[PushNotificationsSettings]
values (1, -- Use sandbox
'D:\Work\xchat-pushworker-certificates\WChatePushNotification.p12', -- location sertificate file
'', -- password setificate file
0, -- push badge
'default', -- push sound
'helios', -- user message domain
'conference.helios'  -- conference message domain
)

CREATE TABLE [dbo].[pushLog](
	[message] [nvarchar](max) NOT NULL,
	[time] [datetime] NOT NULL,
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO