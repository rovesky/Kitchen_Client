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

        protected  override void OnCreate()
        {
            snapShotQuery = GetEntityQuery(ComponentType.ReadOnly<SnapshotTick>());

            if (snapShotQuery.CalculateEntityCount() == 0)
            {
                var entity = EntityManager.CreateEntity(typeof(SnapshotTick));
                snapShotQuery.SetSingleton(new SnapshotTick()
                {
                    tick = 0,
                    length = 0,
                    data = (uint*)UnsafeUtility.Malloc(4 * 1024, UnsafeUtility.AlignOf<UInt32>(), Allocator.Persistent)
                });

                FSLog.Debug("Snapshot create1!");
            }
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

        protected  override void OnUpdate()
        {

          //  FSLog.Info($" ReadSnapshotSystem OnUpdate!");
            //添加player到队列
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity,ref Player player) =>
            {
                if (!entities.ContainsKey(player.id))
                {
                    entities[player.id] = entity;
                }
            });
         
            if (snapShotQuery.CalculateEntityCount() == 0 )
                return;
          //  FSLog.Info($" ReadSnapshotSystem OnUpdate2!");

            var snapshot = snapShotQuery.GetSingleton<SnapshotTick>();
            if (snapshot.length == 0)
                return;

            FSLog.Info($" begin reader:{snapshot.length}!");
            //var reader = new NetworkReader(snapshot.data, null);

            var stream = new UnmanagedMemoryStream((byte*)snapshot.data, snapshot.length);
            var reader = new BinaryReader(stream);

            snapshot.tick = reader.ReadUInt32();
            var playerCount = reader.ReadInt32();

            List<int> spawnPlayer = new List<int>();
            for (int i = 0; i < playerCount; ++i)
            {
                var playerId = reader.ReadInt32();
                //    var pos = reader.ReadVector3Q();
                float3 pos;
                pos.x= reader.ReadSingle();
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
                    if (!spawn.spawned)
                    {
                        EntityManager.AddBuffer<PlayerId>(entity);
                        var buffer = EntityManager.GetBuffer<PlayerId>(entity);

                        foreach (var id in spawnPlayer)
                        {
                            buffer.Add(new PlayerId() { playerId = id });
                        }
                        FSLog.Info($"SpawnPlayer end");
                        spawn.spawned = true;
                    }
                });
            }
        }
    }
}


