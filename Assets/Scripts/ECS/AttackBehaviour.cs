using Unity.Entities;
using UnityEngine;

public struct Attack : IComponentData
{   
    public int Power;


}

public class AttackBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Power;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Attack() { Power = Power });
    }
}

