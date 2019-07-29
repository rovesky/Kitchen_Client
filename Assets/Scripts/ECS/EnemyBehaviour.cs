using Unity.Entities;
using UnityEngine;

public struct Enemy : IComponentData
{
    public bool a;
}

public class EnemyBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Enemy() { a = false });
    }
}

