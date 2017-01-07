USE [openfire]
GO

ALTER DATABASE [openfire]
SET COMPATIBILITY_LEVEL =  100
GO

ALTER DATABASE [openfire]
SET TRUSTWORTHY ON
GO

EXEC sp_configure 'show advanced options' , '1';
reconfigure;
GO

EXEC sp_configure 'clr enabled' , '1';
reconfigure;
GO

EXEC sp_configure 'show advanced options' , '0';
reconfigure;
GO

CREATE ASSEMBLY   [System.IdentityModel]
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY   [Newtonsoft.Json]
FROM 'C:\Projects\XChat\PushWorker\Newtonsoft.Json.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY   [PushSharp.Core]
FROM 'C:\Projects\XChat\PushWorker\PushSharp.Core.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY   [PushSharp.Apple]
FROM 'C:\Projects\XChat\PushWorker\PushSharp.Apple.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY   [PushWorker]
FROM 'C:\Projects\XChat\PushWorker\PushWorker.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE TRIGGER MessageTrigger
ON [messageArchive] FOR INSERT
AS EXTERNAL NAME PushWorker.[PushWorker.PushSender].SendNotification