using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
   
    [DisableAutoCreation]
    public class SpawnEntitiesClientSystem : ComponentSystem
    {

        private Entity rocketEnemy;
        private Entity rocketPlayer;
        private Entity player;

        private GameObject enemy1Prefab;
        private GameObject enemy2Prefab;
 
        private ReadSnapshotSystem readSnapshotSystem;
        private EntityQuery spawnEntitiesQuery;

        protected override void OnCreate()
        {

            spawnEntitiesQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEntitiesClient>());
            var entity = EntityManager.CreateEntity(typeof(SpawnEntitiesClient));
            spawnEntitiesQuery.SetSingleton(new SpawnEntitiesClient());
            EntityManager.AddBuffer<SpawnEntityBuffer>(entity);

            readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();
 
            rocketEnemy = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Prefabs/EnemyRocket") as GameObject, World.Active);

            rocketPlayer = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Prefabs/PlayerRocket") as GameObject, World.Active);


            enemy1Prefab = Resources.Load("Prefabs/Enemy1") as GameObject;
            enemy2Prefab = Resources.Load("Prefabs/Enemy3") as GameObject;
          
         
            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Prefabs/Player") as GameObject, World.Active);

            FSLog.Info($" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
            var entity = spawnEntitiesQuery.GetSingletonEntity();

            var buffer = EntityManager.GetBuffer<SpawnEntityBuffer>(entity);
            if (buffer.Length == 0)
                return;

            var array = buffer.ToNativeArray(Unity.Collections.Allocator.Temp);
            buffer.Clear();

            for (int i = 0; i < array.Length; ++i)
            {
                var entityBuffer = array[i];
                //   FSLog.Info($" spwan enemy ({enemyBuffer.id});");

                Entity e = Entity.Null;
                if (entityBuffer.type == EntityType.Enemy1)
                {
                    e = SpawnEntityUtil.SpwanEnemy(EntityManager, enemy1Prefab, EnemyType.Normal,
                         entityBuffer.pos, rocketEnemy);
                    EntityManager.AddComponentData(e, new Explosion());
                    EntityManager.AddComponentData(e, new EntityPredictData()
                    {
                        position = entityBuffer.pos,
                        rotation = Quaternion.identity
                    });
                }
                else if (entityBuffer.type == EntityType.Enemy2)
                {
                    e = SpawnEntityUtil.SpwanEnemy(EntityManager, enemy2Prefab, EnemyType.Super,
                      entityBuffer.pos, rocketEnemy);
                    EntityManager.AddComponentData(e, new Explosion());
                    EntityManager.AddComponentData(e, new EntityPredictData()
                    {
                        position = entityBuffer.pos,
                        rotation = Quaternion.identity
                    });
                }
                else if (entityBuffer.type == EntityType.Player)
                {
                    e = SpawnEntityUtil.SpwanPlayer(EntityManager, entityBuffer.id, player,
                           entityBuffer.pos, rocketPlayer);

                    EntityManager.AddComponentData(e, new LocalPlayer());
                    EntityManager.AddComponentData(e, new UserCommand());
                    EntityManager.AddComponentData(e, new Explosion());
                    EntityManager.AddComponentData(e, new UpdateUI());
                    var rotation = Quaternion.identity;
                    rotation.eulerAngles = new Vector3(0, -180, 0);
                    EntityManager.AddComponentData(e, new EntityPredictData(){
                        position = entityBuffer.pos,
                        rotation = rotation

                    });
                }
                else if (entityBuffer.type == EntityType.RocketPlayer)
                {
                    // FSLog.Info($"rocketType create:{entityBuffer.type}");
                    e = EntityManager.Instantiate(rocketPlayer);

                    Translation position = new Translation() { Value = entityBuffer.pos };
                    Rotation rotation = new Rotation() { Value = entityBuffer.rotation };

                    EntityManager.SetComponentData(e, position);
                    EntityManager.SetComponentData(e, rotation);
                    EntityManager.AddComponentData(e, new Rocket() { id = entityBuffer.id, Type = RocketType.Player });
                    EntityManager.AddComponentData(e, new Health() { Value = 1 });
                    EntityManager.AddComponentData(e, new Damage());
                    EntityManager.AddComponentData(e, new Attack() { Power = 20 });
                    EntityManager.AddComponentData(e, new MoveForward() { Speed = 6 });

                    PostUpdateCommands.AddComponent(e, new EntityPredictData()
                    {
                        position = position.Value,
                        rotation = rotation.Value
                    });
                }
                else if (entityBuffer.type == EntityType.RocketEnemy)
                {
                    e = EntityManager.Instantiate(rocketEnemy);

                    Translation position = new Translation() { Value = entityBuffer.pos };
                    Rotation rotation = new Rotation() { Value = entityBuffer.rotation };

                    EntityManager.SetComponentData(e, position);
                    EntityManager.SetComponentData(e, rotation);
                    EntityManager.AddComponentData(e, new Rocket() { id = entityBuffer.id, Type = RocketType.Enemy });
                    EntityManager.AddComponentData(e, new Attack() { Power = 1 });
                    EntityManager.AddComponentData(e, new Health() { Value = 1 });
                    EntityManager.AddComponentData(e, new Damage());
                    EntityManager.AddComponentData(e, new MoveForward() { Speed = 3 });
                    PostUpdateCommands.AddComponent(e, new EntityPredictData()
                    {
                        position = position.Value,
                        rotation = rotation.Value
                    });
                }

                readSnapshotSystem.AddEntity(entityBuffer.id, e);
            }

            array.Dispose();

        }
    }
}
