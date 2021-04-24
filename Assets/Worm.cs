using UnityEngine;

public class Worm : MonoBehaviour
{
    public LayerMask _potatoLayerMask;

    private TatoInputActions _input;
    private bool _startedMoving;
    private float _speed = 5f;
    private Vector2 _movementDirection;
    private Vector3 _movementDirection3D => new Vector3(_movementDirection.x, _movementDirection.y, 0f);
    private int _piecesEaten;

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

        foreach(var hit in Physics2D.RaycastAll(transform.position, velocity.normalized, velocity.magnitude, _potatoLayerMask))
        {
            var piece = hit.collider.GetComponent<PotatoPiece>();

            if(piece.IsEaten == false)
            {
                _piecesEaten += 1;
                piece.IsEaten = true;
            }
        }

        if(_startedMoving)
        {
            transform.position += velocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _movementDirection3D);
    }
}
