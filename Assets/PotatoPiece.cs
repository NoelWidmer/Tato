using UnityEngine;

public class PotatoPiece : MonoBehaviour
{
    public static readonly float Radius = .05f;
    public bool IsEaten;

    public void Awake()
    {
        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = Radius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsEaten ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
