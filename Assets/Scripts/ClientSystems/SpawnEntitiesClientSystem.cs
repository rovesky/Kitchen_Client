using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class SpawnEntitiesClientSystem : ComponentSystem, ISnapshotConsumer
    {
  
        private Entity player;
        private Entity platePrefab;
        private NetworkClientNewSystem networkClientNewSystem;
        private InterpolatedSystem interpolatedSystem;
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        public void ProcessEntitySpawn(int serverTime, int id, ushort typeId)
        {        
            Entity e = Entity.Null;         
            if ((EntityType)typeId == EntityType.Player)
            {             

                e = EntityManager.Instantiate(player);
                Translation position = new Translation() { Value = Vector3.zero };
                Rotation rotation = new Rotation() { Value = Quaternion.identity };

                EntityManager.SetComponentData(e, position);

                EntityManager.AddComponentData(e, new Player() { playerId = id, id = e.Index });
                EntityManager.AddComponentData(e, new Attack() { Power = 10000 });
                EntityManager.AddComponentData(e, new Damage());
                EntityManager.AddComponentData(e, new Health() { Value = 30 });
                EntityManager.AddComponentData(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
                EntityManager.AddComponentData(e, new UpdateUI());
				EntityManager.AddComponentData(e, new CharacterDataComponent() 
				{
					SkinWidth = 0.02f,
					Entity = e,
				});

				EntityManager.AddComponentData(e, new EntityPredictData()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity
                });

             
            }
            else if ((EntityType)typeId == EntityType.Plate)
            {
                FSLog.Info($"ProcessEntitySpawn Plate:{id}");
                e = EntityManager.Instantiate(platePrefab);
                EntityManager.AddComponentData(e, new Plate() { id = id });
                Translation position = new Translation() { Value = Vector3.zero};
                Rotation rotation = new Rotation() { Value = Quaternion.identity };

                EntityManager.SetComponentData(e, position);
                EntityManager.SetComponentData(e, rotation);

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

            var localPalyer = GetSingleton<LocalPlayer>();

            if (EntityManager.HasComponent<Player>(entity))
            {
                var player = EntityManager.GetComponentData<Player>(entity);
                player.id = reader.ReadInt32();
                player.id = id;
                player.playerId = reader.ReadInt32();
                EntityManager.SetComponentData(entity,player);

                var position = reader.ReadVector3Q();
                var rotation = reader.ReadQuaternionQ();

                // FSLog.Error($"player.playerId:{player.playerId},localPalyer.playerId:{localPalyer.playerId}");
                if (localPalyer.playerId == player.playerId)
                {
                    if (localPalyer.playerEntity == Entity.Null)
                    {
                        localPalyer.playerEntity = entity;
                        SetSingleton(localPalyer);
                    }
                    if (!EntityManager.HasComponent<UserCommand>(entity))
                        EntityManager.AddComponentData(entity, new UserCommand());

                    if (!EntityManager.HasComponent<MoveInput>(entity))
                        EntityManager.AddComponentData(entity, new MoveInput()
                        {
                            Speed = 6,
                        });

                    if (!EntityManager.HasComponent<EntityPredictDataSnapshot>(entity))
                    { 
                        EntityManager.AddComponentData(entity, new EntityPredictDataSnapshot()
                        {
                            position = Vector3.zero,
                            rotation = Quaternion.identity
                        });
                    }

                    if (!EntityManager.HasComponent<PickupItem>(entity))
                    {
                        EntityManager.AddComponentData(entity, new PickupItem()
                        {
                            pickupEntity = Entity.Null
                        });
                    }

                    var predictData = EntityManager.GetComponentData<EntityPredictDataSnapshot>(entity);
                    predictData.position = position;         
                    predictData.rotation = rotation;
                    EntityManager.SetComponentData(entity, predictData);
                }
                else
                {
                    if (!EntityManager.HasComponent<EntityInterpolate>(entity))
                    {                   
                        EntityManager.AddComponentData(entity, new EntityInterpolate());
                    }                                

              
                    var interpolateData = new EntityPredictData()
                    {
                        position = position,
                        rotation = rotation
                    };
                    interpolatedSystem.AddData(serverTime,id,ref interpolateData);
                }

                //   FSLog.Info($"player Update:{player.id},{id},[{predictData.position.x},{predictData.position.y},{predictData.position.z}]");
                var health = EntityManager.GetComponentData<Health>(entity);
                health.Value = reader.ReadInt32();
                EntityManager.SetComponentData(entity, health);

                var score = EntityManager.GetComponentData<Score>(entity);
                score.ScoreValue = reader.ReadInt32();
                score.MaxScoreValue = reader.ReadInt32();
                EntityManager.SetComponentData(entity, score);
            }        
            else if (EntityManager.HasComponent<Plate>(entity))
            {
           
                var id1 = reader.ReadInt32();

                var position = reader.ReadVector3Q();
                var rotation = reader.ReadQuaternionQ();

                if(EntityManager.GetComponentData<Translation>(entity).Value.Equals((float3)Vector3.zero))
                {
                    EntityManager.SetComponentData(entity, new Translation() { Value = position });

                }
                if (EntityManager.GetComponentData<Rotation>(entity).Value.Equals(Quaternion.identity))
                {
                    EntityManager.SetComponentData(entity, new Rotation() { Value = rotation });

                }

                //if (!EntityManager.HasComponent<EntityInterpolate>(entity))
                //{
                //    EntityManager.AddComponentData(entity, new EntityInterpolate());
                //}

                //var interpolateData = new EntityPredictData()
                //{
                //    position = position,
                //    rotation = rotation
                //};
                //interpolatedSystem.AddData(serverTime, id, ref interpolateData);
            }
        }

        protected override void OnCreate()
        {
            var entity = EntityManager.CreateEntity(typeof(SpawnEntitiesClient));
            SetSingleton(new SpawnEntitiesClient());
            EntityManager.AddBuffer<SpawnEntityBuffer>(entity);

            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer(){ playerId = -1, playerEntity = Entity.Null });

            networkClientNewSystem = World.GetOrCreateSystem<NetworkClientNewSystem>();
            interpolatedSystem = World.GetOrCreateSystem<InterpolatedSystem>();

            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player1") as GameObject, World.Active);

            platePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                 Resources.Load("Plate") as GameObject, World.Active);

            FSLog.Info($" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
          

        }
    }
}
