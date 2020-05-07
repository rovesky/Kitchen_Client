﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnCharactersSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, PlayerEntity = Entity.Null});
        }

        protected override void OnUpdate()
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            if (localPlayer.PlayerEntity != Entity.Null)
                return;

            localPlayer.PlayerEntity = CreateCharacter(new float3 {x = 0, y = 1, z = -4}, true,0);
            SetSingleton(localPlayer);
          
            //var e = CreateCharacter(new float3 {x = -3, y = 1, z = -4}, false,1);
            //var interpolatedState = EntityManager.GetComponentData<CharacterInterpolatedState>(e);
            //interpolatedState.MaterialId = 1;
            //EntityManager.SetComponentData(e, interpolatedState);
        }

        private Entity CreateCharacter(float3 position, bool isLocal,int id)
        {
            var e = ClientCharacterUtilities.
                CreateCharacter(EntityManager,new float3 {x = 0, y = 1, z = -4});


            EntityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id =  id,
                PredictingPlayerId = isLocal ? 0 : 1
            });

            if (!isLocal)
                return e;

            EntityManager.AddComponentData(e, new UpdateUI());
            EntityManager.AddComponentData(e, new ServerEntity());
            EntityManager.AddComponentData(e, new Connection());

            return e;
        }
    }
}