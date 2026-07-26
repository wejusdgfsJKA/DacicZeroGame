using UnityEngine;

namespace DacicZero.Data {
    /// <summary> scriptableobject storing mission configuration, scene routing, and rewards. </summary>
    [CreateAssetMenu(fileName = "NewMissionData", menuName = "DacicZero/Mission Data", order = 1)]
    public class MissionDataSO : ScriptableObject {
        [field: Header("Mission Identity")]
        [field: Tooltip("unique identifier for this mission.")]
        [field: SerializeField] public string MissionId { get; private set; }

        [field: Tooltip("title displayed in mission selector.")]
        [field: SerializeField] public string MissionTitle { get; private set; }

        [field: Tooltip("description of mission objectives.")]
        [field: TextArea(3, 5)]
        [field: SerializeField] public string MissionDescription { get; private set; }

        [field: Header("Scene Routing")]
        [field: Tooltip("scene name to load for this mission.")]
        [field: SerializeField] public string SceneName { get; private set; }

        [field: Tooltip("-1 uses scene name instead of build index.")]
        [field: SerializeField] public int SceneBuildIndex { get; private set; } = -1;

        [field: Header("Mission Status")]
        [field: Tooltip("true if mission has been completed.")]
        [field: SerializeField] public bool IsCleared { get; set; }

        [field: Header("Rewards")]
        [field: Tooltip("scrap material awarded upon completion.")]
        [field: SerializeField] public int ScrapReward { get; private set; }
    }
}
