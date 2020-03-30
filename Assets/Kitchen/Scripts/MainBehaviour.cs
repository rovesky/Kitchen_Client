using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class MainBehaviour : MonoBehaviour
    {
        private int frameCount;
        private float passedTime;

        public bool IsPreview;


        // Start is called before the first frame update
        private void Start()
        {
            var simulationSystemGroup =
                World.DefaultGameObjectInjectionWorld.GetExistingSystem<SimulationSystemGroup>();

            if (IsPreview)
                simulationSystemGroup.AddSystemToUpdateList(World.DefaultGameObjectInjectionWorld
                    .CreateSystem<PreviewClientSimulationSystemGroup>());
            else
                simulationSystemGroup.AddSystemToUpdateList(World.DefaultGameObjectInjectionWorld
                    .CreateSystem<ClientSimulationSystemGroup>());
        }

        // Update is called once per frame
        private void Update()
        {
            frameCount++;
            passedTime += UnityEngine.Time.unscaledDeltaTime;
            if (passedTime >= 1.5f && frameCount>=1)
            {
                UIManager.Instance.UpdateFps(frameCount / passedTime);
                frameCount = 0;
                passedTime = 0.0f;
            }
        }
    }
}