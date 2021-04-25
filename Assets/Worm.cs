using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worm : MonoBehaviour
{
    private Potato _potato;

    // inspector
    public LayerMask WormObstacleLayerMask;
    public Canvas GameplayCanvas;
    public DeatchScreen DeathCanvas;

    // eyes
    public List<Transform> NormalEyes;
    public List<Transform> DeadEyes;

    // movement
    private TatoInputActions _input;
    private bool _wasInputProvidedAtLeastOnce;
    private static readonly float _normalSpeed = 2f;
    private static readonly float _wetSpeed = 1f;
    private float _speed;
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
    private int _requiredPotatoPiecesForGrowth = 50;
    private int _piecesEaten;

    // stats
    private float _timePlaying;
    public int Depth { get; private set; } = 1;
    public int Stars { get; private set; } = 0;

    // exit
    private bool _isExiting;
    private static readonly float _defaultBodyPartDelay = .1f;
    private static readonly float _exitingBodyPartDelay = .025f;

    private void Awake()
    {
        _potato = FindObjectOfType<Potato>();

        _input = new TatoInputActions();
        _input.Enable();

        //Cursor.visible = false;

        for(var i = 0; i < _initialBodyPartCount; i++)
        {
            Grow();
        }

        SetBodyPartDelay(_defaultBodyPartDelay);
    }

    private void Update()
    {
        HandleInput();

        if(_wasInputProvidedAtLeastOnce)
        {
            _timePlaying += Time.deltaTime;

            if(_isExiting)
            {
                PerormBodyPartCatchUp();
            }
            else
            {
                var velocity = _movementDirection * _speed * Time.deltaTime;
                var hits = GetHits(velocity);

                EatOverlapingPotatoPieces(hits);
                CollectStars(hits);
                SetSpeed(hits);

                var exitHits = GetHitsByTag(hits, "Exit");

                if(exitHits.Any())
                {
                    Destroy(exitHits.First().collider.gameObject);
                    Depth += 1;
                    _isExiting = true;

                    SetBodyPartDelay(_exitingBodyPartDelay);
                    SetNormalEyes(false);
                    StartCoroutine(OnEnterNewLayer());
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
    }

    private System.Collections.IEnumerator OnEnterNewLayer()
    {
        yield return new WaitForSeconds(_bodyParts.Count * .04f);
        _isExiting = false;
        SetNormalEyes(true);
        var potato = FindObjectOfType<Potato>();
        potato.OnExited();
        SetBodyPartDelay(_defaultBodyPartDelay);
    }

    private void SetBodyPartDelay(float delay)
    {
        foreach(var bodyPart in _bodyParts)
        {
            bodyPart.Delay = delay;
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

    private void SetSpeed(RaycastHit2D[] hits)
    {
        if(_potato.IsInsidePotato(transform.position) == false)
        {
            if(hits.Any(hit => hit.collider.tag == "Drop"))
            {
                _speed = _wetSpeed;
                Debug.Log("wet");
            }
            else
            {
                _speed = _normalSpeed;
                Debug.Log("normal");
            }
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
        PerormBodyPartCatchUp();
    }

    private void PerormBodyPartCatchUp()
    {
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
        enabled = false;

        ShowDeadEyes();

        string info;
        string hint;

        switch(reason)
        {
            case DeathReason.Lifetime:
                info = "You've let Tato starve  ;(";
                hint = "Hint: Continuously feed Tato tasty potato.";
                break;
            case DeathReason.Suicide:
                info = "You've let Tato hurt herself  ;(";
                hint = "Hint: Don't give Tato a chance to bite herself.";
                break;
            default:
                throw new NotImplementedException();
        }

        DeathCanvas.SetValues(
            info,
            hint, 
            Stars, 
            Depth, 
            _bodyParts.Count,
            _timePlaying
            );

        GameplayCanvas.gameObject.SetActive(false);
        DeathCanvas.gameObject.SetActive(true);
    }

    private void SetNormalEyes(bool isActive)
    {
        foreach(var normalEye in NormalEyes)
        {
            normalEye.gameObject.SetActive(isActive);
        }
    }

    private void ShowDeadEyes()
    {
        SetNormalEyes(false);

        foreach(var deadEye in DeadEyes)
        {
            deadEye.gameObject.SetActive(true);
        }
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
