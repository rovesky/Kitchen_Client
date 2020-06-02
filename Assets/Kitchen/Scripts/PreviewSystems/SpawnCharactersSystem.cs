using FootStone.ECS;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnCharactersSystem : ComponentSystem
    {
        private EntityQuery spawnPointQuery;
        private Random random;

        protected override void OnCreate()
        {
            EntityManager.CreateEntity(typeof(LocalPlayer));
            SetSingleton(new LocalPlayer {PlayerId = -1, CharacterEntity = Entity.Null});

            spawnPointQuery = EntityManager.CreateEntityQuery(typeof(SpawnPoint));
            random = new Unity.Mathematics.Random(20);
        }

        protected override void OnUpdate()
        {
            var localPlayer = GetSingleton<LocalPlayer>();
            if (localPlayer.CharacterEntity != Entity.Null)
                return;

            var spawnPoints = spawnPointQuery.ToEntityArray(Allocator.TempJob);

            var pos0 = EntityManager.GetComponentData<LocalToWorld>(spawnPoints[0]).Position;

            localPlayer.CharacterEntity = CreateCharacter(pos0, true,0);
          //  EntityManager.AddComponentData(localPlayer.CharacterEntity, new LocalCharacter());
            SetSingleton(localPlayer);
          
          //  var pos1 = EntityManager.GetComponentData<LocalToWorld>(spawnPoints[1]).Position;
          //  var e = CreateCharacter(pos1, false,1);

            spawnPoints.Dispose();

        }

        private Entity CreateCharacter(float3 position, bool isLocal, int id)
        {
            var e = ClientCharacterUtilities.CreateCharacter(EntityManager, position);

            EntityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id = id,
                PredictingPlayerId = isLocal ? 0 : -1
            });
            EntityManager.AddComponentData(e, new ServerEntity());

            if (!isLocal)
                return e;

            //  EntityManager.AddComponentData(e, new UpdateUI());
            EntityManager.AddComponentData(e, new LocalCharacter());
            EntityManager.AddComponentData(e, new Connection());

            return e;
        }
      
    }
}