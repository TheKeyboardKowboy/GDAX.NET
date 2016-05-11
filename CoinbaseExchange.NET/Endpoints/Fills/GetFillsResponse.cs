namespace CoinbaseExchange.NET.Endpoints.Fills {
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Newtonsoft.Json.Linq;

    public class GetFillsResponse : ExchangePageableResponseBase {
        public GetFillsResponse( ExchangeResponse response ) : base( response ) {
            var json = response.ContentBody;
            var jArray = JArray.Parse( json );
            Fills = jArray.Select( elem => new Fill( elem ) ).ToList();
        }

        public IEnumerable< Fill > Fills { get; private set; }
    }
}
