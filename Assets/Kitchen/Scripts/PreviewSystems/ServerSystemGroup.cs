using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class ServerSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupBoxSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<FoodSlicedSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CountdownSystem>());
         //   m_systemsToUpdate.Add(World.GetOrCreateSystem<MenuSystem>());
        }
    }

}
