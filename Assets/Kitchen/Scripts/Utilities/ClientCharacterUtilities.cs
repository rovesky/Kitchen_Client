using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class ClientCharacterUtilities
    {
        private static Entity entityPrefab;
        private static GameObject presentationPrefab;
     //   private static GameObject chooseEffectPrefab;

        public static void Init()
        {
            entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Character/CharacterEntity") as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

            presentationPrefab = Resources.Load("Character/Character1") as GameObject;
       //     chooseEffectPrefab = Resources.Load("Effect/ChooseEffect") as GameObject;
        }

        public static Entity CreateCharacter(EntityManager entityManager,float3 position)
        {
            var e = entityManager.Instantiate(entityPrefab);
          
            var rotation = Quaternion.identity;

            CreateCharacterUtilities.CreateCharacterComponent(entityManager, e, position, rotation);
            

            var characterPresentation = Object.Instantiate(presentationPrefab);
            characterPresentation.SetActive(false);

            
         //   var chooseEffect= Object.Instantiate(chooseEffectPrefab);
         //   chooseEffect.SetActive(false);
            

            entityManager.AddComponentData(e, new  CharacterPresentation()
            {
                CharacterObject = characterPresentation,
                ChooseEffect = null
            });

            return e;
        }
      
    }
}