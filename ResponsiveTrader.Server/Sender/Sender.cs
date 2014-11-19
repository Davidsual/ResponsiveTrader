using NetMQ;
using NetMQ.Sockets;
using ResponsiveTrader.Shared;
using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ResponsiveTrader.Server.Sender
{
    public class Sender : ISender
    {
        private readonly string _serverUrl;
        private Random random = new Random();

        public IObservable<int> Init()
        {
           return Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .Delay(TimeSpan.FromMilliseconds(100))
                .Select(_ => random.Next(1, 1000));
        }
    }
}
