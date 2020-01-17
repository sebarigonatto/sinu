namespace SINU.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fechacreacion1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FechaCreacion", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
        }
    }
}
