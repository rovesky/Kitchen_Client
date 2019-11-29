using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace FootStone.Kitchen
{
    public enum EntityType
    {
        Player,
        Plate
    }

    [DisableAutoCreation]
    public class ReplicateEntitySystemGroup : NoSortComponentSystemGroup
    {
        public ReplicatedEntityClient EntityClient { get; } = new ReplicatedEntityClient(World.Active);
        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer() { playerId = -1, playerEntity = Entity.Null });

         //   replicatedEntityClient = new ReplicatedEntityClient(World.Active);
         
           // FSLog.Error($"RegisterFactorys");
            EntityClient.RegisterFactory((ushort)EntityType.Player,new CharacterFactory());
            EntityClient.RegisterFactory((ushort)EntityType.Plate, new PlateFactory());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>());

        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            Interpolate();
        }

        public void SetLocalPlayerId(int id)
        {
            World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>().SetLocalPlayerId(id);
        }

        public void Interpolate()
        {
            EntityClient.Interpolate(GetSingleton<WorldTime>().GameTick);
        }

        public void Rollback()
        {
            EntityClient.Rollback();
        }
    }

}
