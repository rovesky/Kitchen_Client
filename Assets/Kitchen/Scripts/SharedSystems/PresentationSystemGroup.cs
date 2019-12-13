using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class PresentationSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ClearTriggerColorSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateItemPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyItemPresentationSystem>());

           // m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateUISystem>());
        }
    }

}
