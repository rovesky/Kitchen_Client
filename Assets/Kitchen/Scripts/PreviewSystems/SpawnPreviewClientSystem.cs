using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnPreviewClientSystem : ComponentSystem
    {
        private GameObject playerObj;

        private Entity player;

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, PlayerEntity = Entity.Null});

            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
               Resources.Load("Player2") as GameObject, 
               GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                   World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

            playerObj = Object.Instantiate(Resources.Load("CharacterRobot1") as GameObject);
            FSLog.Info(" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            if (localPlayer.PlayerEntity != Entity.Null)
                return;
           
         //   var e = playerObj.GetComponent<EntityTracker>().EntityToTrack;
            var e = EntityManager.Instantiate(player);

            var position = new Vector3 {x = 0, y = 1, z = -5};
            var rotation = Quaternion.identity;
          
            CreateEntityUtilities.CreateCharacterComponent(EntityManager, e, position, rotation);

            var presentationEntity = playerObj.GetComponentInChildren<GameObjectEntity>() == null
                ? Entity.Null
                : playerObj.GetComponentInChildren<GameObjectEntity>().Entity;

           // EntityManager.AddComponentData(presentationEntity, new Parent() {Value = e});
           // EntityManager.AddComponentData(presentationEntity, new LocalToParent());

            EntityManager.SetComponentData(e, new Character
            {
                PresentationEntity = presentationEntity
            });

            EntityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id = 0,
                PredictingPlayerId = -1
            });

            EntityManager.AddComponentData(e, new ServerEntity());
            EntityManager.AddComponentData(e, new UpdateUI());

            localPlayer.PlayerEntity = e;
            SetSingleton(localPlayer);
        }
    }
}