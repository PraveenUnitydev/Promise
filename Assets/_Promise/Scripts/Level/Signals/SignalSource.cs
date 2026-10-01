using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Something that turns on and off (lever, pressure plate, trigger area, memory light) and drives any number of
// SignalReceivers (platforms, doors, lights). Wire targets in the Inspector; a receiver can listen to several sources.
public abstract class SignalSource : MonoBehaviour
{
    [Tooltip("Receivers driven by this source.")]
    [SerializeField] protected List<SignalReceiver> targets = new List<SignalReceiver>();

    [Tooltip("One-off effects such as sounds. Not rewound on respawn; use receivers for anything that changes the level.")]
    [SerializeField] UnityEvent onActivated = new UnityEvent();
    [SerializeField] UnityEvent onDeactivated = new UnityEvent();

    public bool IsOn { get; private set; }

    protected virtual void Awake()
    {
        foreach (var target in targets)
        {
            if (target != null)
                target.RegisterSource(this);
        }
    }

    protected void SetOn(bool on)
    {
        if (on == IsOn)
            return;
        IsOn = on;
        foreach (var target in targets)
        {
            if (target != null)
                target.SetSignal(this, on);
        }
        (on ? onActivated : onDeactivated).Invoke();
        OnStateChanged(on);
    }

    /// <summary>Sets the state without notifying targets or events. Used when rewinding; receivers rewind themselves.</summary>
    protected void SetOnSilently(bool on)
    {
        IsOn = on;
        OnStateChanged(on);
    }

    /// <summary>Visual feedback on this source (lights, animator).</summary>
    protected virtual void OnStateChanged(bool on) { }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = IsOn ? Color.green : Color.cyan;
        foreach (var target in targets)
        {
            if (target != null)
                Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
