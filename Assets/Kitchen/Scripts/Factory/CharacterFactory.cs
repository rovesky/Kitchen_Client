using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CharacterFactory : ReplicatedEntityFactory
    {
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
            //var playerPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            //     Resources.Load("Player1") as GameObject, World.Active);

            //var e = entityManager.Instantiate(playerPrefab);
            //entityManager.AddComponentData(e, new Character());

            var go = Instantiate(Resources.Load("Player3") as GameObject);
            var e = go.GetComponent<EntityTracker>().EntityToTrack;

            CreateEntityUtilities.CreateCharacterComponent(entityManager, e, Vector3.zero, Quaternion.identity);

            entityManager.SetComponentData(e, new Character
            {
                PresentationEntity = go.GetComponentInChildren<GameObjectEntity>().Entity
            });

            entityManager.AddComponentData(e, new UpdateUI());

            return e;
        }
    }
}