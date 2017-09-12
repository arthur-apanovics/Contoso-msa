using System.Data.Entity.Migrations;

namespace ContosoData.Migrations
{
    public partial class AddAccountAndTransactionModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Number = c.String(),
                        Balance = c.Single(nullable: false),
                        OverdraftLimit = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        TransactionType = c.String(),
                        Amount = c.Single(nullable: false),
                        RecepientName = c.String(),
                        RecepientAccountNumber = c.String(),
                        Description = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Transactions", new[] { "AccountId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.Accounts");
        }
    }
}
