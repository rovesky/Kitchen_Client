﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class ClientCharacterUtilities
    {
        private static Entity entityPrefab;
        private static GameObject presentationPrefab;

        public static void Init()
        {
            entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Character/CharacterEntity") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

            presentationPrefab = Resources.Load("Character/CharacterRobot") as GameObject;
        }

        public static Entity CreateCharacter(EntityManager entityManager,float3 position, bool isLocal,int id)
        {
            var e = entityManager.Instantiate(entityPrefab);
            var characterPresentation = Object.Instantiate(presentationPrefab);
          
            var rotation = Quaternion.identity;

            CreateCharacterUtilities.CreateCharacterComponent(entityManager, e, position, rotation);

            var presentationEntity = characterPresentation.GetComponentInChildren<GameObjectEntity>() == null
                ? Entity.Null
                : characterPresentation.GetComponentInChildren<GameObjectEntity>().Entity;

            entityManager.SetComponentData(e, new Character
            {
                PresentationEntity = presentationEntity
            });
            
            entityManager.SetComponentData(e, new ReplicatedEntityData
            {
                Id =  id,
                PredictingPlayerId = isLocal ? 0 : 1
            });

            if (!isLocal)
                return e;


            entityManager.AddComponentData(e, new ServerEntity());
            entityManager.AddComponentData(e, new UpdateUI());
            entityManager.AddComponentData(e, new Connection());

            return e;
        }
      
    }
}