using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class VerticalActuator : MovementActuator
{
   
    [SerializeField]
    private LayerMask _layersToCollide; // Defines which layers this object can collide with

    // Movement direction options
    private enum Direction
    {
        Down = -1,
        Up = 1
    }

    // Reaction type when a collision occurs
    public enum OnCollisionReaction
    {
        None = 0,
        Bounce = 1,
        Destroy = 2
    }

    [SerializeField, HideInInspector] private float _speed;
    [SerializeField, HideInInspector] private bool _throw; // If true, movement is applied only once
    [SerializeField, HideInInspector] private float _goalSpeed;
    [SerializeField, HideInInspector] private float _interpolationTime = 0;

    private float _initialSpeed = 0;

    [Tooltip("Movement direction")]
    [SerializeField, HideInInspector] private Direction _direction = Direction.Up;

    private float _time;
    private Rigidbody2D _rigidbody;
    private EasingFunction.Function _easingFunc;

    [SerializeField, HideInInspector] private OnCollisionReaction _onCollisionReaction = OnCollisionReaction.None;

    AnimatorManager _animatorManager;
    [SerializeField, HideInInspector] private bool _followPlayer = false;
    private GameObject _playerReference;

    // Called when the actuator starts
    public override void StartActuator()
    {
        _animatorManager = this.gameObject.GetComponent<AnimatorManager>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _easingFunc = EasingFunction.GetEasingFunction(_easingFunction);
        _time = 0;

        if (_isAccelerated)
        {
            _speed = _rigidbody.velocity.x;
        }

        _initialSpeed = _speed;

        if (_throw)
        {
            ApplyForce(); // Apply force once if "throw" is enabled
        }

        if (_followPlayer)
        {
            var objectsWithPlayerTagArray = GameObject.FindGameObjectsWithTag("Player");
            if (objectsWithPlayerTagArray.Length == 0)
            {
                Debug.LogWarning("There was no object with Player tag, the projectile angle won't be controlled");
            }
            else
            {
                _playerReference = objectsWithPlayerTagArray[0];
                if (_playerReference == null)
                {
                    Debug.LogWarning("Player reference was null, the actuator may not be precise.");
                }
                else
                {
                    Vector3 direction = _playerReference.transform.position - transform.position;
                    _direction = direction.y > 0 ? Direction.Up : Direction.Down;
                }
            }
        }

        // Update animator state based on movement direction
        if (_animatorManager != null)
        {
            _animatorManager.ChangeSpeedY(_initialSpeed);
            if (_direction == Direction.Up)
                _animatorManager.UpDirection();
            else
                _animatorManager.DownDirection();
        }
    }

    // Called when the actuator is destroyed
    public override void DestroyActuator()
    {
        // No specific cleanup required currently
    }

    // Called every frame if is in the actual State
    public override void UpdateActuator()
    {
        // Continuously apply force unless it's a "throw"
        if (!_throw) ApplyForce();
    }

    // Applies the movement force to the Rigidbody2D
    private void ApplyForce()
    {
        _time += Time.deltaTime;

        // Update direction based on player's Y position
        if (_followPlayer && _playerReference != null)
        {
            Vector3 direction = _playerReference.transform.position - transform.position;
            _direction = direction.y > 0 ? Direction.Up : Direction.Down;
        }
        else if (_playerReference == null)
        {
            Debug.LogWarning("Player reference was null, the actuator may not be precise.");
        }

        int dirValue = (int)_direction;

        if (!_isAccelerated)
        {
            // Uniform motion (MRU)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _speed * dirValue);
        }
        else
        {
            // Accelerated motion (MRUA) with easing
            float t = (_time / _interpolationTime);
            float easedSpeed = _easingFunc(_initialSpeed, _goalSpeed, t);

            if (t >= 1.0f)
            {
                _speed = _goalSpeed;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _goalSpeed * dirValue);
            }
            else
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, easedSpeed * dirValue);
                _speed = easedSpeed;
            }

            // Update animation speed
            if (_animatorManager != null) _animatorManager.ChangeSpeedY(_rigidbody.velocity.y);
        }
    }

    // Handles collisions with defined layers and reacts accordingly
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collisions not in selected layers or with no defined reaction
        if ((_layersToCollide.value & (1 << collision.gameObject.layer)) == 0 || _onCollisionReaction == OnCollisionReaction.None) return;

        ContactPoint2D contact = collision.contacts[0];
        Vector2 normal = contact.normal;

        // Check vertical collision
        if (Mathf.Abs(normal.x) < Mathf.Abs(normal.y))
        {
            if (_onCollisionReaction == OnCollisionReaction.Bounce)
            {
                bool correctCollision = (_direction == Direction.Up && normal.y < 0) || (_direction == Direction.Down && normal.y > 0);
                if (correctCollision)
                {
                    // Reverse direction
                    _direction = _direction == Direction.Up ? Direction.Down : Direction.Up;

                    // Update animation
                    if (_animatorManager != null)
                    {
                        _animatorManager.RotateSpriteY();
                        if (_direction == Direction.Up)
                            _animatorManager.UpDirection();
                        else
                            _animatorManager.DownDirection();
                    }
                }
            }
            else if (_onCollisionReaction == OnCollisionReaction.Destroy)
            {
                // Call animation destroy or destroy GameObject directly
                if (_animatorManager != null && _animatorManager.enabled)
                    _animatorManager.Destroy();
                else
                    Destroy(this.gameObject);
            }
        }
    }

    // Draws a direction arrow in the scene view
    private void OnDrawGizmosSelected()
    {
        if (!this.isActiveAndEnabled || !_debugActuator) return;

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Vector3 position = transform.position;
        Vector3 dir = new Vector3(0, (int)_direction, 0);

        // Draw main arrow line
        Gizmos.DrawLine(position, position + dir);

        // Draw arrowhead
        Vector3 arrowTip = position + dir;
        Vector3 right = Quaternion.Euler(0, 0, 135) * dir * 0.25f;
        Vector3 left = Quaternion.Euler(0, 0, -135) * dir * 0.25f;
        Gizmos.DrawLine(arrowTip, arrowTip + right);
        Gizmos.DrawLine(arrowTip, arrowTip + left);
    }
}
