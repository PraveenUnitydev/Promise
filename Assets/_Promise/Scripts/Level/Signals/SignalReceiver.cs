using System.Collections.Generic;
using UnityEngine;

// Base for anything a SignalSource can drive. Combines all sources that target it (Any / All) and reports a single
// on/off state through OnSignalChanged. Rewinds its signal state on respawn; subclasses rewind their own motion.
public abstract class SignalReceiver : MonoBehaviour, IResettable
{
    public enum Logic
    {
        [Tooltip("On while at least one source is on.")] Any,
        [Tooltip("On only while every source is on (e.g. two pressure plates).")] All
    }

    [Header("Signal")]
    [SerializeField] Logic _logic = Logic.Any;
    [Tooltip("Swap on and off, e.g. a door that is open until a plate is pressed.")]
    [SerializeField] bool _invert;

    readonly HashSet<SignalSource> _sources = new HashSet<SignalSource>();
    readonly HashSet<SignalSource> _activeSources = new HashSet<SignalSource>();
    readonly List<SignalSource> _savedActiveSources = new List<SignalSource>();
    bool _savedIsOn;

    public bool IsOn { get; private set; }

    public void RegisterSource(SignalSource source) => _sources.Add(source);

    public void SetSignal(SignalSource source, bool on)
    {
        if (on)
            _activeSources.Add(source);
        else
            _activeSources.Remove(source);
        Evaluate();
    }

    protected virtual void Start()
    {
        // Applies the initial state, which matters for inverted receivers (on with no signal).
        Evaluate();
    }

    void Evaluate()
    {
        bool raw = _logic == Logic.All
            ? _sources.Count > 0 && _activeSources.Count == _sources.Count
            : _activeSources.Count > 0;
        bool on = raw != _invert;
        if (on == IsOn)
            return;
        IsOn = on;
        OnSignalChanged(on);
    }

    protected abstract void OnSignalChanged(bool on);

    public virtual void CaptureState()
    {
        _savedActiveSources.Clear();
        _savedActiveSources.AddRange(_activeSources);
        _savedIsOn = IsOn;
    }

    public virtual void RestoreState()
    {
        _activeSources.Clear();
        foreach (var source in _savedActiveSources)
            _activeSources.Add(source);
        IsOn = _savedIsOn;
    }
}
