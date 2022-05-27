using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.Common;

namespace SampleBase
{
    public interface INodeData : IWeighted, IColored, IPositioned
    {
    }
}