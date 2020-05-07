using System.Collections.Generic;
using FootStone.ECS;
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

            var entity = factory.Create(EntityManager, null, null,typeId);
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
                localPlayer.CharacterEntity == Entity.Null &&
                replicatedData.PredictingPlayerId == localPlayer.PlayerId)
            {
                EntityManager.AddComponentData(entity, new LocalCharacter());
                localPlayer.CharacterEntity = entity;
              
                SetSingleton(localPlayer);
            }
        }

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, CharacterEntity = Entity.Null});

            replicatedEntities = new ReplicatedEntityCollection(EntityManager);
            factoryManager = new ReplicatedEntityFactoryManager();

            var itemFactory = new ItemFactory();
         
            factoryManager.RegisterFactory((ushort) EntityType.Character, new CharacterFactory());
            factoryManager.RegisterFactory((ushort) EntityType.Plate,  itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.PlateDirty, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Shrimp, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.ShrimpSlice, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.ShrimpProduct, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Cucumber, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.CucumberSlice, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Rice, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.RiceCooked, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.KelpSlice, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Sushi, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Pot, itemFactory);
       //     factoryManager.RegisterFactory((ushort) EntityType.PotFull, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Extinguisher, itemFactory);
            factoryManager.RegisterFactory((ushort) EntityType.Game, new GameFactory());
            factoryManager.RegisterFactory((ushort) EntityType.Menu, new MenuFactory());

        //    m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>());
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