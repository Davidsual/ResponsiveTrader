using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ResponsiveTrader.Server
{

    public static class Utils
    {
        public static byte[] ObjectToByteArray<T>(T obj)
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
        
    }

}
