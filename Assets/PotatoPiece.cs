using UnityEngine;

public class PotatoPiece : MonoBehaviour
{
    public static readonly float Radius = .1f;
    public bool IsEaten { get; private set; }

    private SpriteRenderer _sr;

    public void Awake()
    {
        enabled = false;

        _sr = GetComponentInChildren<SpriteRenderer>();
        _sr.enabled = false;

        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = Radius;
    }

    public void BecomeEaten()
    {
        IsEaten = true;
        _sr.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsEaten ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
