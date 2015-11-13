using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace PetaPoco.Migrations
{
    [Migration(1)]
    public class FooMigration1 : Migration
    {
        public override void Up()
        {
            Create.Table("Foos")
                .WithColumn("FooID").AsInt32().PrimaryKey().Identity()
                .WithColumn("X").AsInt32().NotNullable()
                .WithColumn("Y").AsInt32().NotNullable()
                .WithColumn("Z").AsString(255).Nullable();

            Create.Index("idx_FooX").OnTable("Foos").OnColumn("X");
        }

        public override void Down()
        {
            Delete.Table("Foos");           
        }           
    }
}
