namespace CoinbaseExchange.NET.Utilities {
    using System;

    public static class DateTimeUtilities {
        public static Double ToUnixTimestamp( this DateTime dateTime ) { return ( dateTime - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds; }
    }
}
