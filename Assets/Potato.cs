using UnityEngine;

public class Potato : MonoBehaviour
{
    public GameObject PotatoPiecePrefab;
    public GameObject PotatoPiecesMask;

    private PolygonCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        GeneratePotatoPieces();
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
            //var potatoPiece = go.GetComponentInChildren<SpriteRenderer>().renderingLayerMask = PotatoPiecesMask;
        }            
    }
}
