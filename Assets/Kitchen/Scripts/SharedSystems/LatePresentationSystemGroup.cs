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
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyBoxAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPotAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyGasCookerAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyExtinguisherAnimSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCatchFireAnimSystem>());
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateGameStateSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateTimeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateScoreSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateRTTSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateProgressSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateFoodIconSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdatePlateIconSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdatePotIconSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateMenuItemSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdatePotInfoSystem>());
           // m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateButton1StateSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateButton2StateSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateButton3StateSystem>());
          
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<RemoveNewServerEntitySystem>());
        }
    }
}
