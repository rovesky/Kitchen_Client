using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

// An example character controller that uses a rigid body to move
// See CharacterControllerComponent for a 'proxy' based character controller also
namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct MoveMouse : IComponentData
    {
        public float MovementSpeed;
        public LayerMask InputMask; // 鼠标射线碰撞层
    }

    public class MoveMouseBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float MovementSpeed = 5;
        public LayerMask InputMask; // 鼠标射线碰撞层

        void OnEnable() { }

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (enabled)
            {
                var componentData = new MoveMouse
                {
                    MovementSpeed = MovementSpeed,
                    InputMask = InputMask
                };

                dstManager.AddComponentData(entity, componentData);
            }
        }
    } 

}