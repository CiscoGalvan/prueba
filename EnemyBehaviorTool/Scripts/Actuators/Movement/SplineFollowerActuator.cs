using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody2D))]
public class SplineFollowerActuator : MovementActuator
{


    // Define reaction when a collision happens

    [Tooltip("Speed at which the object moves along the spline.")]
    [SerializeField, HideInInspector] private float _speed =5;
    [SerializeField, HideInInspector] private float _goalSpeed;
    [SerializeField, HideInInspector] private float _interpolationTime = 0;

    private float _time;
    private EasingFunction.Function _easingFunc;




    // Option to decide if the object or the spline moves to match the closest point
    private enum TeleportToClosestPoint
    {
        Enemy = 0,
        Spline = 1
    }

    [Tooltip("Reference to the Spline Container that the object will follow.")]
    [SerializeField] private SplineContainer _splineContainer;


    [Tooltip("Defines whether the enemy teleports to the spline or moves the spline to the enemy.")]
    [SerializeField] private TeleportToClosestPoint _teleportToClosestPoint = TeleportToClosestPoint.Spline;

    private Rigidbody2D _rb;
    private float _distancePercentage = 0f;
    private float splinelength;
    private float _distanceBetweenPoints;

    public override void StartActuator()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Check if the spline is correctly assigned
        if (_splineContainer == null || _splineContainer.Spline == null)
        {
            Debug.LogError($"SplineContainer not assigned or is empty. in gameobject {name}");
            enabled = false;
            return;
        }

        // Calculate the total spline length
        splinelength = _splineContainer.CalculateLength();

        // Small step to get forward direction
        _distanceBetweenPoints = 0.05f;

        // Get the nearest point on the spline to this object
        _distancePercentage = GetClosestPointOnSpline(transform.position);

        // Handle teleporting based on selected mode
        switch (_teleportToClosestPoint)
        {
            case TeleportToClosestPoint.Enemy:
                // Move the object to the spline
                Vector3 currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
                transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
                break;

            case TeleportToClosestPoint.Spline:
                // Move the spline to align with the object without deforming it
                Vector3 closestPoint = _splineContainer.EvaluatePosition(_distancePercentage);
                Vector3 offset = transform.position - closestPoint;
                _splineContainer.transform.position += offset;
                break;
        }
        _easingFunc = EasingFunction.GetEasingFunction(_easingFunction);
    }

    public override void UpdateActuator()
    {
        if (_splineContainer != null)
        {
            if (_isAccelerated)
            {
                float t = (_time / _interpolationTime);
                float easedSpeed = _easingFunc(_speed, _goalSpeed, t);
                _distancePercentage += easedSpeed * Time.deltaTime / splinelength;
            }
            else
            {
                _distancePercentage += _speed * Time.deltaTime / splinelength;
            }
            // Advance along the spline based on speed and spline length
            

            // Evaluate current and next positions
            Vector3 currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
            Vector3 nextPosition = _splineContainer.EvaluatePosition(_distancePercentage + _distanceBetweenPoints);

            // Move the object using Rigidbody2D
            _rb.MovePosition(new Vector2(currentPosition.x, currentPosition.y));

            // Calculate direction and rotate accordingly
            Vector3 direction = (nextPosition - currentPosition);
            Vector3 directionNormalized = direction.normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rb.MoveRotation(angle);

            // If spline is closed and we reached the end, reset the percentage
            if (_distancePercentage > 1f)
            {
                _distancePercentage = 0f;
            }
            else if (_distancePercentage < 0.0f)
            {
                _distancePercentage = 1f;
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public override void DestroyActuator()
    {
        // Cleanup logic here if needed
    }

  
    // Finds the closest point on the spline to the given position.
    private float GetClosestPointOnSpline(Vector3 position)
    {
        float closestDistance = float.MaxValue;
        float closestT = 0f;
        float step = 0.01f;

        // Sample the spline to find the closest point
        for (float t = 0f; t <= 1f; t += step)
        {
            Vector3 splinePoint = _splineContainer.EvaluatePosition(t);
            float distance = Vector3.Distance(position, splinePoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }
}
