using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct Player : IComponentData {  }

public class PlayerBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Player());
    }
}

