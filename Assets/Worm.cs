using System.Collections.Generic;
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

    public GameObject WormBodyPart;
    private List<WormBodyPart> _bodyParts = new List<WormBodyPart>();
    private int _startingWormyBodyPartCount = 10;

    private void Awake()
    {
        _input = new TatoInputActions();
        _input.Enable();

        Cursor.visible = false;

        for(var i = 0; i < _startingWormyBodyPartCount; i++)
        {
            ExpandWormLength();
        }
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
                EatPiece(piece);
            }
        }

        if(_startedMoving)
        {
            transform.position += velocity;
        }

        ReduceLifetime();
    }

    private void EatPiece(PotatoPiece piece)
    {
        _piecesEaten += 1;
        piece.BecomeEaten();
        AddLifetime();
    }

    private void ExpandWormLength()
    {
        var go = Instantiate(WormBodyPart, transform.position, Quaternion.identity);
        var bodyPart = go.GetComponent<WormBodyPart>();
        _bodyParts.Add(bodyPart);

        if(_bodyParts.Count == 1)
        {
            bodyPart.Follow(transform);
        }
        else
        {
            bodyPart.Follow(_bodyParts[_bodyParts.Count - 2].Transform);
        }
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
