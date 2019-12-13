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
             playerObj.transform.position = new Vector3(0, -10, 9);
        }
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
            //var playerPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            //     Resources.Load("Player1") as GameObject, World.Active);

            //var e = entityManager.Instantiate(playerPrefab);
            //entityManager.AddComponentData(e, new Character());
           // playerObj.SetActive(true);
            var e = playerObj.GetComponent<EntityTracker>().EntityToTrack;

            var pos = new Vector3(0, -10, 9);
            CreateEntityUtilities.CreateCharacterComponent(entityManager, e, pos, Quaternion.identity);

            entityManager.SetComponentData(e, new Character
            {
                PresentationEntity = playerObj.GetComponentInChildren<GameObjectEntity>().Entity
            });

            entityManager.AddComponentData(e, new UpdateUI());

            return e;
        }
    }
}