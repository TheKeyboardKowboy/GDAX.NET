﻿namespace CoinbaseExchange.NET.Endpoints.Account {
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Newtonsoft.Json.Linq;

    public class GetAccountHistoryResponse : ExchangePageableResponseBase {
        public GetAccountHistoryResponse( ExchangeResponse response ) : base( response ) {
            var json = response.ContentBody;
            var jArray = JArray.Parse( json );

            AccountHistoryRecords = jArray.Select( AccountHistory.FromJToken ).ToList();
        }

        public IEnumerable< AccountHistory > AccountHistoryRecords { get; private set; }
    }
}
