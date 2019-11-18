using System.Collections.Generic;
using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public enum EntityType
    {
        Player,
        Plate
    }

    [DisableAutoCreation]
	public class SpawnEntitiesClientSystem : ComponentSystem, ISnapshotConsumer
	{

		private Entity player;
		private Entity platePrefab;
		private NetworkClientSystem networkClientNewSystem;
		private InterpolatedSystem interpolatedSystem;
		private ItemInterpolatedSystem<ItemInterpolatedState> itemInterpolatedSystem;
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
			//	EntityManager.AddComponentData(e, new Attack() { Power = 10000 });
			//	EntityManager.AddComponentData(e, new Damage());
			//	EntityManager.AddComponentData(e, new Health() { Value = 30 });
			//	EntityManager.AddComponentData(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
				EntityManager.AddComponentData(e, new UpdateUI());
				EntityManager.AddComponentData(e, new CharacterDataComponent()
				{
					SkinWidth = 0.02f,
					Entity = e,
				});

                //EntityManager.AddComponentData(e, new CharacterPredictState()
                //{
                //	position = Vector3.zero,
                //	rotation = Quaternion.identity,
                //                pickupEntity = Entity.Null
                //            });

                EntityManager.AddComponentData(e, new CharacterInterpolateState()
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity,
                });
            }
			else if ((EntityType)typeId == EntityType.Plate)
			{
				FSLog.Info($"ProcessEntitySpawn Plate:{id}");
				e = EntityManager.Instantiate(platePrefab);
				EntityManager.AddComponentData(e, new Plate() { id = id });			

				EntityManager.AddComponentData(e, new ItemInterpolatedState()
				{
					position = Vector3.zero,
					rotation = Quaternion.identity,
					owner = Entity.Null

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

			var localPalyer = GetSingleton<LocalPlayer>();

			if (EntityManager.HasComponent<Player>(entity))
			{
				var player = EntityManager.GetComponentData<Player>(entity);
				player.id = reader.ReadInt32();
				player.id = id;
				player.playerId = reader.ReadInt32();
				EntityManager.SetComponentData(entity, player);

				var position = reader.ReadVector3Q();
				var rotation = reader.ReadQuaternionQ();

				var pickEntityId = reader.ReadInt32();

				var pickEntity = Entity.Null;
				if (pickEntityId != -1 && entities.ContainsKey(pickEntityId))
				{
					pickEntity = entities[pickEntityId];
				}
				//    FSLog.Info($"pickupEntity:{pickEntityId},{pickEntity.Index}");
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

					if (!EntityManager.HasComponent<PickupItem>(entity))
					{
						EntityManager.AddComponentData(entity, new PickupItem());
					}

					if (!EntityManager.HasComponent<ThrowItem>(entity))
					{
						EntityManager.AddComponentData(entity, new ThrowItem()
						{
							speed = 0
						});
					}

                    if (!EntityManager.HasComponent<CharacterPredictState>(entity))
                    {
                        EntityManager.AddComponentData(entity, new CharacterPredictState()
                        {
                            position = Vector3.zero,
                            rotation = Quaternion.identity,
                            pickupEntity = Entity.Null
                        });
                    }

                    if (!EntityManager.HasComponent<EntityPredictDataSnapshot>(entity))
                    {
                        EntityManager.AddComponentData(entity, new EntityPredictDataSnapshot()
                        {
                            position = Vector3.zero,
                            rotation = Quaternion.identity,
                            pickupEntity = Entity.Null
                        });
                    }

                    var predictData = EntityManager.GetComponentData<EntityPredictDataSnapshot>(entity);
					predictData.position = position;
					predictData.rotation = rotation;
					predictData.pickupEntity = pickEntity;
					EntityManager.SetComponentData(entity, predictData);
				}
				else
				{
					if (!EntityManager.HasComponent<EntityInterpolate>(entity))
					{
						EntityManager.AddComponentData(entity, new EntityInterpolate()
						{
							id = id
						});
					}

					var interpolateData = new CharacterInterpolateState()
					{
						position = position,
						rotation = rotation
					};
					interpolatedSystem.AddData(serverTime, id, ref interpolateData);
				}

			}
			else if (EntityManager.HasComponent<Plate>(entity))
			{
				var id1 = reader.ReadInt32();

				var position = reader.ReadVector3Q();
				var rotation = reader.ReadQuaternionQ();

				var ownerEnityID = reader.ReadInt32();

				var ownerEntity = Entity.Null;
				if (ownerEnityID != -1 && entities.ContainsKey(ownerEnityID))
				{
					ownerEntity = entities[ownerEnityID];
				}

				if (!EntityManager.HasComponent<EntityInterpolate>(entity))
				{
					EntityManager.AddComponentData(entity, new EntityInterpolate()
					{
						id = id
					});
				}
				var state = new ItemInterpolatedState();
				state.position = position;
				state.rotation = rotation;
				state.owner = ownerEntity;
				itemInterpolatedSystem.AddData(serverTime, id, ref state);

			}
		}

		protected override void OnCreate()
		{		

			EntityManager.CreateEntity(typeof(LocalPlayer));
			SetSingleton(new LocalPlayer() { playerId = -1, playerEntity = Entity.Null });

			networkClientNewSystem = World.GetOrCreateSystem<NetworkClientSystem>();

			interpolatedSystem = World.GetOrCreateSystem<InterpolatedSystem>();
			itemInterpolatedSystem = World.GetOrCreateSystem<ItemInterpolatedSystem<ItemInterpolatedState>>();


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
