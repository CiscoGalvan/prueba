using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using static MoveToAPointActuator;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveToAnObjectActuator : MovementActuator
{
    const float ALMOST_REACHED_ONE = 0.995f; // Threshold to consider the interpolation as complete

    // Struct representing data for each waypoint
    [System.Serializable]
    public struct WaypointData
    {
        public Transform waypoint;              // Reference to the waypoint's transform
        public float timeToReach;               // Time it takes to reach this point
        public bool isAccelerated;              // If the speed is not constant
        public bool shouldStop;
        [SerializeField, HideInInspector]
        public EasingFunction.Ease easingFunction;  // Type of easing used
    }

 
    [SerializeField, Tooltip("Configure the waypoint with time, acceleration and easing function")]
    private WaypointData _waypointData;

    private Rigidbody2D _rb;
    private bool _moving;

    private float _travelElapsedTime;
    private float _t; 

    private Vector2 _startInterpolationPosition;

    // Called when the actuator starts
    public override void StartActuator()
    {
        _rb = GetComponent<Rigidbody2D>();
        _travelElapsedTime = 0f;
        _t = 0f;
        _moving = true;

        // Validate the waypoint is set
        if (_waypointData.waypoint == null)
        {
            Debug.LogError($"MoveToAnObject error in {name}: No waypoint assigned.");

        }
        else
        {
            // Store current position as the starting point for interpolation
            _startInterpolationPosition = _rb.position;
        }

        // If an AnimatorManager exists, tell it to follow
        AnimatorManager _animatorManager = this.gameObject.GetComponent<AnimatorManager>();
        if (_animatorManager != null) _animatorManager.Follow();
    }

    // Called every frame if is in the actual State
    public override void UpdateActuator()
    {
        if (!_moving || _waypointData.waypoint == null)
            return;

        // Move toward the target if it's set
        if (_waypointData.waypoint.position != null)
            MoveTowardsTarget(_waypointData, _waypointData.waypoint.position);
        else
        {
            Debug.LogWarning("The MoveToAnObjectActuator goal Transform is null");
        }
    }

    // Handles the interpolation and movement logic toward the target
    private void MoveTowardsTarget(WaypointData waypoint, Vector2 targetPos)
    {
        _travelElapsedTime += Time.deltaTime;
        _t = _travelElapsedTime / waypoint.timeToReach;

        // Apply easing if enabled
        if (waypoint.isAccelerated)
        {
            _t = EasingFunction.GetEasingFunction(waypoint.easingFunction)(0, 1, _t);
            if (_t >= ALMOST_REACHED_ONE)
                _t = 1f;
        }

        // Interpolate the position between start and target
        Vector2 newPosition = Vector2.Lerp(_startInterpolationPosition, targetPos, _t);
        _rb.MovePosition(newPosition);

        // If movement completed stop movement
        if ((_t >= 1f && !waypoint.shouldStop) || waypoint.waypoint == null)
        {
            _moving = false;
        }
    }

    public override void DestroyActuator()
    {
        // No specific cleanup required currently
    }
    // Draw gizmos in the editor 
    private void OnDrawGizmos()
    {
        if (!_debugActuator) return;
        if (_waypointData.waypoint != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawSphere(_waypointData.waypoint.position, 0.2f);
        }
    }
}
