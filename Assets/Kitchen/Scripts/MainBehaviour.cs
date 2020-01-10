using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class MainBehaviour : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
      
           var simulationSystemGroup = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SimulationSystemGroup>();
           simulationSystemGroup.AddSystemToUpdateList(  World.DefaultGameObjectInjectionWorld.CreateSystem<ClientSimulationSystemGroup>());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
