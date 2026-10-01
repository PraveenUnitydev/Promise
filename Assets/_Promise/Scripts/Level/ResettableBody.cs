using UnityEngine;

// Rewinds an object's transform, active state and Rigidbody2D on respawn. Put it on boxes, logs, rocks:
// anything the player can move or knock over.
public class ResettableBody : MonoBehaviour, IResettable
{
    Rigidbody2D _body;

    Transform _parent;
    Vector3 _position;
    Quaternion _rotation;
    bool _active;
    RigidbodyType2D _bodyType;
    bool _simulated;
    float _mass;

    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    public void CaptureState()
    {
        _parent = transform.parent;
        _position = transform.position;
        _rotation = transform.rotation;
        _active = gameObject.activeSelf;
        if (_body != null)
        {
            _bodyType = _body.bodyType;
            _simulated = _body.simulated;
            _mass = _body.mass;
        }
    }

    public void RestoreState()
    {
        transform.SetParent(_parent);
        transform.SetPositionAndRotation(_position, _rotation);
        gameObject.SetActive(_active);
        if (_body != null)
        {
            _body.bodyType = _bodyType;
            _body.simulated = _simulated;
            _body.mass = _mass;
            _body.position = _position;
            _body.rotation = _rotation.eulerAngles.z;
            if (_bodyType != RigidbodyType2D.Static)
            {
                _body.linearVelocity = Vector2.zero;
                _body.angularVelocity = 0f;
            }
        }
    }
}
