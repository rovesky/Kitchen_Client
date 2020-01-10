using System.Collections.Generic;
using System.Linq;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Profiling;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class ReplicateEntitySystemGroup : NoSortComponentSystemGroup, ISnapshotConsumer
    {
        private ReplicatedEntityFactoryManager factoryManager;
        private ReplicatedEntityCollection replicatedEntities;
        private EntityQuery sceneEntityQuery;
        private WorldSceneEntitiesSystem worldSceneEntitiesSystem;

        public void ProcessEntityDespawns(int serverTick, List<int> despawns)
        {
            foreach (var id in despawns)
            {
                var entity = replicatedEntities.Unregister(id);

                EntityManager.AddComponentData(entity, new Despawn() { Frame = 0 });
                if (EntityManager.HasComponent<Character>(entity))
                {
                    var animEntity = EntityManager.GetComponentData<Character>(entity).PresentationEntity;
                    EntityManager.AddComponentData(animEntity, new Despawn() { Frame = 0 });
                }
            }
        }

        public void ProcessEntitySpawn(int serverTick, int id, ushort typeId)
        {
            FSLog.Info("ProcessEntitySpawns. Server tick:" + serverTick + " id:" + id + " typeid:" + typeId);

            Profiler.BeginSample("ReplicatedEntitySystemClient.ProcessEntitySpawns()");

            if (id < worldSceneEntitiesSystem.SceneEntities.Count)
            {
                replicatedEntities.Register(id, worldSceneEntitiesSystem.SceneEntities[id]);
                return;
            }

            var factory = factoryManager.GetFactory(typeId);
            if (factory == null)
                return;

            var entity = factory.Create(EntityManager, null, null);
            if (entity == Entity.Null)
                return;

            var replicatedDataEntity = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedDataEntity.Id = id;
            EntityManager.SetComponentData(entity, replicatedDataEntity);

            replicatedEntities.Register(id, entity);

            Profiler.EndSample();   
        }

        public void ProcessEntityUpdate(int serverTick, int id, ref NetworkReader reader)
        {
            replicatedEntities.ProcessEntityUpdate(serverTick, id, ref reader);

            //Update PlayerEntity
            var entity = replicatedEntities.GetEntity(id);
            var replicatedData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            var localPlayer = GetSingleton<LocalPlayer>();

            if (EntityManager.HasComponent<Character>(entity) &&
                localPlayer.PlayerEntity == Entity.Null &&
                replicatedData.PredictingPlayerId == localPlayer.PlayerId)
            {
                localPlayer.PlayerEntity = entity;
                SetSingleton(localPlayer);
            }
        }

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, PlayerEntity = Entity.Null});

            replicatedEntities = new ReplicatedEntityCollection(EntityManager);
            factoryManager = new ReplicatedEntityFactoryManager();

         
            factoryManager.RegisterFactory((ushort) EntityType.Character, new CharacterFactory());
            factoryManager.RegisterFactory((ushort) EntityType.Plate, new PlateFactory());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>());
            worldSceneEntitiesSystem = World.GetOrCreateSystem<WorldSceneEntitiesSystem>();
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
            replicatedEntities.Interpolate(GetSingleton<WorldTime>().GameTick);
        }

        public void Rollback()
        {
            replicatedEntities.Rollback();
        }
    }
}