using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Core.Types
{
    /// <summary>
    /// An event which can be raised
    /// </summary>
    public class Event
    {
        public string Name { get; set; }

        public Event(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
