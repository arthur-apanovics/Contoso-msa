namespace ContosoData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateAcocunts : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Accounts (Name, Type, Number, Balance, OverdraftLimit) VALUES ('Streamline', 'Cheque', '02-1354-48978531-00', 5419.18, 200)");
            Sql("INSERT INTO Accounts (Name, Type, Number, Balance, OverdraftLimit) VALUES ('Credit', 'Credit', '02-1354-48978533-00', 234.8, 0)");
            Sql("INSERT INTO Accounts (Name, Type, Number, Balance, OverdraftLimit) VALUES ('Piggy Bank', 'Savings', '02-1354-48978532-00', 25311.5, 0)");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM Accounts WHERE Type = 'Cheque'");
            Sql("DELETE FROM Accounts WHERE Type = 'Credit'");
            Sql("DELETE FROM Accounts WHERE Type = 'Savings'");
        }
    }
}
