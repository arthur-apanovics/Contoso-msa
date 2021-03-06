using System.Data.Entity.Migrations;

namespace ContosoData.Migrations
{
    public partial class ActivityLogging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FromId = c.String(),
                        FromName = c.String(),
                        RecipientId = c.String(),
                        RecipientName = c.String(),
                        TextFormat = c.String(),
                        TopicName = c.String(),
                        Local = c.String(),
                        Text = c.String(),
                        Summary = c.String(),
                        ChannelId = c.String(),
                        ServiceUrl = c.String(),
                        ReplyToId = c.String(),
                        Action = c.String(),
                        Type = c.String(),
                        Timestamp = c.DateTimeOffset(nullable: false, precision: 7),
                        ConversationId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Activities");
        }
    }
}
