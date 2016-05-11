namespace CoinbaseExchange.NET.Endpoints.Account {
    using System;
    using Core;

    public class GetAccountHistoryRequest : ExchangePageableRequestBase {
        public GetAccountHistoryRequest( String accountId ) : base( "GET" ) {
            if ( String.IsNullOrWhiteSpace( accountId ) ) {
                throw new ArgumentNullException( nameof( accountId ) );
            }

            var urlFormat = String.Format( "/accounts/{0}/ledger", accountId );
            this.RequestUrl = urlFormat;
        }
    }
}
