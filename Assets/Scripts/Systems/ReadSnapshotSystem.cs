using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public unsafe class ReadSnapshotSystem : ComponentSystem
    {
        private Dictionary<int,Entity> entities = new Dictionary<int, Entity>();  

        private EntityQuery snapShotQuery;
        private EntityQuery spawnEntitiesyQuery;
     
        protected override void OnCreate()
        {
            snapShotQuery = GetEntityQuery(ComponentType.ReadWrite<Snapshot>());

            if (snapShotQuery.CalculateEntityCount() == 0)
            {
                var entity = EntityManager.CreateEntity(typeof(Snapshot));
                snapShotQuery.SetSingleton(new Snapshot());
                EntityManager.AddBuffer<SnapshotTick>(entity);  
                FSLog.Debug("Snapshot create!");
            }        

            spawnEntitiesyQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEntitiesClient>());      
        }

        protected unsafe override void OnDestroy()
        {
            if (snapShotQuery.CalculateEntityCount() > 0)
            {
                var entity = snapShotQuery.GetSingletonEntity();
                var buffer = EntityManager.GetBuffer<SnapshotTick>(entity);

                if (buffer.Length == 0)
                    return;

                var array = buffer.AsNativeArray();
                for (int i = 0; i < array.Length; ++i)
                {
                    UnsafeUtility.Free(array[i].data, Allocator.Persistent);
                }
                buffer.Clear();
                FSLog.Debug("Snapshot destroy!");
            }
        }

        protected override void OnUpdate()
        {                       
            if (snapShotQuery.CalculateEntityCount() == 0)
                return;

            var buffer = EntityManager.GetBuffer<SnapshotTick>(snapShotQuery.GetSingletonEntity());

            FSLog.Info($"snapshot buffer length:{buffer.Length}");
            if (buffer.Length == 0)
                return;

            var snapshot = buffer[0];

            //remove first buffer snapshotTick
            buffer.RemoveAt(0);

            var stream = new UnmanagedMemoryStream((byte*)snapshot.data, snapshot.length);
            var reader = new BinaryReader(stream);

            snapshot.tick = reader.ReadUInt32();

            Dictionary<int, int> snapshotEntites = new Dictionary<int, int>();

            var spawnEntitiesBuffer = EntityManager.GetBuffer<EntityBuffer>(spawnEntitiesyQuery.GetSingletonEntity());

            //玩家
            var playerCount = reader.ReadInt32();          
            for (int i = 0; i < playerCount; ++i)
            {
                var playerId = reader.ReadInt32();
           
                float3 pos;
                pos.x = reader.ReadSingle();
                pos.y = reader.ReadSingle();
                pos.z = reader.ReadSingle();

                var health = reader.ReadInt32();
                var score = reader.ReadInt32();
                var maxScore = reader.ReadInt32();

                snapshotEntites.Add(playerId, 0);
                if (!entities.ContainsKey(playerId))
                {      
                    spawnEntitiesBuffer.Add(new EntityBuffer() { id = playerId, pos = pos ,type = EntityType.Player});
                }
                else
                {
                    var enity = entities[playerId];

                    var translation = EntityManager.GetComponentData<Translation>(enity);
                    translation.Value = pos;
                    EntityManager.SetComponentData(enity, translation);

                    var healthC = EntityManager.GetComponentData<Health>(enity);
                    healthC.Value = health;
                    EntityManager.SetComponentData(enity, healthC);

                    var scoreC = EntityManager.GetComponentData<Score>(enity);
                    scoreC.ScoreValue = score;
                    scoreC.MaxScoreValue = maxScore;
                    EntityManager.SetComponentData(enity, scoreC);
                }
            }          

            //敌人
            var enemyCount = reader.ReadInt32();         
            for (int i = 0; i < enemyCount; ++i)
            {
                //decode
                var enemyId = reader.ReadInt32(); 
                var enemyType = (EnemyType)reader.ReadByte();
                float3 pos;
                pos.x = reader.ReadSingle();
                pos.y = reader.ReadSingle();
                pos.z = reader.ReadSingle();

                var health = reader.ReadInt32();
                var attack = reader.ReadInt32();

                float fireCooldown = default;
                float rocketTimer = default;
                if (enemyType == EnemyType.Super)
                {
                    fireCooldown = reader.ReadSingle();
                    rocketTimer = reader.ReadSingle();
                }

                snapshotEntites.Add(enemyId, 0);
                if (!entities.ContainsKey(enemyId))
                {
                    //  FSLog.Info($" spwan enemy ({enemyId});"); 
                    spawnEntitiesBuffer.Add(new EntityBuffer() {
                        id = enemyId,
                        type = enemyType == EnemyType.Normal?EntityType.Enemy1:EntityType.Enemy2,
                        pos = pos
                    });
                }
                else
                {         

                    var enity = entities[enemyId];
                    var translation = EntityManager.GetComponentData<Translation>(enity);
                    translation.Value = pos;
                    EntityManager.SetComponentData(enity, translation);

                    var healthC = EntityManager.GetComponentData<Health>(enity);
                    healthC.Value = health;
                    EntityManager.SetComponentData(enity, healthC);

                    var attackC = EntityManager.GetComponentData<Attack>(enity);
                    attackC.Power = attack;
                    EntityManager.SetComponentData(enity, attackC);

                    if (enemyType == EnemyType.Super)
                    {
                        var fireRocketC = EntityManager.GetComponentData<FireRocket>(enity);
                        fireRocketC.FireCooldown = fireCooldown;
                        fireRocketC.RocketTimer = rocketTimer;
                        EntityManager.SetComponentData(enity, fireRocketC);

                    }
                }
            }

            //rocket
            var rocketCount = reader.ReadInt32();      
            for (int i = 0; i < rocketCount; ++i)
            {
                //decode
                var rocketId = reader.ReadInt32();
                var rocketType = (RocketType)reader.ReadByte();
                float3 pos;
                pos.x = reader.ReadSingle();
                pos.y = reader.ReadSingle();
                pos.z = reader.ReadSingle();

                quaternion rotation = new quaternion(
                     reader.ReadSingle(),
                     reader.ReadSingle(),
                     reader.ReadSingle(),
                     reader.ReadSingle());


                var health = reader.ReadInt32();
                var attack = reader.ReadInt32();
                float speed = reader.ReadSingle();

                Direction dir;
                if(rocketType == RocketType.Player)
                {
                    dir = (Direction)reader.ReadByte();
                }
               

                snapshotEntites.Add(rocketId, 0);
                if (!entities.ContainsKey(rocketId))
                {
                 //   FSLog.Info($"rocketType:{rocketType}");
                    spawnEntitiesBuffer.Add(new EntityBuffer()
                    {
                        id = rocketId,
                        type = rocketType == RocketType.Player ? EntityType.RocketPlayer : EntityType.RocketEnemy,
                        pos = pos,
                        rotation = rotation
                    });
                }
                else
                {
                    //if(rocketType == RocketType.Player)
                    //{
                    //    FSLog.Info($"update rocekt pos:({pos.x},{pos.y},{pos.z})");
                    //}
                    var enity = entities[rocketId];
                    var translation = EntityManager.GetComponentData<Translation>(enity);
                    translation.Value = pos;
                    EntityManager.SetComponentData(enity, translation);

                    var rotationC = EntityManager.GetComponentData<Rotation>(enity);
                    rotationC.Value = rotation;
                    EntityManager.SetComponentData(enity, rotationC);

                    var healthC = EntityManager.GetComponentData<Health>(enity);
                    healthC.Value = health;
                    EntityManager.SetComponentData(enity, healthC);

                    var attackC = EntityManager.GetComponentData<Attack>(enity);
                    attackC.Power = attack;
                    EntityManager.SetComponentData(enity, attackC);
                 
                }
            }


            //remove entity
            List<int> removed = new List<int>();
            foreach (var key in entities.Keys)
            {
                if (!snapshotEntites.ContainsKey(key))
                {
                    var entity = entities[key];
                   
                    EntityManager.AddComponentData(entity, new Despawn() { Frame = 0 });
                    removed.Add(key);
                }
            }

            foreach(var key in removed)
            {
                entities.Remove(key);
            }
        }

        public void AddEntity(int id, Entity e)
        {
            if (entities.ContainsKey(id))
                return;
            entities[id] = e;
        }
    }
}
