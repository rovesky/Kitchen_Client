using Assets.Scripts.Components;
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

        private EntityQuery snapShotQuery;
        private EntityQuery spawnEnemyQuery;

        protected  override void OnCreate()
        {
            snapShotQuery = GetEntityQuery(ComponentType.ReadOnly<SnapshotTick>());

           // if (snapShotQuery.CalculateEntityCount() == 0)
          //  {
                EntityManager.CreateEntity(typeof(SnapshotTick));
                snapShotQuery.SetSingleton(new SnapshotTick()
                {
                    tick = 0,
                    length = 0,
                    data = (uint*)UnsafeUtility.Malloc(4 * 1024, UnsafeUtility.AlignOf<UInt32>(), Allocator.Persistent)
                });

                FSLog.Debug("Snapshot create!");
            // }

            spawnEnemyQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnEnemyClient>());
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (snapShotQuery.CalculateEntityCount() > 0)
            {
                var snapshotTick = snapShotQuery.GetSingleton<SnapshotTick>();
                UnsafeUtility.Free(snapshotTick.data, Allocator.Persistent);
            }
        }

        protected override void OnUpdate()
        {

            //  FSLog.Info($" ReadSnapshotSystem OnUpdate!");
            //添加player到队列
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref Player player) =>
            {
                if (!entities.ContainsKey(player.id))
                {
                    entities[player.id] = entity;
                }
            });



            if (snapShotQuery.CalculateEntityCount() == 0)
                return;
        
            var snapshot = snapShotQuery.GetSingleton<SnapshotTick>();
            if (snapshot.length == 0)
                return;

            //   FSLog.Info($" begin reader:{snapshot.length}!");
            //var reader = new NetworkReader(snapshot.data, null);

            var stream = new UnmanagedMemoryStream((byte*)snapshot.data, snapshot.length);
            var reader = new BinaryReader(stream);

            snapshot.tick = reader.ReadUInt32();

            Dictionary<int, int> snapshotEntites = new Dictionary<int, int>();
            //玩家
            var playerCount = reader.ReadInt32();

            List<int> spawnPlayer = new List<int>();
            for (int i = 0; i < playerCount; ++i)
            {
                var playerId = reader.ReadInt32();

                snapshotEntites.Add(playerId, 0);
                //    var pos = reader.ReadVector3Q();
                float3 pos;
                pos.x = reader.ReadSingle();
                pos.y = reader.ReadSingle();
                pos.z = reader.ReadSingle();

                var health = reader.ReadInt32();
                var score = reader.ReadInt32();
                var maxScore = reader.ReadInt32();

                if (!entities.ContainsKey(playerId))
                {
                    FSLog.Info($" spwan.Add({playerId});");
                    spawnPlayer.Add(playerId);
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

            if (spawnPlayer.Count > 0)
            {
                Entities.ForEach((Entity entity, ref SpawnPlayer spawn) =>
                {
                    FSLog.Info($"SpawnPlayer begin");
                    if (!spawn.spawn)
                    {
                        EntityManager.AddBuffer<PlayerBuffer>(entity);
                        var buffer = EntityManager.GetBuffer<PlayerBuffer>(entity);

                        foreach (var id in spawnPlayer)
                        {
                            buffer.Add(new PlayerBuffer() { playerId = id });
                        }
                        FSLog.Info($"SpawnPlayer end");
                        spawn.spawn = true;
                    }
                });
            }

            //敌人
            var enemyCount = reader.ReadInt32();
            var spawnEnemyBuffer = EntityManager.GetBuffer<EnemyBuffer>(spawnEnemyQuery.GetSingletonEntity());

            for (int i = 0; i < enemyCount; ++i)
            {
                var enemyId = reader.ReadInt32();

                snapshotEntites.Add(enemyId, 0);
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

                if (!entities.ContainsKey(enemyId))
                {
                    //  FSLog.Info($" spwan enemy ({enemyId});"); 
                    spawnEnemyBuffer.Add(new EnemyBuffer() {
                        id = enemyId,
                        type = enemyType,
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


