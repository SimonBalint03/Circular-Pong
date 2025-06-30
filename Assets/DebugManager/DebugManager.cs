using System.Collections.Generic;
using UnityEngine;

namespace DebugManager
{
    public class DebugManager : MonoBehaviour
    {
        [Header("Debug Panel Settings")]
        public List<MonoBehaviour> scriptsToMonitor;

        private GameObject debugCanvas;

        void Start()
        {
            CreateUI();
        }

        void Update()
        {
        
        }

        private void CreateUI()
        {
            debugCanvas = Resources.Load<GameObject>("Components/DebugCanvas");
            Instantiate(debugCanvas, transform);
        }

    
    }
}
