namespace CoinbaseExchange.NET.Core {
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class CBAuthenticationContainer {
        private readonly String _secret;

        public CBAuthenticationContainer( String apiKey, String passphrase, String secret ) {
            if ( String.IsNullOrWhiteSpace( apiKey ) ) {
                throw new ArgumentNullException( nameof( apiKey ), "An API key is required to use the coinbase API" );
            }

            if ( String.IsNullOrWhiteSpace( passphrase ) ) {
                throw new ArgumentNullException( nameof( passphrase ), "A passphrase is required to use the coinbase API" );
            }

            if ( String.IsNullOrWhiteSpace( secret ) ) {
                throw new ArgumentNullException( nameof( secret ), "A secret is required to use the coinbase API" );
            }

            this.ApiKey = apiKey;
            this.Passphrase = passphrase;
            this._secret = secret;
        }

        public String ApiKey { get; private set; }
        public String Passphrase { get; private set; }

        public String ComputeSignature( String timestamp, String relativeUrl, String method, String body ) {
            var data = Convert.FromBase64String( this._secret );
            var prehash = timestamp + method + relativeUrl + body;
            return HashString( prehash, data );
        }

        private String HashString( String str, Byte[] secret ) {
            var bytes = Encoding.UTF8.GetBytes( str );
            using ( var hmac = new HMACSHA256( secret ) ) {
                var hash = hmac.ComputeHash( bytes );
                return Convert.ToBase64String( hash );
            }
        }
    }
}
