using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnPreviewClientSystem : ComponentSystem
    {
        private Entity player;

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, PlayerEntity = Entity.Null});

            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player2") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));
        }

        protected override void OnUpdate()
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            if (localPlayer.PlayerEntity != Entity.Null)
                return;

            localPlayer.PlayerEntity = CreateCharacter(new float3 {x = 0, y = 1, z = -5}, true);
            SetSingleton(localPlayer);

            var e = CreateCharacter(new float3 {x = -3, y = 3, z = -5}, false);

            EntityManager.SetComponentData(e,new VelocityPredictedState()
            {
                Linear = new float3(6,0,0)
            });
        }

        private Entity CreateCharacter(float3 position, bool isLocal)
        {
            var e = EntityManager.Instantiate(player);
            var playerObj = Object.Instantiate(Resources.Load("CharacterRobot1") as GameObject);
            FSLog.Info(" spwan entity OnCreate2");

            var rotation = Quaternion.identity;

            CreateCharacterUtilities.CreateCharacterComponent(EntityManager, e, position, rotation);

            var presentationEntity = playerObj.GetComponentInChildren<GameObjectEntity>() == null
                ? Entity.Null
                : playerObj.GetComponentInChildren<GameObjectEntity>().Entity;

            EntityManager.SetComponentData(e, new Character
            {
                PresentationEntity = presentationEntity
            });
            
            EntityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id = isLocal ? 0 : 1,
                PredictingPlayerId = isLocal ? 0 : 1
            });

            if (!isLocal)
                return e;


            EntityManager.AddComponentData(e, new ServerEntity());
            EntityManager.AddComponentData(e, new UpdateUI());

            return e;
        }
    }
}