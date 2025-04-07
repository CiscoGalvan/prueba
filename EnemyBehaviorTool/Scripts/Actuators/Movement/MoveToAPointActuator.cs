using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MoveToAPointActuator;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveToAPointActuator : MovementActuator
{
    const float ALMOST_REACHED_ONE = 0.999f; // Threshold to consider the interpolation as complete

    // Struct representing data for each waypoint
    [System.Serializable]
    public struct WaypointData
    {
        public Transform waypoint;              // Reference to the waypoint's transform
        public float timeToReach;               // Time it takes to reach this point
        public bool isAccelerated;              // If the speed is not constant
        public bool shouldStop;                 
        [HideInInspector]
        public float stopDuration;              // Time to stop at this waypoint
        [SerializeField, HideInInspector]
        public EasingFunction.Ease easingFunction; // Type of easing used
    }

    // Movement types
    public enum UsageWay
    {
        Waypoint = 0,        // Follow defined waypoints
        RandomArea = 1       // Move within a random area
    };

    #region Shared Waypoint Settings (used for uniform configuration)
    [SerializeField, HideInInspector]
    private float _timeToReachForAllWaypoints;
    [SerializeField, HideInInspector]
    private bool _areAccelerated;
    [SerializeField, HideInInspector]
    private bool _shouldThemStop;
    [SerializeField, HideInInspector]
    private float _stopTime;
    [SerializeField, HideInInspector]
    private EasingFunction.Ease _easingFunctionForAllWaypoints;
    #endregion

    [SerializeField]
    private UsageWay _usageWay;

    [SerializeField]
    private List<WaypointData> _waypointsData = new List<WaypointData>();

    [SerializeField]
    private Collider2D _randomArea;

    private Rigidbody2D _rb;
    private bool _moving;

    private float _travelElapsedTime;
    private float _stopElapsedTime;

    private Vector2 _startInterpolationPosition;
    private float _t;
    private int _currentWaypointIndex;

    [SerializeField, Tooltip("If true, the waypoint path will loop: after reaching the last waypoint, it will return to the first one")]
    private bool _isACicle = false;                         // Should the path loop
    [SerializeField, HideInInspector]
    private bool _allWaypointsHaveTheSameData = false;      // Do all waypoints share settings
    [SerializeField, HideInInspector]
    private bool _ciclicWaypointAdded;

    private bool _randomPointReached = true;
    private Vector2 _currentRandomPoint;
    [SerializeField]
    private float _timeBetweenRandomPoints;
    private List<Vector2> _cachedWaypointPositions = new List<Vector2>();
    AnimatorManager _animatorManager;
    Vector2 _previousPosition;

    // Called when the actuator starts
    public override void StartActuator()
    {
        _rb = GetComponent<Rigidbody2D>();
        _travelElapsedTime = 0f;
        _stopElapsedTime = 0f;
        _t = 0f;
        _currentWaypointIndex = 0;

        // Check if waypoints are properly set
        if (_usageWay == UsageWay.Waypoint && (_waypointsData == null || _waypointsData.Count == 0))
        {
            Debug.LogError($"MoveToAPoint_Actuator error in {name}: No waypoints were set.");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
        else
        {
            if (_usageWay == UsageWay.Waypoint)
            {
                // Cache positions of waypoints to improve performance
                foreach (var waypoint in _waypointsData)
                {
                    if (waypoint.waypoint != null)
                        _cachedWaypointPositions.Add(waypoint.waypoint.position);
                    else
                        _cachedWaypointPositions.Add(Vector2.zero);
                }
            }
            _startInterpolationPosition = _rb.position;
            _moving = true;
        }

        // Check for random area setup
        if (_usageWay == UsageWay.RandomArea)
        {
            if (_randomArea == null)
            {
                Debug.LogError($"No area was attached in object {name}");
                return;
            }
            _randomArea.isTrigger = true;
        }

        _animatorManager = this.GetComponent<AnimatorManager>();
        _previousPosition = _rb.position;
    }

    // Called every frame if is in the actual State
    public override void UpdateActuator()
    {
        if (!_moving && _isACicle)
        {
            _moving = true;
            _currentWaypointIndex = 0;
        }
        if (!_moving) return;

        // Handle movement type
        switch (_usageWay)
        {
            case UsageWay.Waypoint:
                MoveAlongWaypoints();
                break;
            case UsageWay.RandomArea:
                MoveToRandomPoint();
                break;
        }
    }

    // Generate a new random point inside the given area and move towards it
    private void MoveToRandomPoint()
    {
        if (_randomArea == null)
        {
            Debug.LogError("Random area not set!");
            return;
        }

        if (_randomPointReached)
        {
            _currentRandomPoint = new Vector2(
                Random.Range(_randomArea.bounds.min.x, _randomArea.bounds.max.x),
                Random.Range(_randomArea.bounds.min.y, _randomArea.bounds.max.y)
            );
            _randomPointReached = false;
        }

        // Create a temporary waypoint data to use with easing and timing
        WaypointData randomWaypoint = new WaypointData
        {
            waypoint = null,
            timeToReach = _timeBetweenRandomPoints,
            isAccelerated = false,
            shouldStop = false,
            stopDuration = Random.Range(0.5f, 2f),
            easingFunction = EasingFunction.Ease.EaseInOutQuad
        };

        MoveTowardsTarget(randomWaypoint, _currentRandomPoint);
    }

    // Move towards the next waypoint in the list
    private void MoveAlongWaypoints()
    {
        if (_currentWaypointIndex >= _cachedWaypointPositions.Count)
            return;

        Vector2 targetPos = _cachedWaypointPositions[_currentWaypointIndex];
        WaypointData currentWaypoint = _waypointsData[_currentWaypointIndex];
        MoveTowardsTarget(currentWaypoint, targetPos);
    }

    // Handles actual movement, easing, and stopping at a target position
    private void MoveTowardsTarget(WaypointData waypoint, Vector2 targetPos)
    {
        // If stopped at a waypoint, wait the required time
        if (_t >= 1f && waypoint.shouldStop)
        {
            _stopElapsedTime += Time.deltaTime;
            if (_stopElapsedTime >= waypoint.stopDuration)
            {
                AdvanceToNextWaypoint(targetPos);
            }
            return;
        }

        // Update travel time and calculate normalized interpolation t value
        _travelElapsedTime += Time.deltaTime;
        _t = _travelElapsedTime / waypoint.timeToReach;

        // Apply easing if enabled
        if (waypoint.isAccelerated)
        {
            _t = EasingFunction.GetEasingFunction(waypoint.easingFunction)(0, 1, _t);
            if (_t >= ALMOST_REACHED_ONE)
                _t = 1f;
        }

        Vector2 newPosition = Vector2.Lerp(_startInterpolationPosition, targetPos, _t);
        _rb.MovePosition(newPosition);

        // Animate based on movement direction
        if (newPosition.x < _previousPosition.x)
        {
            _animatorManager.XLeftChangeAndFlip();
        }
        else if (newPosition.x > _previousPosition.x)
        {
            _animatorManager.XRightChangeAndFlip();
        }

        _previousPosition = newPosition;

        // If the point has been reached, go to the next one
        if (_t >= 1f && !waypoint.shouldStop)
        {
            AdvanceToNextWaypoint(targetPos);
        }
    }

    // Prepares movement to the next point or finishes if the path is complete
    private void AdvanceToNextWaypoint(Vector2 reachedPos)
    {
        _travelElapsedTime = 0f;
        _stopElapsedTime = 0f;
        _t = 0f;
        _startInterpolationPosition = reachedPos;
        _currentWaypointIndex++;

        if (_usageWay == UsageWay.RandomArea)
        {
            _randomPointReached = true;
        }
        else if (_currentWaypointIndex >= _waypointsData.Count)
        {
            if (_isACicle)
            {
                _currentWaypointIndex = 0;
            }
            else
            {
                _moving = false;
                _rb.velocity = Vector2.zero;
            }
        }
    }

    // Called when the actuator is destroyed
    public override void DestroyActuator()
    {
        // No specific cleanup required currently
    }

    // Draw gizmos in the editor 
    private void OnDrawGizmos()
    {
        if (!_debugActuator) return;
        Gizmos.color = new Color(1f, 0.5f, 0f);

        switch (_usageWay)
        {
            case UsageWay.RandomArea:
                Gizmos.DrawSphere(_currentRandomPoint, 0.2f);
                break;

            case UsageWay.Waypoint:
                if (_waypointsData.Count > 0 && _currentWaypointIndex < _waypointsData.Count)
                {
                    Transform currentWaypoint = _waypointsData[_currentWaypointIndex].waypoint;

                    if (currentWaypoint != null)
                    {
                        Gizmos.DrawSphere(currentWaypoint.position, 0.2f);
                    }
                }
                break;
        }
    }
}
