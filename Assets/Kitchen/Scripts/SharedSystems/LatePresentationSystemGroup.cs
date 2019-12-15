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
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateUISystem>());
        }
    }

}
