﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class PresentationSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {            

            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyItemPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ClientTriggerProcessSystem>());      
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateUISystem>());
        }
    }

}