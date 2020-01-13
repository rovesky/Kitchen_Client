using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ServerSnapshot : IComponentData
    {
        public uint ServerTick;
        public long TimeSinceSnapshot;
        public int Rtt;
        public int LastAcknowledgedTick;
    }
}