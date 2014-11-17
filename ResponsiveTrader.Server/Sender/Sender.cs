using NetMQ;
using NetMQ.Sockets;
using ResponsiveTrader.Shared;
using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ResponsiveTrader.Server.Sender
{
    public class Sender : ISender, IDisposable
    {
        private readonly string _serverUrl;
        private IDisposable _rxSender;
        private NetMQContext _context;
        private PushSocket _pushSocket;

        public Sender(string serverUrl)
        {
            _serverUrl = serverUrl;

        }

        public IObservable<int> StartSending(long milliseconds)
        {
            var random = new Random();
            _context = NetMQContext.Create();

            _pushSocket = _context.CreatePushSocket();
            _pushSocket.Bind(_serverUrl);


            var source = Observable
                  .Interval(TimeSpan.FromMilliseconds(milliseconds))
                  .Delay(TimeSpan.FromMilliseconds(100))
                  .Select(_ => random.Next(1, 1000));

            _rxSender = source.ObserveOn(NewThreadScheduler.Default).Subscribe(value => _pushSocket.Send(ObjectToByteArray(new RateDto()
            {
                RateDate = DateTime.Now,
                RateName = "Test my rate",
                RateValue = value
            })));

            return source;
        }

        public void StopSending()
        {
            this.Dispose();
        }


        private byte[] ObjectToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;

            MemoryStream ms = new MemoryStream();
            XmlSerializer xmlS = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTW = new XmlTextWriter(ms, Encoding.UTF8);

            xmlS.Serialize(xmlTW, obj);
            ms = (MemoryStream)xmlTW.BaseStream;

            return ms.ToArray();
        }

        public void Dispose()
        {

            try
            {
                _rxSender.Dispose();               
                _pushSocket.Close();
                _pushSocket.Dispose();               
                _context.Terminate();
                _context.Dispose();                
            }
            catch
            {

            }
            finally
            {
                _rxSender = null;
                _pushSocket = null;
                _context = null;
            }
        }
    }
}
