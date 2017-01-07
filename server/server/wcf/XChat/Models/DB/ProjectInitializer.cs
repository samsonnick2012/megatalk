using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using XChat.Migrations;


namespace XChat.Models.DB
{
    internal class ProjectInitializer : MigrateDatabaseToLatestVersion<XChatContext, Configuration>
    {
    }
}