using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Worm : MonoBehaviour
{
    public bool RemoveBehaviourOnPlay;

    private Potato _potato;
    private GameState _gameState;

    // inspector
    public LayerMask WormObstacleLayerMask;
    public Canvas GameplayCanvas;
    public DeatchScreen DeathCanvas;
    public Text Tutorial;

    // eyes
    public List<Transform> NormalEyes;
    public List<Transform> DeadEyes;

    // sound
    public AudioSource PotatoSound;
    public AudioSource ChipSound;
    public AudioSource HurtSound;
    public AudioSource StarveSound;

    // movement
    private TatoInputActions _input;
    private bool _isMovementEnabled;
    private static readonly float _normalSpeed = 2f;
    private static readonly float _wetSpeed = 1f;
    private float _currentSpeed;
    private Vector3 _movementDirection;

    //lifetime
    public float RemainingLifetimeFraction => 1f / _maxLifetime * _remainingLifetime;
    private static readonly float _maxLifetime = 5f;
    private static readonly float _lifetimeGainedByPiece = .0365f;
    private static readonly float _lifetimeMultiplierByChip = 12.5f;
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
        if (RemoveBehaviourOnPlay)
        {
            Destroy(this);
            return;
        }

        _potato = FindObjectOfType<Potato>();
        _gameState = FindObjectOfType<GameState>();

        if (_gameState.UseController)
        {
            Tutorial.text = "Use the left stick to control Tato.";
        }
        else
        {
            Tutorial.text = "Left mouse click to make Tato follow your cursor.";
        }

        _input = new TatoInputActions();
        _input.Enable();

        for (var i = 0; i < _initialBodyPartCount; i++)
        {
            Grow();
        }

        SetBodyPartDelay(_defaultBodyPartDelay);
    }

    private void Update()
    {
        HandleInput();

        if(_isMovementEnabled)
        {
            _timePlaying += Time.deltaTime;

            if(_isExiting)
            {
                PerormBodyPartCatchUp();
            }
            else
            {
                var velocity = _movementDirection * _currentSpeed * Time.deltaTime;
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

    private void SetBodyPartDelay(float delay)
    {
        foreach(var bodyPart in _bodyParts)
        {
            bodyPart.Delay = delay;
        }
    }

    private void HandleInput()
    {
        if (_gameState.UseController)
        {
            var stickInput = _input.Default.Move.ReadValue<Vector2>();

            if (stickInput.magnitude > 0f)
            {
                EnableMovement();

                if (Application.isEditor == false)
                {
                    // no idea why the built game has an inverted y axis -_-
                    stickInput *= new Vector2(1f, -1f);
                }

                stickInput = stickInput.normalized;
                _movementDirection = new Vector3(stickInput.x, stickInput.y, 0f).normalized;
            }
        }
        else
        {
            if (_isMovementEnabled == false)
            {
                if (_input.Default.MouseStart.ReadValue<float>() != 0f)
                {
                    EnableMovement();
                }
            }

            if (_isMovementEnabled)
            {
                var mousePosition = _input.Default.MoveMouse.ReadValue<Vector2>();
                var mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
                var vector = mousePositionInWorld - transform.position;
                _movementDirection = new Vector3(vector.x, vector.y, 0f).normalized;
            }
        }
    }

    private void EnableMovement()
    {
        if (_isMovementEnabled == false)
        {
            _isMovementEnabled = true;
            Tutorial.gameObject.SetActive(false);
        }
    }

    private RaycastHit2D[] GetHits(Vector3 velocity)
    {
        var radius = .14f;
        var origin = transform.position + velocity.normalized * radius;
        
        Debug.DrawLine(origin, origin + velocity, Color.blue);

        return Physics2D.RaycastAll(
            origin,
            velocity.normalized,
            velocity.magnitude, 
            WormObstacleLayerMask);
    }

    private void EatOverlapingPotatoPieces(RaycastHit2D[] hits)
    {
        var piecesEaten = 0;

        foreach(var hit in hits)
        {
            var piece = hit.collider.GetComponent<PotatoPiece>();

            if(piece != null && piece.IsEaten == false)
            {
                piecesEaten += 1;
                EatPiece(piece);
            }
        }

        if (piecesEaten > 1 && PotatoSound.isPlaying == false)
        {
            PotatoSound.Play();
        }
    }

    private void CollectStars(RaycastHit2D[] hits)
    {
        foreach(var starHit in GetHitsByTag(hits, "Star"))
        {
            ChipSound.Play();
            AddLifetime(_lifetimeMultiplierByChip);
            Stars += 1;
            Destroy(starHit.collider.gameObject);
            var potato = FindObjectOfType<Potato>();
            potato.OnStarCollected();
        }
    }

    private void SetSpeed(RaycastHit2D[] hits)
    {
        if (_potato.IsInsidePotato(transform.position) == false)
        {
            if (hits.Any(hit => hit.collider.tag == "Drop"))
            {
                _currentSpeed = _wetSpeed;
                return;
            }
        }

        _currentSpeed = _normalSpeed;
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

        if (_isExiting)
        {
            var lastBodyPartPosition = _bodyParts.Last().transform.position;
            var distanceToLastBodyPart = Vector3.Distance(transform.position, lastBodyPartPosition);

            if (distanceToLastBodyPart < .1f)
            {
                _isExiting = false;
                SetNormalEyes(true);
                var potato = FindObjectOfType<Potato>();
                potato.OnExited();
                SetBodyPartDelay(_defaultBodyPartDelay);
            }
        }
    }

    private void EatPiece(PotatoPiece piece)
    {
        _piecesEaten += 1;
        piece.BecomeEaten();
        AddLifetime(1f);

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
        _bodyParts.Add(bodyPart);
        bodyPart.Follow(leader, _bodyParts.Count);
    }

    private void AddLifetime(float multiplier)
    {
        _remainingLifetime += _lifetimeGainedByPiece * multiplier;

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
                StarveSound.Play();
                info = "You let Tato starve :(";
                hint = "Be sure to continuously feed Tato with fresh potato.";
                break;
            case DeathReason.Suicide:
                HurtSound.Play();
                info = "You hurt Tato :(";
                hint = "Don't make Tato bite herself!";
                break;
            default:
                throw new NotImplementedException();
        }

        if (Stars > _gameState.HighScore)
        {
            _gameState.HighScore = Stars;
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
}

public enum DeathReason
{ 
    Lifetime, 
    Suicide
}
