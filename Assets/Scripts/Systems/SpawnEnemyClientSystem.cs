using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    //[UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public class SpawnEnemyClientSystem : ComponentSystem
    {

        private Entity rocket;
        private GameObject enemy1Prefab;
        private GameObject enemy2Prefab;
        private ReadSnapshotSystem readSnapshotSystem;

        protected override void OnCreate()
        {
            FSLog.Info($" spwan enemy OnCreate1");
            var spawnEnemyQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEnemyClient>());
            var entity = EntityManager.CreateEntity(typeof(SpawnEnemyClient));
            spawnEnemyQuery.SetSingleton(new SpawnEnemyClient());
            EntityManager.AddBuffer<EnemyBuffer>(entity);

            readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();


            var prefab = Resources.Load("Prefabs/EnemyRocket") as GameObject;       
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, World.Active);

            enemy1Prefab = Resources.Load("Prefabs/Enemy1") as GameObject;
            enemy2Prefab = Resources.Load("Prefabs/Enemy3") as GameObject;

            FSLog.Info($" spwan enemy OnCreate2");
        }

        protected override void OnUpdate()
        {
         
            Entities.ForEach(
               (Entity entity,ref SpawnEnemyClient spawn) =>
               {
                   var buffer = EntityManager.GetBuffer<EnemyBuffer>(entity);
                   if (buffer.Length == 0)
                       return;

                   var array = buffer.ToNativeArray(Unity.Collections.Allocator.Temp);
                   buffer.Clear();
                  // FSLog.Info($"buffer.Length ({buffer.Length});");               
                   for (int i = 0; i < array.Length; ++i)
                   {
                       var enemyBuffer = array[i];
                    //   FSLog.Info($" spwan enemy ({enemyBuffer.id});");
                       var enemyPrefab = enemyBuffer.type == EnemyType.Normal ? enemy1Prefab : enemy2Prefab;
                       var e = SpawnEnemyUtil.SpwanEnemy(EntityManager, enemyPrefab, enemyBuffer.type,
                             enemyBuffer.pos, rocket);

                       EntityManager.AddComponentData(e,new Explosion());            

                       readSnapshotSystem.AddEntity(enemyBuffer.id, e);
                   }

                   array.Dispose();
               });
        }
    }
}
