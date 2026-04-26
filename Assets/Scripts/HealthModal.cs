using System;
using UnityEngine;

[Serializable]
public class HealthModal
{
    [field: SerializeField] public float Max { get; private set; } = 10f;
    [field: SerializeField] public float Current { get; private set; } = 10f;

    public event Action<float, float> Changed;
    public event Action Depleted;

    public void SetMax(float max, bool fillToMax = true)
    {
        Max = Mathf.Max(1f, max);
        if (fillToMax) Current = Max;
        ClampAndNotify();
    }

    public void Fill()
    {
        Current = Max;
        ClampAndNotify();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || Current <= 0f) return;
        Current -= amount;
        ClampAndNotify();
        if (Current <= 0f) Depleted?.Invoke();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || Current <= 0f) return;
        Current += amount;
        ClampAndNotify();
    }

    private void ClampAndNotify()
    {
        Current = Mathf.Clamp(Current, 0f, Max);
        Changed?.Invoke(Current, Max);
    }
}