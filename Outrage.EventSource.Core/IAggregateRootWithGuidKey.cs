using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public interface IAggregateRootWithGuidKey
    {
        Guid Id { get; set; }
    }
}
