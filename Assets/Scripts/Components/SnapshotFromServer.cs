using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public unsafe struct SnapshotFromServer : IComponentData
    {
        public uint tick;
        public long time;
        public int rtt;
        public int lastAcknowlegdedCommandTime;
     //   public int length;
     //   public uint* data;

     //   public EntityPredictData predictData;


    }

}