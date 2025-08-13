using UnityEngine;

public class ObjectData<T> : ScriptableObject
{
    [field: SerializeField] public T Prefab { get; protected set; }
}
