using System.Collections.Generic;
using UnityEngine;

public class PenetrableCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private float _sphareCastRadius;

    [SerializeField] private List<PenetrableGameObject> _penetrableGameObjects;

    private const int HitBufferSize = 32;
    private readonly RaycastHit[] _hitBuffer = new RaycastHit[HitBufferSize];
    private readonly HashSet<PenetrableGameObject> _hitOnFrame = new();
    private void LateUpdate()
    {
        var camPos = transform.position;
        var sphereCastDirection = (_target.position - camPos);
        sphereCastDirection.Normalize();
        var sphereCastDistance = (_target.position - camPos).magnitude - _sphareCastRadius;
        var hitCount = Physics.SphereCastNonAlloc(camPos, _sphareCastRadius, sphereCastDirection, _hitBuffer, sphereCastDistance,
            LayerMaskDefine.PenetrableLayerMask, QueryTriggerInteraction.Ignore);

        _hitOnFrame.Clear();
        for (var i = 0; i < hitCount; i++)
        {
            var hit = _hitBuffer[i];
            var penetrableGameObject = hit.transform.gameObject.GetComponent<PenetrableGameObject>();
            if (penetrableGameObject == null)
                continue;

            _hitOnFrame.Add(penetrableGameObject);
        }

        foreach (var penetrableGameObject in _penetrableGameObjects)
        {
            penetrableGameObject.SetPenetrated(_hitOnFrame.Contains(penetrableGameObject));
        }
    }
}