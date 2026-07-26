using UnityEngine;

namespace NPC {
    public enum Skills {
        Strength,
        Tech
    }
    [System.Serializable]
    public class Skill {
        [field: SerializeField] public Skills Type { get; protected set; }
        [field: SerializeField] public int Level { get; set; }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            Skill other = obj as Skill;
            if (other == null) return false;
            return Type == other.Type;
        }
        public override int GetHashCode() {
            return Type.GetHashCode();
        }
        public override string ToString() {
            return $"Level {Level} {Type.ToString()}.";
        }
    }
}
