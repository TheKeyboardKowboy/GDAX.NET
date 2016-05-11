namespace CoinbaseExchange.NET.Endpoints.OrderBook {
    using System;
    using Core;

    public class GetProductOrderBookRequest : ExchangeRequestBase {
        public GetProductOrderBookRequest( String productId, Int64 level = 1 ) : base( "GET" ) {
            if ( String.IsNullOrWhiteSpace( productId ) ) {
                throw new ArgumentNullException( nameof( productId ) );
            }

            this.RequestUrl = String.Format( "/products/{0}/book?level={1}", productId, level );
        }
    }
}
