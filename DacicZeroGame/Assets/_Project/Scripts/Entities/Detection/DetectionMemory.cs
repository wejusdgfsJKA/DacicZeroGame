using Entity;
using System.Collections.Generic;
namespace Detection
{
    public class DetectionMemory
    {
        public HashSet<EntityBase> Targets { get; protected set; } = new();
    }
}