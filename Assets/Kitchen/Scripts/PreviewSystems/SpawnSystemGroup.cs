using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class SpawnSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnGameSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnMenuSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnFoodsSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnPlatesSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnCharactersSystem>());
     
        }
    }

}
