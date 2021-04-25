using System.Collections.Generic;
using UnityEngine;

public class Potato : MonoBehaviour
{
    public GameObject PotatoPiecePrefab;
    public GameObject ExitPrefab;
    public GameObject StarPrefab;
    public SpriteRenderer FullPotatoSprite;
    public float ExitRadius;
    public float StarRadius;

    private Worm _worm;
    private PolygonCollider2D _collider;
    private List<PotatoPiece> _potatoPieces = new List<PotatoPiece>();

    private void Awake()
    {
        _worm = FindObjectOfType<Worm>();
        _collider = GetComponentInChildren<PolygonCollider2D>();
        SetupFirstStage();
    }

    private void SetupFirstStage()
    {
        GeneratePotatoPieces();
        SpawnExit();
        SpawnStar();
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

        if(IsInsidePotato(position))
        {
            var go = Instantiate(PotatoPiecePrefab, position, Quaternion.identity, transform);
            var potatoPiece = go.GetComponent<PotatoPiece>();
            _potatoPieces.Add(potatoPiece);
        }
    }

    public bool IsInsidePotato(Vector2 point)
    {
        return _collider.OverlapPoint(point);
    }

    private void SpawnExit()
    {
        var exitDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        var exitDistanceFromCenter = ExitRadius * Random.value;
        var exitPosition = transform.position + exitDirection * exitDistanceFromCenter;
        Instantiate(ExitPrefab, exitPosition, Quaternion.identity);
    }

    private void SpawnStar()
    {
        var starPosition = GetStarSpawnPosition();
        Instantiate(StarPrefab, starPosition, Quaternion.identity);
    }

    public Vector3 GetStarSpawnPosition()
    {
        var starDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        var minDistance = ExitRadius;
        var variableDistance = StarRadius - minDistance;
        var starDistanceFromCenter = variableDistance * Random.value + minDistance;
        return transform.position + starDirection * starDistanceFromCenter;
    }

    private void LateUpdate()
    {
        bool shouldBeActive;

        if (IsInsidePotato(_worm.transform.position))
        {
            shouldBeActive = false;
        }
        else
        {
            shouldBeActive = true;
        }

        if (shouldBeActive != FullPotatoSprite.gameObject.activeSelf)
        {
            FullPotatoSprite.gameObject.SetActive(shouldBeActive);
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

    public void OnStarCollected()
    {
        SpawnStar();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, ExitRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StarRadius);
    }
}
