using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    [UnityEngine.ExecuteAlways]
    public class ClientSimulationSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<InputSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<NetworkClientSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ReadSnapshotSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnPlayerClientSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<SpawnEnemyClientSystem>());

            //  m_systemsToUpdate.Add(World.GetOrCreateSystem<PlayerFireSystem>());
            //  m_systemsToUpdate.Add(World.GetOrCreateSystem<EnemyFireSystem>());

            //  m_systemsToUpdate.Add(World.GetOrCreateSystem<RayCastSystem>());
            //  m_systemsToUpdate.Add(World.GetOrCreateSystem<HealthSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ExlosionSystem>());
       //     m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateHealthUISystem>());



        }

        public override void SortSystemUpdateList()
        {
           
        }
    }
}
