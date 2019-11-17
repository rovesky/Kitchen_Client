using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{  
    [DisableAutoCreation]
    public class SpawnClientSystemGroup : NoSortComponentSystemGroup
    {
     
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnEntitiesClientSystem>());
        }
    } 
    
}
