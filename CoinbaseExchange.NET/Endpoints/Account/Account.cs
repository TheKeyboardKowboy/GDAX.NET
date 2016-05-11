namespace CoinbaseExchange.NET.Endpoints.Account {
    using System;
    using Newtonsoft.Json.Linq;

    public class Account {
        public Account( JToken jtoken ) {
            this.Id = jtoken[ "id" ].Value< String >();
            this.Currency = jtoken[ "currency" ].Value< String >();
            this.Balance = jtoken[ "balance" ].Value< Decimal >();
            this.Available = jtoken[ "available" ].Value< Decimal >();
        }

        public String Id { get; set; }
        public Decimal Balance { get; set; }
        public Decimal Available { get; set; }
        public String Currency { get; set; }
    }
}
