using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;


[Migration(20231028143600, TransactionBehavior.None)]
public class AddModifiedAtToTaskComments : Migration 
{
    public override void Up()
    {
        const string sql = "alter table task_comments add modified_at timestamp with time zone null;";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = "alter table task_comments drop modified_at;";
        
        Execute.Sql(sql);
    }
}