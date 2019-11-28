using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct ServerSnapshot : IComponentData
    {
        public uint tick;
        public long time;
        public int rtt;
        public int lastAcknowlegdedCommandTime;
    }

}