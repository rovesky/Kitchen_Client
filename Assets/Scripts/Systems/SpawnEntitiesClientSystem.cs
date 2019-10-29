using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
   
    [DisableAutoCreation]
    public class SpawnEntitiesClientSystem : ComponentSystem, ISnapshotConsumer
    {

        private Entity rocketEnemy;
        private Entity rocketPlayer;
        private Entity player;

        private GameObject enemy1Prefab;
        private GameObject enemy2Prefab;
 
        private ReadSnapshotSystem readSnapshotSystem;
        private NetworkClientNewSystem networkClientNewSystem;
        private EntityQuery spawnEntitiesQuery;
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
                   


        //public void AddEntity(int id, Entity e)
        //{
        //    entities[id] = e;
        //}

        public void ProcessEntitySpawn(int serverTime, int id, ushort typeId)
        {
            //var spawnEntitiesBuffer = EntityManager.GetBuffer<SpawnEntityBuffer>(GetSingletonEntity<SpawnEntitiesClient>());
            //FSLog.Info($"ProcessEntitySpawn,id:{id},type:{typeId}");
            //spawnEntitiesBuffer.Add(new SpawnEntityBuffer() {
            //    id = id,
            //    type = (EntityType)typeId,
            //    pos = Vector3.zero,
            //    rotation = Quaternion.identity
            //});

            Entity e = Entity.Null;
            if ((EntityType)typeId == EntityType.Enemy1)
            {
                e = SpawnEntityUtil.SpwanEnemy(EntityManager, enemy1Prefab, EnemyType.Normal,
                     Vector3.zero, rocketEnemy);
                EntityManager.AddComponentData(e, new Explosion());
                EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity
                });
            }
            else if ((EntityType)typeId == EntityType.Enemy2)
            {
                e = SpawnEntityUtil.SpwanEnemy(EntityManager, enemy2Prefab, EnemyType.Super,
                  Vector3.zero, rocketEnemy);
                EntityManager.AddComponentData(e, new Explosion());
                EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity
                });
            }
            else if ((EntityType)typeId == EntityType.Player)
            {
             //   FSLog.Info($"Player create:{entityBuffer.type}");
                e = SpawnEntityUtil.SpwanPlayer(EntityManager, id, player,
                       Vector3.zero, rocketPlayer);

                EntityManager.AddComponentData(e, new LocalPlayer());
                EntityManager.AddComponentData(e, new UserCommand());
                EntityManager.AddComponentData(e, new Explosion());
                EntityManager.AddComponentData(e, new UpdateUI());

                var rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, -180, 0);
                EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = rotation
                });
            }
            entities[id] = e;
        }

        public void ProcessEntityDespawns(int serverTime, List<int> despawns)
        {
            foreach (var id in despawns)
            {
                EntityManager.AddComponentData(entities[id], new Despawn() { Frame = 0 });
                entities.Remove(id);
            }
        }


        public void ProcessEntityUpdate(int serverTime, int id, ref NetworkReader reader)
        {
          

            if (!entities.ContainsKey(id))
                return;



         //   FSLog.Info($"player Update:{id}!");
            var entity = entities[id];
            if (EntityManager.HasComponent<Player>(entity))
            {
               // FSLog.Info($"player Update!");
                var player = EntityManager.GetComponentData<Player>(entity);
                player.id = reader.ReadInt32();
                player.playerId = reader.ReadInt32();

                //var predictData = EntityManager.GetComponentData<EntityPredictData>(entity);
                //predictData.position = reader.ReadVector3Q();
                //EntityManager.SetComponentData(entity, predictData);
                var snapshotFromServer = GetSingleton<SnapshotFromServer>();
                snapshotFromServer.predictData.position = reader.ReadVector3Q();
                var rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, -180, 0);
                snapshotFromServer.predictData.rotation = rotation;
                SetSingleton(snapshotFromServer);


             //   FSLog.Info($"player Update:{player.id},{id},[{predictData.position.x},{predictData.position.y},{predictData.position.z}]");
                var health = EntityManager.GetComponentData<Health>(entity);
                health.Value = reader.ReadInt32();
                EntityManager.SetComponentData(entity, health);

                var score = EntityManager.GetComponentData<Score>(entity);
                score.ScoreValue = reader.ReadInt32();
                score.MaxScoreValue = reader.ReadInt32();
                EntityManager.SetComponentData(entity, score);
            }
            else if (EntityManager.HasComponent<Enemy>(entity))
            {
                var enemy = EntityManager.GetComponentData<Enemy>(entity);
                enemy.id = reader.ReadInt32();
                enemy.type = (EnemyType)reader.ReadByte();


                var predictData = EntityManager.GetComponentData<EntityPredictData>(entity);
                predictData.position = reader.ReadVector3();
                EntityManager.SetComponentData(entity, predictData);

                var health = EntityManager.GetComponentData<Health>(entity);
                health.Value = reader.ReadInt32();
                EntityManager.SetComponentData(entity, health);


                var attack = EntityManager.GetComponentData<Attack>(entity);
                attack.Power = reader.ReadInt32();
                EntityManager.SetComponentData(entity, attack);

                if (enemy.type == EnemyType.Super)
                {
                    var fireRocket = EntityManager.GetComponentData<FireRocket>(entity);
                    fireRocket.FireCooldown = reader.ReadFloatQ();
                    fireRocket.RocketTimer = reader.ReadFloatQ();
                }
            }
        }

        protected override void OnCreate()
        {

            spawnEntitiesQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEntitiesClient>());
            var entity = EntityManager.CreateEntity(typeof(SpawnEntitiesClient));
            spawnEntitiesQuery.SetSingleton(new SpawnEntitiesClient());
            EntityManager.AddBuffer<SpawnEntityBuffer>(entity);          


            readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();
            networkClientNewSystem = World.GetOrCreateSystem<NetworkClientNewSystem>();

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
          /*  var entity = spawnEntitiesQuery.GetSingletonEntity();

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
                    FSLog.Info($"Player create:{entityBuffer.type}");
                    e = SpawnEntityUtil.SpwanPlayer(EntityManager, entityBuffer.id, player,
                           entityBuffer.pos, rocketPlayer);

                    EntityManager.AddComponentData(e, new LocalPlayer());
                    EntityManager.AddComponentData(e, new UserCommand());
                    EntityManager.AddComponentData(e, new Explosion());
                    EntityManager.AddComponentData(e, new UpdateUI());

                    var rotation = Quaternion.identity;
                    rotation.eulerAngles = new Vector3(0, -180, 0);                
                    EntityManager.AddComponentData(e, new EntityPredictData()
                    {
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
                networkClientNewSystem.AddEntity(entityBuffer.id, e);
            }

            array.Dispose();*/

        }
    }
}
