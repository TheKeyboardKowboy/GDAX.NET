namespace GDAX.NET.Utilities {
    using System;

    public static class DateTimeUtilities {

        public static readonly DateTime UnixEpoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

        public static Double ToUnixTimestamp( this DateTime dateTime ) {
            return ( dateTime - UnixEpoch ).TotalSeconds;
        }
    }
}
