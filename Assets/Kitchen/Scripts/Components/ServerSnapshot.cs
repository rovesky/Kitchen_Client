using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ServerSnapshot : IComponentData
    {
        public uint Tick;
        public long Time;
        public int Rtt;
        public int LastAcknowledgedTick;
    }
}