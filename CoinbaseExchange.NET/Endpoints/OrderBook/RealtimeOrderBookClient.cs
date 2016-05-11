namespace CoinbaseExchange.NET.Endpoints.OrderBook {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Newtonsoft.Json.Linq;

    public class RealtimeOrderBookClient : ExchangeClientBase {
        private const String Product = "BTC-USD";
        private readonly Object _askLock = new Object();
        private readonly Object _bidLock = new Object();

        private readonly Object _spreadLock = new Object();

        public RealtimeOrderBookClient( CBAuthenticationContainer auth ) : base( auth ) {
            _sells = new List< BidAskOrder >();
            _buys = new List< BidAskOrder >();

            Sells = new List< BidAskOrder >();
            Buys = new List< BidAskOrder >();

            ResetStateWithFullOrderBook();
        }

        private List< BidAskOrder > _sells { get; set; }
        private List< BidAskOrder > _buys { get; set; }

        public List< BidAskOrder > Sells { get; set; }
        public List< BidAskOrder > Buys { get; set; }

        public Decimal Spread {
            get {
                lock ( _spreadLock ) {
                    if ( !Buys.Any() || !Sells.Any() ) {
                        return 0;
                    }

                    var maxBuy = Buys.Select( x => x.Price ).Max();
                    var minSell = Sells.Select( x => x.Price ).Min();

                    return minSell - maxBuy;
                }
            }
        }

        public event EventHandler Updated;

        private async void ResetStateWithFullOrderBook() {
            var response = await GetProductOrderBook( Product, 3 );

            lock ( _spreadLock ) {
                lock ( _askLock ) {
                    lock ( _bidLock ) {
                        _buys = response.Buys.ToList();
                        _sells = response.Sells.ToList();

                        Buys = _buys.ToList();
                        Sells = _sells.ToList();
                    }
                }
            }

            OnUpdated();

            Subscribe( Product, OnOrderBookEventReceived );
        }

        private void OnUpdated() {
            if ( Updated != null ) {
                Updated( this, new EventArgs() );
            }
        }

        public async Task< GetProductOrderBookResponse > GetProductOrderBook( String productId, Int32 level = 1 ) {
            var request = new GetProductOrderBookRequest( productId, level );
            var response = await this.GetResponse( request );
            var orderBookResponse = new GetProductOrderBookResponse( response );
            return orderBookResponse;
        }

        private void OnOrderBookEventReceived( RealtimeMessage message ) {
            if ( message is RealtimeReceived ) {
                var receivedMessage = message as RealtimeReceived;
                OnReceived( receivedMessage );
            }

            else if ( message is RealtimeOpen ) {}

            else if ( message is RealtimeDone ) {
                var doneMessage = message as RealtimeDone;
                OnDone( doneMessage );
            }

            else if ( message is RealtimeMatch ) {}

            else if ( message is RealtimeChange ) {}

            else if ( message is RealtimeOpen ) {}

            OnUpdated();
        }

        private void OnReceived( RealtimeReceived receivedMessage ) {
            var order = new BidAskOrder();

            order.Id = receivedMessage.OrderId;
            order.Price = receivedMessage.Price;
            order.Size = receivedMessage.Size;

            lock ( _spreadLock ) {
                if ( receivedMessage.Side == "buy" ) {
                    lock ( _bidLock ) {
                        _buys.Add( order );
                        Buys = _buys.ToList();
                    }
                }

                else if ( receivedMessage.Side == "sell" ) {
                    lock ( _askLock ) {
                        _sells.Add( order );
                        Sells = _sells.ToList();
                    }
                }
            }
        }

        private void OnDone( RealtimeDone message ) {
            lock ( _spreadLock ) {
                lock ( _askLock ) {
                    lock ( _bidLock ) {
                        _buys.RemoveAll( b => b.Id == message.OrderId );
                        _sells.RemoveAll( a => a.Id == message.OrderId );

                        Buys = _buys.ToList();
                        Sells = _sells.ToList();
                    }
                }
            }
        }

        private static async void Subscribe( String product, Action< RealtimeMessage > onMessageReceived ) {
            if ( String.IsNullOrWhiteSpace( product ) ) {
                throw new ArgumentNullException( nameof( product ) );
            }

            if ( onMessageReceived == null ) {
                throw new ArgumentNullException( nameof( onMessageReceived ), "Message received callback must not be null." );
            }

            var uri = new Uri( "wss://ws-feed.exchange.coinbase.com" );
            var webSocketClient = new ClientWebSocket();
            var cancellationToken = new CancellationToken();
            var requestString = String.Format( @"{{""type"": ""subscribe"",""product_id"": ""{0}""}}", product );
            var requestBytes = Encoding.UTF8.GetBytes( requestString );
            await webSocketClient.ConnectAsync( uri, cancellationToken );

            if ( webSocketClient.State == WebSocketState.Open ) {
                var subscribeRequest = new ArraySegment< Byte >( requestBytes );
                var sendCancellationToken = new CancellationToken();
                await webSocketClient.SendAsync( subscribeRequest, WebSocketMessageType.Text, true, sendCancellationToken );

                while ( webSocketClient.State == WebSocketState.Open ) {
                    var receiveCancellationToken = new CancellationToken();
                    var receiveBuffer = new ArraySegment< Byte >( new Byte[ 1024 * 1024 * 5 ] ); // 5MB buffer
                    var webSocketReceiveResult = await webSocketClient.ReceiveAsync( receiveBuffer, receiveCancellationToken );
                    if ( webSocketReceiveResult.Count == 0 ) {
                        continue;
                    }

                    var jsonResponse = Encoding.UTF8.GetString( receiveBuffer.Array, 0, webSocketReceiveResult.Count );
                    var jToken = JToken.Parse( jsonResponse );

                    var typeToken = jToken[ "type" ];
                    if ( typeToken == null ) {
                        continue;
                    }

                    var type = typeToken.Value< String >();
                    RealtimeMessage realtimeMessage = null;

                    switch ( type ) {
                        case "received":
                            realtimeMessage = new RealtimeReceived( jToken );
                            break;
                        case "open":
                            realtimeMessage = new RealtimeOpen( jToken );
                            break;
                        case "done":
                            realtimeMessage = new RealtimeDone( jToken );
                            break;
                        case "match":
                            realtimeMessage = new RealtimeMatch( jToken );
                            break;
                        case "change":
                            realtimeMessage = new RealtimeChange( jToken );
                            break;
                        default:
                            break;
                    }

                    if ( realtimeMessage == null ) {
                        continue;
                    }

                    onMessageReceived( realtimeMessage );
                }
            }
        }
    }
}
