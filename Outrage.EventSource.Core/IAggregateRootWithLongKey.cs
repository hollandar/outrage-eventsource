using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public interface IAggregateRootWithLongKey
    {
        long Id { get; set; }
    }
}
