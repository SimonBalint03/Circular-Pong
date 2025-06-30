using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
    public string powerUpName;
    public float duration = 5f;
    public Sprite icon;

    public abstract void Apply(PlayerController player);
    public abstract void Remove(PlayerController player);
    public abstract bool CanApply(PlayerController player);
    public abstract bool CanRemove(PlayerController player);
}
