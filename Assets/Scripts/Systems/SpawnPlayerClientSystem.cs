using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class SpawnPlayerClientSystem : ComponentSystem
    {

        private Entity rocket;
        private Entity player;   
        private ReadSnapshotSystem readSnapshotSystem;

        protected override void OnCreate()
        {
            FSLog.Info($" spwan player OnCreate1");
            var spawnPlayerQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnPlayerClient>());
            var entity = EntityManager.CreateEntity(typeof(SpawnPlayerClient));
            spawnPlayerQuery.SetSingleton(new SpawnPlayerClient());
            EntityManager.AddBuffer<PlayerClientBuffer>(entity);

            readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();

         
            var prefab = Resources.Load("Prefabs/Rocket") as GameObject;       
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);

            var playerPrefab = Resources.Load("Prefabs/Player") as GameObject;
            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, World.Active);

            FSLog.Info($" spwan player OnCreate2");
        }

        protected override void OnUpdate()
        {
         
            Entities.ForEach(
               (Entity entity,ref SpawnPlayerClient spawn) =>
               {
                   var buffer = EntityManager.GetBuffer<PlayerClientBuffer>(entity);
                   if (buffer.Length == 0)
                       return;

                   var array = buffer.ToNativeArray(Unity.Collections.Allocator.Temp);
                   buffer.Clear();
                  // FSLog.Info($"buffer.Length ({buffer.Length});");               
                   for (int i = 0; i < array.Length; ++i)
                   {
                       var playerBuffer = array[i];             
                    
                       var e = SpawnEnemyUtil.SpwanPlayer(EntityManager, playerBuffer.id, player, 
                             playerBuffer.pos, rocket);

                       EntityManager.AddComponentData(e,new Explosion());            

                       readSnapshotSystem.AddEntity(playerBuffer.id, e);
                   }

                   array.Dispose();
               });
        }
    }
}
