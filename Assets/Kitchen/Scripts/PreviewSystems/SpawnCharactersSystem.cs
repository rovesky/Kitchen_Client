using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnCharactersSystem : ComponentSystem
    {
        private Entity playerPrefab;

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, PlayerEntity = Entity.Null});

            playerPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Character/Player2") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));
        }

        protected override void OnUpdate()
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            if (localPlayer.PlayerEntity != Entity.Null)
                return;

            localPlayer.PlayerEntity = CreateCharacter(new float3 {x = 0, y = 1, z = -4}, true,0);
            SetSingleton(localPlayer);
          
            //var e = CreateCharacter(new float3 {x = -3, y = 1, z = -4}, false,1);
            //var interpolatedState = EntityManager.GetComponentData<CharacterInterpolatedState>(e);
            //interpolatedState.MaterialId = 1;
            //EntityManager.SetComponentData(e, interpolatedState);
        }

        private Entity CreateCharacter(float3 position, bool isLocal,int id)
        {
            var e = EntityManager.Instantiate(playerPrefab);
            var playerObj = Object.Instantiate(Resources.Load("Character/CharacterRobot1") as GameObject);
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
                Id =  id,
                PredictingPlayerId = isLocal ? 0 : 1
            });

            if (!isLocal)
                return e;


            EntityManager.AddComponentData(e, new ServerEntity());
            EntityManager.AddComponentData(e, new UpdateUI());
            EntityManager.AddComponentData(e, new Connection());

            return e;
        }
    }
}