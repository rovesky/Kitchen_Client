using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public struct MoveTranslation : IComponentData
    {
        public int Speed;
        public Direction Direction;    }


    public class MoveTranslationBehaviour : MonoBehaviour,  IConvertGameObjectToEntity
    {
        public int Speed;
        public Direction Direction;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(
                entity,
                new MoveTranslation()
                {
                    Speed = Speed,
                    Direction = Direction,
                });
        }
    }
         
}