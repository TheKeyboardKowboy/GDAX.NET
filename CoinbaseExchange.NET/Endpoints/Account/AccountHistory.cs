namespace CoinbaseExchange.NET.Endpoints.Account {
    using System;
    using Newtonsoft.Json.Linq;

    public class AccountHistory {
        protected AccountHistory( JToken jtoken ) {
            this.JsonSource = jtoken.ToString();

            this.Id = jtoken[ "id" ].Value< String >();

            // Documented as "time", however the API is returning "created_at"
            this.TimeStamp = jtoken[ "created_at" ].Value< DateTime >();

            this.Amount = jtoken[ "amount" ].Value< Decimal >();
            this.Balance = jtoken[ "balance" ].Value< Decimal >();
            this.Type = jtoken[ "type" ].Value< String >();
        }

        public String JsonSource { get; set; }
        public String Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Balance { get; set; }
        public String Type { get; set; }

        public static AccountHistory FromJToken( JToken jToken ) {
            var type = jToken[ "type" ].Value< String >().ToLower();
            switch ( type ) {
                case "transfer":
                    return new AccountHistoryTransfer( jToken );
                default:
                    return new AccountHistory( jToken );
            }
        }
    }

    // TODO: Finish implementing subclasses for all entry types
    /*
        deposit 	User funds deposit from Coinbase
        withdraw 	User funds withdraw to Coinbase
        match 	Funds moved as a result of a trade
        fee 	Fee or rebate as a result of a trade
     */

    public class AccountHistoryTransfer : AccountHistory {
        public AccountHistoryTransfer( JToken jToken ) : base( jToken ) {
            if ( this.Type != "transfer" ) {
                throw new InvalidOperationException( "Transfer record can only be created from a valid transfer type json object" );
            }

            var details = jToken[ "details" ];
            var transferIdToken = details[ "transfer_id" ];
            var transferTypeToken = details[ "transfer_type" ];

            this.TransferId = transferIdToken?.Value< String >();
            this.TransferType = transferTypeToken?.Value< String >();
        }

        public String TransferId { get; set; }
        public String TransferType { get; set; }
    }
}
