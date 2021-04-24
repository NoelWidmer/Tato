using UnityEngine;
using UnityEngine.SceneManagement;

public class Worm : MonoBehaviour
{
    public LayerMask PotatoPieceLayerMask;

    private TatoInputActions _input;
    private bool _startedMoving;
    private float _speed = 2f;
    private Vector2 _movementDirection;
    private Vector3 _movementDirection3D => new Vector3(_movementDirection.x, _movementDirection.y, 0f);
    private int _piecesEaten;

    private static readonly float _maxLifetime = 5f;
    private static readonly float _lifetimeGainedByPiece = .05f;
    private float _remainingLifetime = _maxLifetime;
    public float RemainingLifetimeFraction => 1f / _maxLifetime * _remainingLifetime;

    private void Awake()
    {
        _input = new TatoInputActions();
        _input.Enable();

        Cursor.visible = false;
    }

    private void Update()
    {
        var input = _input.Default.Move.ReadValue<Vector2>();

        if(input.magnitude > .5f)
        {
            _startedMoving = true;
            _movementDirection = input.normalized;
        }

        var velocity = _movementDirection3D * _speed * Time.deltaTime;

        foreach(var hit in Physics2D.RaycastAll(transform.position, velocity.normalized, velocity.magnitude, PotatoPieceLayerMask))
        {
            var piece = hit.collider.GetComponent<PotatoPiece>();

            if(piece.IsEaten == false)
            {
                _piecesEaten += 1;
                piece.BecomeEaten();
                AddLifetime();
            }
        }

        if(_startedMoving)
        {
            transform.position += velocity;
        }

        ReduceLifetime();
    }

    private void AddLifetime()
    {
        _remainingLifetime += _lifetimeGainedByPiece;

        if(_remainingLifetime > _maxLifetime)
        {
            _remainingLifetime = _maxLifetime;
        }
    }

    private void ReduceLifetime()
    {
        _remainingLifetime -= Time.deltaTime;
        Debug.Log("lifetime: " + _remainingLifetime);

        if(_remainingLifetime <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("died");
        Destroy(gameObject);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _movementDirection3D);
    }
}
