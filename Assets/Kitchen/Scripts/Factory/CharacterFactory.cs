using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CharacterFactory : ReplicatedEntityFactory
    {
        private readonly GameObject playerObj;

        public CharacterFactory()
        {
             playerObj = Instantiate(Resources.Load("Player3") as GameObject);
        }
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
            //var playerPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            //     Resources.Load("Player1") as GameObject, World.Active);

            //var e = entityManager.Instantiate(playerPrefab);
            //entityManager.AddComponentData(e, new Character());
         
            var e = playerObj.GetComponent<EntityTracker>().EntityToTrack;

            CreateEntityUtilities.CreateCharacterComponent(entityManager, e, Vector3.zero, Quaternion.identity);

            entityManager.SetComponentData(e, new Character
            {
                PresentationEntity = playerObj.GetComponentInChildren<GameObjectEntity>().Entity
            });

            entityManager.AddComponentData(e, new UpdateUI());

            return e;
        }
    }
}