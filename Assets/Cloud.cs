using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private static readonly float _dropsPerSecond = .2f;

    public GameObject DropPrefab;

    private Transform _dropParent;
    private Potato _potato;
    private Worm _worm;

    private float _timeSinceLastDrop;

    private void Start()
    {
        _potato = FindObjectOfType<Potato>();
        _worm = FindObjectOfType<Worm>();
        _dropParent = new GameObject("Drops").transform;
    }

    private void Update()
    {
        var dropsPerSecond = (_worm.Depth - 1) * _dropsPerSecond;
        var dropsSpanInterval = 1f / dropsPerSecond;

        _timeSinceLastDrop += Time.deltaTime;
        if(_timeSinceLastDrop > dropsSpanInterval)
        {
            _timeSinceLastDrop = 0f;
            SpawnDrop();
        }
    }

    private void SpawnDrop()
    {
        var spawnPosition = _potato.GetStarSpawnPosition();

        while(_potato.IsInsidePotato(spawnPosition))
        {
            spawnPosition += spawnPosition.normalized * .1f;
        }

        Instantiate(DropPrefab, spawnPosition, Quaternion.identity, _dropParent);
    }
}
