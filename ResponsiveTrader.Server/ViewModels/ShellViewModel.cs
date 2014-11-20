using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using NetMQ;
using NetMQ.Sockets;
using NetMQ.zmq;
using ResponsiveTrader.Server.Sender;
using Poller = NetMQ.Poller;

namespace ResponsiveTrader.Server.ViewModels
{
    public class ShellViewModel : ViewModelBase, IShellViewModel
    {
        private readonly ISender _sender;
        private int _rateSent = 0;
        private int _lastRateSent = 0;
        private IDisposable _myLocalSubscription = null;

        private NetMQContext _context = NetMQContext.Create();




        public ShellViewModel([Dependency]ISender sender)
        {
            _sender = sender;

            StartStramViewModelCommand = new SimpleCommand(x =>
            {
                _myLocalSubscription = _sender.Init().CreatePublisherSequence(ConfigurationManager.AppSettings["ServerUrl"], _context)
                    .Subscribe(val =>
                    {
                        RateSent = val;
                        LastRateSent = val;
                    });
            });

            StopStramViewModelCommand = new SimpleCommand(x =>
            {
                Task.Run(() =>
                {
                    _myLocalSubscription.Dispose();
                });

            });

            Observable.Create<CompositeDisposable>(i =>
            {
                var rep = _context.CreateResponseSocket();

                Poller poller = new Poller();

                rep.Bind("tcp://127.0.0.1:5002");

                rep.ReceiveReady += (s, a) =>
                {
                    bool more;
                    string m = a.Socket.ReceiveString(out more);

                    a.Socket.Send("Yes");
                };

                poller.AddSocket(rep);

                Task pollerTask = Task.Factory.StartNew(poller.Start);

                return new CompositeDisposable(rep, poller);
            }).Publish().Connect();



        }

        public ICommand StartStramViewModelCommand { get; set; }
        public ICommand StopStramViewModelCommand { get; set; }

        public int RateSent
        {
            get { return _rateSent; }
            set
            {
                ++_rateSent;
                OnPropertyChanged("RateSent");
            }
        }

        public int LastRateSent
        {
            get { return _lastRateSent; }
            set
            {
                _lastRateSent = value;
                OnPropertyChanged("LastRateSent");
            }
        }
    }
}
