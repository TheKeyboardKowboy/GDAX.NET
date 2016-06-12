namespace GDAX.NET.Core {
    using System;
    using System.Windows.Forms;
    using Librainian.Controls;
    using Librainian.FileSystem;
    using Librainian.Persistence;
    using Librainian.Threading;

    public static class Configuration {
        public static Folder SettingsFolder { get; } = new Folder( Environment.SpecialFolder.LocalApplicationData, nameof( GDAX ) );
        public static PersistTable< String, Object > Settings { get; } = new PersistTable< String, Object >( SettingsFolder, nameof( Settings ) );
        public const String Passphrase = "passphrase";
        public const String Apikey = "apikey";
        public const String Secret = "secret";

        public static String Ask( String key, String question ) {
            if ( question == null ) {
                throw new ArgumentNullException( nameof( question ) );
            }
            if ( key == null ) {
                throw new ArgumentNullException( nameof( key ) );
            }
            Object value;
            while ( !Settings.TryGetValue( key, out value ) ) {
                var questionBox = new QuestionBox( question );
                var result = questionBox.ShowDialog();
                if ( result == DialogResult.OK ) {
                    Settings[ key ] = questionBox.Response;
                }
                else {
                    if ( ThreadingExtensions.IsRunningFromNUnit ) {
                        throw new NotImplementedException();
                    }
                }
            }
            return value as String;
        }
    }
}
