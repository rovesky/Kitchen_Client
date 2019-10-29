using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public unsafe class ReadSnapshotSystem : ComponentSystem
    {
        private Dictionary<int,Entity> entities = new Dictionary<int, Entity>();
        private EntityQuery spawnEntitiesyQuery;

        protected override void OnCreate()
        {

            var snapshotEntity = EntityManager.CreateEntity(typeof(SnapshotFromServer));
            SetSingleton(new SnapshotFromServer()
            {
                tick = 0,
                length = 0,
                time = 0,
                rtt = 0,
                lastAcknowlegdedCommandTime = 0,
                data = (uint*)UnsafeUtility.Malloc(4 * 1024, UnsafeUtility.AlignOf<UInt32>(), Allocator.Persistent)
            });

          //  EntityManager.AddBuffer<SnapshotTick>(snapshotEntity);
            
            FSLog.Debug("Snapshot create!");         

            spawnEntitiesyQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEntitiesClient>());
        }

        protected unsafe override void OnDestroy()
        {
            var snapshot = GetSingleton<SnapshotFromServer>();
            UnsafeUtility.Free(snapshot.data, Allocator.Persistent);
            FSLog.Debug("Snapshot destroy!");
        }

        private void ClearBuffer(ref DynamicBuffer<SnapshotTick> buffer)
        {
            var array = buffer.ToNativeArray(Allocator.Temp);
            buffer.Clear();
            for (int i = 0; i < array.Length; ++i)
            {
                UnsafeUtility.Free(array[i].data, Allocator.Persistent);
            }          
            array.Dispose();            
        }

        protected override void OnUpdate()
        {                        
     
            var snapshotFromServer = GetSingleton<SnapshotFromServer>();
            if (snapshotFromServer.length == 0)
                return;

            var stream = new UnmanagedMemoryStream((byte*)snapshotFromServer.data, snapshotFromServer.length);
            var reader = new BinaryReader(stream);

            snapshotFromServer.tick = reader.ReadUInt32();      
          //  FSLog.Info($"recv snapshot.tick:{ snapshotFromServer.tick}");
            Dictionary<int, int> snapshotEntites = new Dictionary<int, int>();

            var spawnEntitiesBuffer = EntityManager.GetBuffer<SpawnEntityBuffer>(spawnEntitiesyQuery.GetSingletonEntity());

            //玩家
            var playerCount = reader.ReadInt32();          
            for (int i = 0; i < playerCount; ++i)
            {
                var playerId = reader.ReadInt32();
                var sessionId = reader.ReadInt32();

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
                    spawnEntitiesBuffer.Add(new SpawnEntityBuffer() { id = playerId, pos = pos, type = EntityType.Player });
                }
                else
                {
                    var enity = entities[playerId];

                    //var predictData = EntityManager.GetComponentData<EntityPredictData>(enity);
                    //predictData.position = pos;
                    //EntityManager.SetComponentData(enity, predictData);   

                    snapshotFromServer.predictData.position = pos;
                    var rotation = Quaternion.identity;
                    rotation.eulerAngles = new Vector3(0, -180, 0);
                    snapshotFromServer.predictData.rotation = rotation;

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
                    spawnEntitiesBuffer.Add(new SpawnEntityBuffer() {
                        id = enemyId,
                        type = enemyType == EnemyType.Normal?EntityType.Enemy1:EntityType.Enemy2,
                        pos = pos
                    });
                }
                else
                {         

                    var enity = entities[enemyId];
                    var predictData = EntityManager.GetComponentData<EntityPredictData>(enity);
                    predictData.position = pos;
                    EntityManager.SetComponentData(enity, predictData);

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

                //Direction dir;
                //if(rocketType == RocketType.Player)
                //{
                //    dir = (Direction)reader.ReadByte();
                //}
               

                snapshotEntites.Add(rocketId, 0);
                if (!entities.ContainsKey(rocketId))
                {
                 //   FSLog.Info($"rocketType:{rocketType}");
                    spawnEntitiesBuffer.Add(new SpawnEntityBuffer()
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

                    var predictData = EntityManager.GetComponentData<EntityPredictData>(enity);
                    predictData.position = pos;
                    predictData.rotation = rotation;
                    EntityManager.SetComponentData(enity, predictData);

                    //var translation = EntityManager.GetComponentData<Translation>(enity);
                    //translation.Value = pos;
                    //EntityManager.SetComponentData(enity, translation);

                    //var rotationC = EntityManager.GetComponentData<Rotation>(enity);
                    //rotationC.Value = rotation;
                    //EntityManager.SetComponentData(enity, rotationC);

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

            SetSingleton(snapshotFromServer);

           // UnsafeUtility.Free(snapshot.data, Allocator.Persistent);
        
            //for (int i = 0; i < array.Length; ++i)
            //{
            //    
            //}          
            //array.Dispose();

        }

        public void AddEntity(int id, Entity e)
        {
            if (entities.ContainsKey(id))
                return;
            entities[id] = e;
        }
    }
}
