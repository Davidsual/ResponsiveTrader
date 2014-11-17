using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using ResponsiveTrader.Server.Sender;

namespace ResponsiveTrader.Server.ViewModels
{
    public class ShellViewModel : ViewModelBase,IShellViewModel
    {
        private readonly ISender _sender;
        private int _rateSent = 0;
        private int _lastRateSent = 0;
        private IDisposable _myLocalSubscription = null;

        public ShellViewModel([Dependency]ISender sender)
        {
            _sender = sender;
            ;
            StartStramViewModelCommand = new SimpleCommand(x => _myLocalSubscription = _sender.StartSending(500).Subscribe(val =>
            {
                RateSent = val;
                LastRateSent = val;
            } ));

            StopStramViewModelCommand = new SimpleCommand(x =>
            {
                _sender.StopSending();
                _rateSent = 0;
                OnPropertyChanged("RateSent");

                LastRateSent = 0;

                if (_myLocalSubscription != null)
                {
                    _myLocalSubscription.Dispose();
                    _myLocalSubscription = null;
                }
               
            });
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
