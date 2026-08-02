using System.Collections.Generic;
using UnityEngine;

namespace DacicZero.NPC {
    public class NPCDirector : MonoBehaviour {
        
        [Header("Managed NPCs")]
        [Tooltip("List of all NPCs. If left empty, it will auto-populate on awake.")]
        [SerializeField] private List<NPCController> _npcsInScene = new List<NPCController>();

        public IReadOnlyList<NPCController> NpcsInScene => _npcsInScene;

        private void Awake() {
            if (_npcsInScene.Count == 0) {
                var foundNPCs = Object.FindObjectsByType<NPCController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                _npcsInScene.AddRange(foundNPCs);
                
                if (_npcsInScene.Count > 0) 
                    Debug.Log($"[NPCDirector] auto-populated {_npcsInScene.Count} NPCs into the Director.");
            }
        }

        private void Start() => RefreshNPCVisibility();

        public void RefreshNPCVisibility() {
            foreach (var npc in _npcsInScene) 
                if (npc != null) 
                    npc.gameObject.SetActive(true);
        }
    }
}