using UnityEngine;

namespace FootStone.Kitchen
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        public Camera CameraMain;
        public Transform LeftEdge;
        public Transform RightEdge;
        public Transform TopEdge;
        public Transform BottomEdge;
        private void Start()
        {
            Instance = this;
        }

    }


}