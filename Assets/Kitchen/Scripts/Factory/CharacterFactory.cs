//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
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

            var go = Object.Instantiate(Resources.Load("Player3") as GameObject);
            var e = go.GetComponent<EntityTracker>().EntityToTrack;

            entityManager.AddComponentData(e, new Character()
            {
                PresentationEntity = go.GetComponentInChildren<GameObjectEntity>().Entity
            });
          

            entityManager.AddComponentData(e, new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.AddComponentData(e, new CharacterPredictedState()
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                PickupedEntity = Entity.Null
            });

            entityManager.AddComponentData(e, new CharacterInterpolatedState()
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
            });

            entityManager.AddComponentData(e, new UserCommand());
            entityManager.AddComponentData(e, new UpdateUI());

            entityManager.AddComponentData(e, new CharacterMove()
            {
                SkinWidth = 0.02f,
                Velocity = 7.0f
            });
            entityManager.AddComponentData(e, new CharacterPickupItem());

            entityManager.AddComponentData(e, new CharacterThrowItem()
            {
                Velocity = 10.0f
            });

            return e;
        }
    }
}
