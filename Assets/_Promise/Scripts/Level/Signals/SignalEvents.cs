using UnityEngine;
using UnityEngine.Events;

// Calls UnityEvents when the signal changes, for anything without a dedicated receiver (animators, sounds, particles).
// Events are not rewound on respawn, so prefer SignalMover / SignalToggle for changes the player relies on.
public class SignalEvents : SignalReceiver
{
    [SerializeField] UnityEvent _onTurnedOn = new UnityEvent();
    [SerializeField] UnityEvent _onTurnedOff = new UnityEvent();

    protected override void OnSignalChanged(bool on) => (on ? _onTurnedOn : _onTurnedOff).Invoke();
}
