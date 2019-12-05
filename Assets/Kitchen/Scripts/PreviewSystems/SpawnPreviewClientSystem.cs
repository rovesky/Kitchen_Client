using FootStone.ECS;
using Unity.Entities;
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
                Resources.Load("Player2") as GameObject, World.Active);

            FSLog.Info(" spwan entity OnCreate2");
        }

        protected override void OnUpdate()
        {
            var localPalyer = GetSingleton<LocalPlayer>();
            if (localPalyer.PlayerEntity != Entity.Null)
                return;

            var go = Object.Instantiate(Resources.Load("Player3") as GameObject);
            var e = go.GetComponent<EntityTracker>().EntityToTrack;
            //  var e = EntityManager.Instantiate(player);

            var position = new Vector3 {x = 0, y = 1, z = -5};
            var rotation = Quaternion.identity;
          
            CreateEntityUtilities.CreateCharacterComponent(EntityManager, e, position, rotation);

            EntityManager.SetComponentData(e, new Character
            {
                PresentationEntity = go.GetComponentInChildren<GameObjectEntity>().Entity
            });

            EntityManager.AddComponentData(e, new ServerEntity());
            EntityManager.AddComponentData(e, new UpdateUI());

            localPalyer.PlayerEntity = e;
            SetSingleton(localPalyer);
        }
    }
}