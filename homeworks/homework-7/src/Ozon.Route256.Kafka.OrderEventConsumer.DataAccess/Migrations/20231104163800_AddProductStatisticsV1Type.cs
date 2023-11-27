using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20231104163800, TransactionBehavior.None)]
public class AddProductStatisticsV1Type: Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'product_statistics_v1') THEN
            CREATE TYPE product_statistics_v1 as
            (
                id                  bigint
                , item_id           bigint
                , created_count     bigint
                , delivered_count   bigint
                , cancelled_count   bigint
                , modified_at       timestamp with time zone
            );
        END IF;
    END
$$;";

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
DO $$
    BEGIN
        DROP TYPE IF EXISTS product_statistics_v1;
    END
$$;";

        Execute.Sql(sql);
    }
}
