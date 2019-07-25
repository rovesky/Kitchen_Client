using Unity.Entities;
using UnityEngine;

public struct CollisionDestroy : IComponentData
{     
    public bool IsCollision;
}

public class CollisionDestroyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CollisionDestroy() { IsCollision = false});
    }
}

