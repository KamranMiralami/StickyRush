using UnityEngine;

public abstract class ScriptableObjectBehavior : ScriptableObject
{
    public virtual void Initialize() { }
    public virtual void Update() { }
    public virtual void OnApplicationPause(bool pause) { }
    public virtual void OnApplicationQuit() { }
}