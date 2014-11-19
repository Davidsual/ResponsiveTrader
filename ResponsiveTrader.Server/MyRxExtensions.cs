using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using ResponsiveTrader.Shared;

namespace ResponsiveTrader.Server
{
    public static class MyRxExtensions
    {
        public static IObservable<int> CreatePublisherSequence(this IObservable<int> source, string serverUrl,  NetMQContext _context)
        {
            return Observable.Create<int>(o =>
            {
                Random random = new Random();

                PublisherSocket publisherSocket = _context.CreatePublisherSocket();
                publisherSocket.Bind(serverUrl);
                NetMqDisposable netMqDisposable = new NetMqDisposable(publisherSocket);

                var sourceSubs = source.Subscribe(
                    //on next
                    value =>
                    {
                        o.OnNext(value);

                        publisherSocket.Send(Utils.ObjectToByteArray(new RateDto()
                        {
                            RateDate = DateTime.Now,
                            RateName = "Test my rate",
                            RateValue = value
                        }));
                    },
                    //on error
                    ex =>
                    {
                        o.OnCompleted();
                    },
                    //on complete
                    () =>
                    {
                        Console.WriteLine("Status subscription completed");
                        o.OnCompleted();
                    });

                return new CompositeDisposable { sourceSubs, netMqDisposable };
            });
        }

    }

    public class NetMqDisposable : IDisposable
    {
        private readonly PublisherSocket _theSocket;

        public NetMqDisposable(PublisherSocket theSocket)
        {
            _theSocket = theSocket;
        }

        public void Dispose()
        {
            try
            {
                _theSocket.Close();
                _theSocket.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

}
