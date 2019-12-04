using System;
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
    public class PlateFactory : ReplicatedEntityFactory
    {
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
            var platePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Plate") as GameObject, World.Active);

            var e = entityManager.Instantiate(platePrefab);

            entityManager.AddComponentData(e, new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.AddComponentData(e, new Plate());
            entityManager.AddComponentData(e, new ItemInterpolatedState()
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            entityManager.AddComponentData(e, new ItemPredictedState()
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            return e;
        }
    }
}
