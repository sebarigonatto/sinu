namespace SINU.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fechatoken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FechaToken", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FechaToken");
        }
    }
}
