using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class LatePresentationSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateTimeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateRTTSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateItemUISystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateMenuSystem>());
        }
    }

}
