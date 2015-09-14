using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.AsyncLib
{
    internal class ValueQueue<V> : NodeQueue<IValue<V>> where V : IEquatable<V>
    {

    }

    internal class ValueQueueOrdered<V> : NodeQueueOrdered<IOrdered<V>> where V : IComparable<V>, IEquatable<V>
    {

    }
}
