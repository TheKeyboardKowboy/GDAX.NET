namespace GDAX.NET.Core {
    using System;
    using Utilities;

    public abstract class ExchangeRequestBase {
        protected ExchangeRequestBase( String method ) {
            this.Method = method;
            this.TimeStamp = DateTime.UtcNow.ToUnixTimestamp();
        }

        public String Method { get; private set; }
        public Double TimeStamp { get; }
        public String RequestUrl { get; protected set; }
        public String RequestBody { get; protected set; }

        public Boolean IsExpired => this.GetCurrentUnixTimeStamp() - this.TimeStamp >= 30;

        protected virtual Double GetCurrentUnixTimeStamp() { return DateTime.UtcNow.ToUnixTimestamp(); }
    }
}
