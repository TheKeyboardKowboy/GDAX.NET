namespace CoinbaseExchange.NET.Endpoints.Account {
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Newtonsoft.Json.Linq;

    public class ListAccountsResponse : ExchangePageableResponseBase {
        public ListAccountsResponse( ExchangeResponse response ) : base( response ) {
            var json = response.ContentBody;
            var jArray = JArray.Parse( json );

            Accounts = jArray.Select( elem => new Account( elem ) ).ToList();
        }

        public IEnumerable< Account > Accounts { get; private set; }
    }
}
