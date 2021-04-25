using System.Collections.Generic;
using UnityEngine;

public class Potato : MonoBehaviour
{
    public GameObject PotatoPiecePrefab;
    public GameObject ExitPrefab;
    public float ExitRadius;

    private PolygonCollider2D _collider;
    private List<PotatoPiece> _potatoPieces = new List<PotatoPiece>();

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        SetupFirstStage();
    }

    private void SetupFirstStage()
    {
        GeneratePotatoPieces();
        SpawnExit();
    }

    private void SpawnExit()
    {
        var exitDirection = new Vector3(Random.value, Random.value, 0f).normalized;
        var exitDistanceFromCenter = ExitRadius * Random.value;
        var exitPosition = transform.position + exitDirection * exitDistanceFromCenter;
        Instantiate(ExitPrefab, exitPosition, Quaternion.identity);
    }

    private void GeneratePotatoPieces()
    {
        var dimension = 100;

        for(var x = 0; x < dimension; x += 1)
        {
            for(var y = 0; y < dimension; y += 1)
            {
                GeneratePotatoPiece(x, y);
            }
        }
    }

    private void GeneratePotatoPiece(int x, int y)
    {
        var position = new Vector2(x * PotatoPiece.Radius, y * PotatoPiece.Radius);

        // add offset
        var offset = new Vector2(-4f, -3.5f);
        position += offset;

        // add extra offset for odd rows
        if(y % 2 != 0)
        {
            position += new Vector2(PotatoPiece.Radius, 0f);
        }

        if(_collider.OverlapPoint(position))
        {
            var go = Instantiate(PotatoPiecePrefab, position, Quaternion.identity, transform);
            var potatoPiece = go.GetComponent<PotatoPiece>();
            _potatoPieces.Add(potatoPiece);
        }            
    }

    public void OnExited()
    {
        foreach(var potatoPiece in _potatoPieces)
        {
            if(potatoPiece.IsEaten)
            {
                potatoPiece.BecomeEatable();
            }
        }

        SpawnExit();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, ExitRadius);
    }
}
