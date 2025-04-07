using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalActuator : MovementActuator
{
    // Defines the layers with which this object can collide
    [SerializeField, HideInInspector]
    private LayerMask _layersToCollide = ~0;

    // Define direction
    private enum Direction
    {
        Left = -1,
        Right = 1
    }

    // Define reaction when a collision happens
    public enum OnCollisionReaction
    {
        None = 0,
        Bounce = 1,
        Destroy = 2
    }

    [SerializeField, HideInInspector] private float _speed;
    [SerializeField, HideInInspector] private float _goalSpeed;
    [SerializeField, HideInInspector] private float _interpolationTime = 0;

    // If true, movement will be applied only once (i.e., throw effect)
    [SerializeField, HideInInspector] private bool _throw;

    private float _initialSpeed = 0;

    [SerializeField, HideInInspector]
    private Direction _direction = Direction.Left;

    [SerializeField, HideInInspector]
    private OnCollisionReaction _onCollisionReaction = OnCollisionReaction.None;

    // Whether to follow the player’s horizontal position
    [SerializeField, HideInInspector]
    private bool _followPlayer = false;

    private float _time;
    private Rigidbody2D _rigidbody;
    private EasingFunction.Function _easingFunc;
    AnimatorManager _animatorManager;
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

        if (_throw) ApplyForce();

        // Find the player in the scene and calculate direction
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
                    _direction = direction.x > 0 ? Direction.Right : Direction.Left;
                }
            }
        }

        // Set initial animation based on direction
        if (_animatorManager != null && _animatorManager.enabled)
        {
            if (_direction == Direction.Left)
                _animatorManager.XLeftChangeAndFlip();
            else
                _animatorManager.XRightChangeAndFlip();
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
        if (!_throw) ApplyForce(); // Apply movement only if not thrown
    }

    // Applies velocity to the Rigidbody
    private void ApplyForce()
    {
        _time += Time.deltaTime;

        // Adjust direction based on player's current position
        if (_followPlayer && _playerReference != null)
        {
            float playerX = _playerReference.transform.position.x;
            float playerWidth = 0f;

            // Get player's collider width
            Collider2D playerCollider = _playerReference.GetComponent<Collider2D>();
            playerWidth = playerCollider.bounds.extents.x;

            float playerLeft = playerX - playerWidth;
            float playerRight = playerX + playerWidth;
            float enemyX = transform.position.x;

            if (enemyX > playerRight) _direction = Direction.Left;
            else if (enemyX < playerLeft) _direction = Direction.Right;
        }
        else if (_followPlayer && _playerReference == null)
        {
            Debug.LogWarning("Player reference was null, the actuator may not be precise");
        }

        int dirValue = (int)_direction;

        // Non-accelerated motion: constant velocity
        if (!_isAccelerated)
        {
            _rigidbody.velocity = new Vector2(_speed * dirValue, _rigidbody.velocity.y);
        }
        else
        {
            // Accelerated motion: use easing function for smooth acceleration
            float t = (_time / _interpolationTime);
            float easedSpeed = _easingFunc(_initialSpeed, _goalSpeed, t);

            if (t >= 1.0f)
            {
                _speed = _goalSpeed;
                _rigidbody.velocity = new Vector2(_goalSpeed * dirValue, _rigidbody.velocity.y);
            }
            else
            {
                _rigidbody.velocity = new Vector2(easedSpeed * dirValue, _rigidbody.velocity.y);
                _speed = easedSpeed;
            }
        }

        // Update animation according to movement direction
        if (_animatorManager != null && _animatorManager.enabled)
        {
            if (_direction == Direction.Left)
                _animatorManager.XLeftChangeAndFlip();
            else
                _animatorManager.XRightChangeAndFlip();
        }
    }

    // Handles collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collisions not in the specified layers or if no reaction is defined
        if ((_layersToCollide.value & (1 << collision.gameObject.layer)) == 0 || _onCollisionReaction == OnCollisionReaction.None) return;

        ContactPoint2D contact = collision.contacts[0];
        Vector2 normal = contact.normal;

        // Check if the collision is mostly horizontal
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        {
            if (_onCollisionReaction == OnCollisionReaction.Bounce)
            {
                // Flip direction if hit from the front
                bool hitFromCorrectSide = (_direction == Direction.Left && normal.x > 0) || (_direction == Direction.Right && normal.x < 0);
                if (hitFromCorrectSide)
                {
                    _direction = _direction == Direction.Left ? Direction.Right : Direction.Left;
                }
            }
            else if (_onCollisionReaction == OnCollisionReaction.Destroy)
            {
                // Destroy the object or play destruction animation
                if (_animatorManager != null || !_animatorManager.enabled) _animatorManager.Destroy();
                else Destroy(this.gameObject);
            }
        }
    }

    // Draws a gizmo in the editor indicating direction
    private void OnDrawGizmosSelected()
    {
        if (!this.isActiveAndEnabled || !_debugActuator) return;

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Vector3 position = transform.position;
        Vector3 direction = new Vector3((int)_direction, 0, 0);

        Gizmos.DrawLine(position, position + direction);
        Vector3 arrowTip = position + direction;
        Vector3 right = Quaternion.Euler(0, 0, 135) * direction * 0.25f;
        Vector3 left = Quaternion.Euler(0, 0, -135) * direction * 0.25f;
        Gizmos.DrawLine(arrowTip, arrowTip + right);
        Gizmos.DrawLine(arrowTip, arrowTip + left);
    }

    #region Setters and Getters 

    public void SetSpeed(float newValue) { _speed = newValue; }
    public float GetSpeed() { return _speed; }

    public void SetGoalSpeed(float newValue) { _goalSpeed = newValue; }
    public float GetGoalSpeed() { return _goalSpeed; }

    public void SetInterpolationTime(float newValue) { _interpolationTime = newValue; }
    public float GetInterpolationTime() { return _interpolationTime; }

    public bool GetBouncing() { return _onCollisionReaction == OnCollisionReaction.Bounce; }
    public bool GetDestroying() { return _onCollisionReaction == OnCollisionReaction.Destroy; }

    #endregion
}
