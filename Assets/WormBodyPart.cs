using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBodyPart : MonoBehaviour
{
    public Transform Transform => transform;

    private Transform _leader;
    private Queue<(float Time, Vector3 Position)> _leaderPositions = new Queue<(float Time, Vector3 Position)>();
    private static readonly float delay = .1f;

    public void Follow(Transform leader)
    {
        _leader = leader;
    }

    private void Update()
    {
        _leaderPositions.Enqueue((Time.time, _leader.position));

        var peek = _leaderPositions.Peek();

        while(Time.time - delay >= peek.Time)
        {
            _leaderPositions.Dequeue();
            transform.position = peek.Position;
            peek = _leaderPositions.Peek();
        }
    }
}
