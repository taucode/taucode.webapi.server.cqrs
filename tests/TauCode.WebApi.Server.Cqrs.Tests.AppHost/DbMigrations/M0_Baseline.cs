using System;
using System.Collections.Generic;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost.DbMigrations
{
    [Migration(0)]
    public class M0_Baseline : AutoReversingMigration
    {
        private static readonly Guid CurrencyLayerUserId = new Guid("a637d99f-6af9-40e8-b1c5-865f7ca5e468");
        private static readonly Guid UsdId = new Guid("ce49c7a1-66f8-494e-b5cd-9b9b925637ee");

        public override void Up()
        {
            this.Create.Table("currency")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_currency")
                .WithColumn("code")
                    .AsAnsiString()
                    .NotNullable()
                    .Unique("UX_currency_code")
                .WithColumn("name")
                    .AsString()
                    .NotNullable()
                .WithColumn("is_available_to_users")
                    .AsBoolean()
                    .NotNullable();

            this.Create.Table("user")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_user");

            this.Create.Table("system_user")
                .WithColumn("user_id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_systemUser")
                .WithColumn("code")
                    .AsString(50)
                    .NotNullable()
                    .Unique("UX_systemUser_Code");

            this.Create.ForeignKey("FK_systemUser_user")
                .FromTable("system_user").ForeignColumn("user_id")
                .ToTable("user").PrimaryColumn("id");

            this.Create.Table("quote")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_quote")
                .WithColumn("user_id")
                    .AsGuid()
                    .NotNullable()
                .WithColumn("currency_id")
                    .AsGuid()
                    .NotNullable()
                .WithColumn("date")
                    .AsDateTime()
                    .NotNullable()
                .WithColumn("is_basic")
                    .AsBoolean()
                    .NotNullable()
                .WithColumn("rate")
                    .AsDecimal(20, 10)
                    .Nullable()
                .WithColumn("system_user_id")
                    .AsGuid()
                    .Nullable();

            this.Create.ForeignKey("FK_quote_currency")
                .FromTable("quote").ForeignColumn("currency_id")
                .ToTable("currency").PrimaryColumn("id");

            this.Create.ForeignKey("FK_quote_user")
                .FromTable("quote").ForeignColumn("user_id")
                .ToTable("user").PrimaryColumn("id");

            this.Create.ForeignKey("FK_quote_systemUser")
                .FromTable("quote").ForeignColumn("system_user_id")
                .ToTable("system_user").PrimaryColumn("user_id");

            this.Create.UniqueConstraint("UX_quote_userId_currencyId_date")
                .OnTable("quote").Columns("user_id", "currency_id", "date");

            this.Create.Index("IX_quote_userId")
                .OnTable("quote").OnColumn("user_id");

            this.Create.Index("IX_quote_userId_currencyId")
                .OnTable("quote")
                    .OnColumn("user_id").Ascending()
                    .OnColumn("currency_id").Ascending();

            this.Create.Index("IX_quote_userId_date")
                .OnTable("quote")
                    .OnColumn("user_id").Ascending()
                    .OnColumn("date").Descending();

            this.Create.Index("IX_quote_systemUserId")
                .OnTable("quote")
                .OnColumn("system_user_id").Ascending();

            this.InsertCurrencies();
            this.InsertSystemUser();
            this.InsertSystemUserQuotes();
        }

        private void InsertCurrencies()
        {
            var json = this.GetType().Assembly.GetResourceText("currencies.json", true);
            var currencies = JsonConvert.DeserializeObject<List<CurrencyDto>>(json);

            foreach (var currency in currencies)
            {
                var row = new
                {
                    id = currency.Id,
                    code = currency.Code,
                    name = currency.Name,
                    is_available_to_users = currency.InitialIsAvailableToUsers,
                };

                this.Insert.IntoTable("currency").Row(row);
            }
        }

        private void InsertSystemUser()
        {
            var user = new
            {
                id = CurrencyLayerUserId,
            };

            this.Insert.IntoTable("user").Row(user);

            var systemUser = new
            {
                user_id = CurrencyLayerUserId,
                code = "currencylayer",
            };

            this.Insert.IntoTable("system_user").Row(systemUser);
        }

        private void InsertSystemUserQuotes()
        {
            var json = this.GetType().Assembly.GetResourceText("currencies.json", true);
            var currencies = JsonConvert.DeserializeObject<List<CurrencyDto>>(json);

            var date = "1900-01-01".ToExactDate();

            foreach (var currency in currencies)
            {
                var isUsd = currency.Code == "USD";

                var quote = new
                {
                    id = Guid.NewGuid(),
                    user_id = CurrencyLayerUserId,
                    currency_id = currency.Id,
                    date = date,
                    is_basic = isUsd,
                    rate = currency.InitialRate,
                    system_user_id = (object)null,
                };

                this.Insert.IntoTable("quote").Row(quote);
            }
        }
    }
}

