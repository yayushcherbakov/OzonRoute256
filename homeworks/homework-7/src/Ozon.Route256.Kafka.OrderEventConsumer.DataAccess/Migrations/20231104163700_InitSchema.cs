using System;
using FluentMigrator;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(20231104163700, TransactionBehavior.None)]
public sealed class InitSchema : Migration
{
    public override void Up()
    {
         const string sql =
            @"
             create table if not exists product_statistics
             (
                 id serial primary key,
                 item_id bigint not null,
                 created_count bigint not null,
                 delivered_count bigint not null,
                 cancelled_count bigint not null,
                 modified_at timestamp with time zone not null
             );
            ";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = "drop table if exists product_statistics;";

        Execute.Sql(sql);
    }
}
