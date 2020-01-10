using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CharacterFactory : ReplicatedEntityFactory
    {
      //  private  GameObject playerObj;
        private readonly Entity player;

        public CharacterFactory()
        {
          //  playerObj = Instantiate(Resources.Load("Player3") as GameObject);
          //  playerObj.transform.position = new Vector3(0, -10, 9);

            player = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player2") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));
        }
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {

            var e = entityManager.Instantiate(player);
            var playerObj = Instantiate(Resources.Load("CharacterRobot1") as GameObject);

            FSLog.Info($" spawn character:{e}");
            var pos = new Vector3(0, -10, 9);
            CreateCharacterUtilities.CreateCharacterComponent(entityManager, e, pos, Quaternion.identity);

            var presentationEntity = playerObj.GetComponentInChildren<GameObjectEntity>() == null
                ? Entity.Null
                : playerObj.GetComponentInChildren<GameObjectEntity>().Entity;

            entityManager.SetComponentData(e, new Character
            {
                PresentationEntity = presentationEntity
            });

            entityManager.AddComponentData(e, new UpdateUI());

            return e;
        }
    }
}