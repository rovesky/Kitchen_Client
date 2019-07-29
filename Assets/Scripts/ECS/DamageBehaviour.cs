using Unity.Entities;
using UnityEngine;

public struct Damage : IComponentData
{   
    public int damage;
}

public class DamageBehaviour : MonoBehaviour, IConvertGameObjectToEntity { 


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Damage(){ damage = 0 });
    }
}

