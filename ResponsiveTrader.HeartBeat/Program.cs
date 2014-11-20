using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.zmq;
using Poller = NetMQ.Poller;

namespace ResponsiveTrader.HeartBeat
{

    class Program
    {
        private static void Main(string[] args)
        {


            Observable.Create<int>(o =>
            {
                NetMQContext context = NetMQContext.Create();

                var req = context.CreateRequestSocket();

                req.Connect("tcp://127.0.0.1:5002");

                Observable
                    .Interval(TimeSpan.FromMilliseconds(2000))
                    .Delay(TimeSpan.FromMilliseconds(100))
                    //.Timeout(TimeSpan.FromMilliseconds(600))

                    .Select(i => i)
                    .Subscribe(i =>
                    {
                        req.Send("Are you there?");
                        Console.WriteLine("Are you there? " + i);
                        bool more2;

                        string m1 = req.ReceiveString(out more2);

                        Console.WriteLine(m1);
                    });
                return new CompositeDisposable(context, req);
            }).Publish().Connect();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
