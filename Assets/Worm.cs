using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Worm : MonoBehaviour
{
    // inspector
    public LayerMask WormObstacleLayerMask;

    // movement
    private TatoInputActions _input;
    private bool _wasInputProvidedAtLeastOnce;
    private float _speed = 2f;
    private Vector3 _movementDirection;

    //lifetime
    public float RemainingLifetimeFraction => 1f / _maxLifetime * _remainingLifetime;
    private static readonly float _maxLifetime = 5f;
    private static readonly float _lifetimeGainedByPiece = .04f;
    private float _remainingLifetime = _maxLifetime;

    // body parts
    public GameObject WormBodyPartPrefab;
    private List<WormBodyPart> _bodyParts = new List<WormBodyPart>();
    private Transform _bodyPartParent;
    private int _initialBodyPartCount = 5;
    private int _requiredPotatoPiecesForGrowth = 60;
    private int _piecesEaten;

    // stats
    public int Depth { get; private set; } = 1;
    public int Stars { get; private set; } = 0;

    private void Awake()
    {
        _input = new TatoInputActions();
        _input.Enable();

        //Cursor.visible = false;

        for(var i = 0; i < _initialBodyPartCount; i++)
        {
            Grow();
        }
    }

    private void Update()
    {
        HandleInput();

        if(_wasInputProvidedAtLeastOnce)
        {
            var velocity = _movementDirection * _speed * Time.deltaTime;
            var hits = GetHits(velocity);

            EatOverlapingPotatoPieces(hits);
            CollectStars(hits);

            var exitHits = GetHitsByTag(hits, "Exit");

            if(exitHits.Any())
            {
                Destroy(exitHits.First().collider.gameObject);
                Depth += 1;
                var potato = FindObjectOfType<Potato>();
                potato.OnExited();
                Move(velocity);
            }
            else if(IsEatingSelf(hits, velocity))
            {
                Move(velocity);
                Die(DeathReason.Suicide);
            }
            else if(HasLifetimeLeft() == false)
            {
                Move(velocity);
                Die(DeathReason.Lifetime);
            }
            else
            {
                Move(velocity);
            }
        }
    }

    private void HandleInput()
    {
        var input = _input.Default.Move.ReadValue<Vector2>();

        if(input.magnitude > 0f)
        {
            _wasInputProvidedAtLeastOnce = true;
            _movementDirection = new Vector3(input.x, input.y, 0f).normalized;
        }
    }

    private RaycastHit2D[] GetHits(Vector3 velocity)
    {
        var origin = transform.position + velocity.normalized * .15f;
        var distance = velocity.magnitude - .15f;

        return Physics2D.RaycastAll(
            origin,
            velocity.normalized,
            distance, 
            WormObstacleLayerMask);
    }

    private void EatOverlapingPotatoPieces(RaycastHit2D[] hits)
    {
        foreach(var hit in hits)
        {
            var piece = hit.collider.GetComponent<PotatoPiece>();

            if(piece != null && piece.IsEaten == false)
            { 
                EatPiece(piece);
            }
        }
    }

    private void CollectStars(RaycastHit2D[] hits)
    {
        foreach(var starHit in GetHitsByTag(hits, "Star"))
        {
            Stars += 1;
            Destroy(starHit.collider.gameObject);
            var potato = FindObjectOfType<Potato>();
            potato.OnStarCollected();
        }
    }

    private RaycastHit2D[] GetHitsByTag(RaycastHit2D[] hits, string tag)
    {
        return hits.Where(hit => hit.collider.tag == tag).ToArray();
    }

    private bool IsEatingSelf(RaycastHit2D[] hits, Vector3 velocity)
    {
        foreach(var hit in hits)
        {
            if(hit.collider.GetComponent<WormBodyPart>() != null)
            {
                var vectorToBodyPart = hit.collider.transform.position - transform.position;

                if(vectorToBodyPart.magnitude > 0f)
                { 
                    if(Vector3.Dot(velocity.normalized, vectorToBodyPart.normalized) > 0f)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool HasLifetimeLeft()
    {
        _remainingLifetime -= Time.deltaTime;
        return _remainingLifetime > 0f;
    }

    private void Move(Vector3 velocity)
    {
        transform.position += velocity;
        transform.up = -velocity.normalized;

        foreach(var bodyPart in _bodyParts)
        {
            bodyPart.OnLeaderPositionUpdated();
        }
    }

    private void EatPiece(PotatoPiece piece)
    {
        _piecesEaten += 1;
        piece.BecomeEaten();
        AddLifetime();

        if(_piecesEaten % _requiredPotatoPiecesForGrowth == 0)
        {
            Grow();
        }
    }

    private void Grow()
    {
        Transform leader;

        if(_bodyParts.Any())
        {
            leader = _bodyParts.Last().transform;
        }
        else
        {
            _bodyPartParent = new GameObject("BodyParts").transform;
            leader = transform;
        }

        var go = Instantiate(WormBodyPartPrefab, leader.position, Quaternion.identity, _bodyPartParent);
        go.name = _bodyParts.Count.ToString();

        var bodyPart = go.GetComponent<WormBodyPart>();
        bodyPart.Follow(leader);
        _bodyParts.Add(bodyPart);
    }

    private void AddLifetime()
    {
        _remainingLifetime += _lifetimeGainedByPiece;

        if(_remainingLifetime > _maxLifetime)
        {
            _remainingLifetime = _maxLifetime;
        }
    }

    private void Die(DeathReason reason)
    {
        Destroy(gameObject);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _movementDirection);
    }
}

public enum DeathReason
{ 
    Lifetime, 
    Suicide
}
