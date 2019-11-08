﻿using System.Collections.Generic;
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

     //   private ReadSnapshotSystem readSnapshotSystem;
        private NetworkClientNewSystem networkClientNewSystem;
        private EntityQuery spawnEntitiesQuery;
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();


        public void ProcessEntitySpawn(int serverTime, int id, ushort typeId)
        {        
            Entity e = Entity.Null;
            if ((EntityType)typeId == EntityType.Enemy1 || (EntityType)typeId == EntityType.Enemy2)
            {
                var prefab = (EntityType)typeId == EntityType.Enemy1 ? enemy1Prefab : enemy2Prefab;
                var type = (EntityType)typeId == EntityType.Enemy1 ? EnemyType.Normal : EnemyType.Super;
                e = SpawnEntityUtil.SpwanEnemy(EntityManager, prefab, type,
                     Vector3.zero, rocketEnemy);
                EntityManager.AddComponentData(e, new Explosion());
                EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity
                });
                EntityManager.AddComponentData(e, new EntityPredictDataSnapshot()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity
                });
            }          
            else if ((EntityType)typeId == EntityType.Player)
            {             
                e = SpawnEntityUtil.SpwanPlayer(EntityManager, id, player,
                       Vector3.zero, rocketPlayer);

                EntityManager.AddComponentData(e, new LocalPlayer());
                EntityManager.AddComponentData(e, new UserCommand());
           //     EntityManager.AddComponentData(e, new Explosion());
               // EntityManager.AddComponentData(e, new UpdateUI());

                var rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, -180, 0);
                EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = rotation
                });

                EntityManager.AddComponentData(e, new EntityPredictDataSnapshot()
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

            var entity = entities[id];
            if (EntityManager.HasComponent<Player>(entity))
            {
                // FSLog.Info($"player Update!");
                var player = EntityManager.GetComponentData<Player>(entity);
                player.id = reader.ReadInt32();
                player.playerId = reader.ReadInt32();
              
                var predictData = EntityManager.GetComponentData<EntityPredictDataSnapshot>(entity);
                predictData.position = reader.ReadVector3Q();
                var rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, -180, 0);
                predictData.rotation = rotation;
                EntityManager.SetComponentData(entity, predictData);

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

                var predictData = EntityManager.GetComponentData<EntityPredictDataSnapshot>(entity);
                predictData.position = reader.ReadVector3Q();
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


     //       readSnapshotSystem = World.GetOrCreateSystem<ReadSnapshotSystem>();
            networkClientNewSystem = World.GetOrCreateSystem<NetworkClientNewSystem>();

            rocketEnemy = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Prefabs/EnemyRocket") as GameObject, World.Active);

            rocketPlayer = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Prefabs/PlayerRocket") as GameObject, World.Active);


            enemy1Prefab = Resources.Load("Prefabs/Enemy1") as GameObject;
            enemy2Prefab = Resources.Load("Prefabs/Enemy3") as GameObject;


            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player1") as GameObject, World.Active);

            FSLog.Info($" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
          

        }
    }
}
