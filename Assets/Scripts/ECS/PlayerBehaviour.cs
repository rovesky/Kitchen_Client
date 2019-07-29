using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{
    public bool a;
}

public class PlayerBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Player() { a = false });
    }
}

