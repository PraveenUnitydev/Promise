using System.Collections.Generic;
using UnityEngine;

// Shows and hides objects with the signal: bridges that appear, barriers that vanish, lights.
public class SignalToggle : SignalReceiver
{
    [SerializeField] List<GameObject> _activeWhenOn = new List<GameObject>();
    [SerializeField] List<GameObject> _activeWhenOff = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        Apply(IsOn);
    }

    protected override void OnSignalChanged(bool on) => Apply(on);

    public override void RestoreState()
    {
        base.RestoreState();
        Apply(IsOn);
    }

    void Apply(bool on)
    {
        foreach (var go in _activeWhenOn)
        {
            if (go != null)
                go.SetActive(on);
        }
        foreach (var go in _activeWhenOff)
        {
            if (go != null)
                go.SetActive(!on);
        }
    }
}
