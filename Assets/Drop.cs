using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    private static Vector3 _falDirection = new Vector3(-1f, -3f, 0f).normalized;
    private static float _fallDistance = 10f;
    private static readonly float _fallSpeed = 20f;
    private static readonly float _lifetime = 15f;

    public Transform BigSplashGameObject;
    public Transform DropGameObject;

    private Vector3 _landPosition;
    private bool _landed;
    private float _distanceFallen;

    private float _timeSinceLanded;

    private void Awake()
    {
        _landPosition = transform.position;
        transform.position -= _falDirection * _fallDistance;
    }

    private void Update()
    {
        if (_landed)
        {
            _timeSinceLanded += Time.deltaTime;

            if (_timeSinceLanded > _lifetime)
            {
                Destroy(gameObject);
            }            
        }
        else
        {
            var velocity = _falDirection * _fallSpeed * Time.deltaTime;
            _distanceFallen += velocity.magnitude;

            if (_distanceFallen > _fallDistance)
            {
                _landed = true;
                transform.position = _landPosition;
                DropGameObject.gameObject.SetActive(false);
                BigSplashGameObject.gameObject.SetActive(true);
            }
            else
            {
                transform.position += velocity;
            }
        }
    }
}
