﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsiveTrader.Server.Sender
{
    public interface ISender
    {
        IObservable<int> Init();
    }
}
