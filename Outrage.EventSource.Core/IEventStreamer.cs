using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public interface IEventStreamer
    {
        Task StreamEventAsync<TEventType> (TEventType evt);
    }
}
