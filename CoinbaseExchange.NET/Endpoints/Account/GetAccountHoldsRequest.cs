namespace CoinbaseExchange.NET.Endpoints.Account {
    using System;
    using Core;

    public class GetAccountHoldsRequest : ExchangePageableRequestBase {
        public GetAccountHoldsRequest( String accountId ) : base( "GET" ) {
            if ( String.IsNullOrWhiteSpace( accountId ) ) {
                throw new ArgumentNullException( nameof( accountId ) );
            }

            var urlFormat = String.Format( "/accounts/{0}/holds", accountId );
            this.RequestUrl = urlFormat;
        }
    }
}
