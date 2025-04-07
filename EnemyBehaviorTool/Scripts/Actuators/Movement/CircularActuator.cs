using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// Ensure that this component always has a Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class CircularActuator : MovementActuator
{
	// Angular speed in degrees per second (it will be converted to radians).
	[SerializeField, HideInInspector]
	private float _angularSpeed;

	// Center of our movement.
	[SerializeField]
	private Transform _rotationPointPosition;

	// Max Angle described by the movement.
	[SerializeField, Range(0f, 360f)]
	private float _maxAngle = 360f;

	// Angular acceleration in case the movement is accelerated.
    [SerializeField, HideInInspector]
    private float _angularAcceleration = 0f;

	// If true the object will rotate with it's movement.
    [SerializeField]
    private bool _canRotate = false;

	//Reference to he object's Rigidbody.
    private Rigidbody2D _rigidbody;

	// Current angular speed applicated to the object.
	// It's a SerializedField because we may want it to change in case we change the _angularSpeed variable.
	[SerializeField]
	private float _currentAngularSpeed;
	
	// Current angle of movement [0,360].
	private float _currentAngle;

	// Boolean to determinate when it's the movement backwards.
	private bool _reversing = false;

	// Actual radius of the movement, it is the distance between the _rotationPointPosition and the CircularActuator's object current position.
	private float _radius = 0;
	
	// It will be used to calculate pendulum-motion properties.
	private float _initAngle;

	// Variable used in case the movement is accelerated to interpolate between an initial velocity and our goal velocity.
	private float _initAngularSpeed;

	// Velocity we want our object to reach.
	[SerializeField]
	private float _goalAngularSpeed;

	// Variable to store passing time.
	private float _time;

	// Time it will take the object to reach it's goal speed.
	[SerializeField, HideInInspector]
	private float _interpolationTime;

	// Will be used to debugging reasons
	private Vector3 _startingPosition;

	// Reference to the AnimatorManager
    AnimatorManager _animatorManager;

	// Boolean that determines if the movement is oriented to the player's position
	[SerializeField, HideInInspector]
	private bool _pointPlayer= false;

	// Player reference
	private GameObject _playerReference;

	// For debugging reasons
	private bool _isActive = false;


	// The actuator is started and everything is set up.
	public override void StartActuator()
	{
		_isActive = true;
		_animatorManager = this.gameObject.GetComponent<AnimatorManager>();
		_startingPosition = transform.position;
		_rigidbody = GetComponent<Rigidbody2D>();
       
		if (_rotationPointPosition == null)
		{
			Debug.LogWarning("No rotation point assigned. The actuator won't update");
		}
		else
		{
			_radius = Vector3.Distance(_rotationPointPosition.position, transform.position);
			Vector2 dir = transform.position - _rotationPointPosition.position;
			_initAngle = Mathf.Atan2(dir.y, dir.x);
			_currentAngle = _initAngle;
			_currentAngularSpeed = _angularSpeed * Mathf.Deg2Rad;
			_initAngularSpeed = _currentAngularSpeed;
			_time = 0;
			if (_pointPlayer)
			{
				var objectsWithPlayerTagArray = GameObject.FindGameObjectsWithTag("Player");
				if (objectsWithPlayerTagArray.Length == 0)
				{
					Debug.LogWarning("There was no object with Player tag, the proyectile angle won't be controlled");
				}
				else
				{
					_playerReference = objectsWithPlayerTagArray[0];
				}

			}
		}
	}

	// Updates the actuator behavior each frame
	public override void UpdateActuator()
	{
		// If rotation point is not set, exit the method
		if (_rotationPointPosition == null) return;

		// Increment time by the frame time
		_time += Time.deltaTime;

		// If the object should follow the player, call FollowPlayerMovement
		if (_pointPlayer)
			FollowPlayerMovement();
		else
			CircularMovement();
	}

	// In case we want the object to follow the player's position.
	private void FollowPlayerMovement()
	{
		// Ensure the player reference is not null
		if (_playerReference != null)
		{
			// Calculate direction vector from rotation point to player position
			Vector2 direction = _playerReference.transform.position - _rotationPointPosition.position;
			direction.Normalize();  // Normalize the direction vector

			// Calculate the angle to the player
			_currentAngle = Mathf.Atan2(direction.y, direction.x);

			// Calculate the offset based on the radius
			Vector2 offset = direction * _radius;
			Vector2 newPosition = (Vector2)_rotationPointPosition.position + offset;

			// Move the rigidbody to the new position
			_rigidbody.MovePosition(newPosition);

			// If rotation is allowed, apply the rotation
			if (_canRotate)
			{
				transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle * Mathf.Rad2Deg);
			}

			// If animator manager exists, update the rotation speed
			if (_animatorManager != null)
			{
				_animatorManager.ChangeSpeedRotation(_currentAngle * Mathf.Rad2Deg);
			}
		}
	}

	// In case we want the object to perform Circular/Pendulum-motion Movement
	private void CircularMovement()
	{
		// If acceleration is enabled, interpolate angular speed
		if (_isAccelerated)
		{
			float t = Mathf.Clamp01(_time / _interpolationTime);  // Clamp time to [0,1] range
																  // Apply easing function to calculate the new angular speed
			_currentAngularSpeed = EasingFunction.GetEasingFunction(_easingFunction)(_initAngularSpeed, _goalAngularSpeed, t);

			// If time has reached its limit, set the angular speed to the goal
			if (t >= 1.0f)
			{
				_currentAngularSpeed = _goalAngularSpeed;
			}
		}

		// Convert the maximum angle to radians
		float maxAngleRad = _maxAngle * Mathf.Deg2Rad;

		// If the max angle is less than 360 degrees, perform pendulum movement
		if (_maxAngle < 360f)
		{
			float upperLimit = _initAngle + (maxAngleRad / 2f);  // Calculate upper angle limit
			float lowerLimit = _initAngle - (maxAngleRad / 2f);  // Calculate lower angle limit

			// If not reversing, move in a positive direction
			if (!_reversing)
			{
				_currentAngle += _currentAngularSpeed * Time.deltaTime;  // Increment the current angle

				// If the angle exceeds the upper limit, reverse the direction
				if (_currentAngle > upperLimit)
				{
					_currentAngle = upperLimit;
					_reversing = true;
				}
			}
			else  // If reversing, move in the negative direction
			{
				_currentAngle -= _currentAngularSpeed * Time.deltaTime;  // Decrement the current angle

				// If the angle falls below the lower limit, stop reversing
				if (_currentAngle < lowerLimit)
				{
					_currentAngle = lowerLimit;
					_reversing = false;
				}
			}
		}
		else // If performing a full circular motion
		{
			_currentAngle += _currentAngularSpeed * Time.deltaTime;  // Increment angle
			_currentAngle = Mathf.Repeat(_currentAngle, 2 * Mathf.PI);  // Keep angle within 0 to 2π radians
		}

		// Calculate tangential speed based on the angular speed and radius
		float tangentialSpeed = _currentAngularSpeed * _radius;

		// If reversing, invert the tangential speed
		if (_maxAngle < 360f && _reversing)
		{
			tangentialSpeed *= -1;
		}

		// Calculate the tangential velocity vector
		Vector2 tangentialVelocity = new Vector2(
			-Mathf.Sin(_currentAngle) * tangentialSpeed,
			Mathf.Cos(_currentAngle) * tangentialSpeed
		);

		// Set the rigidbody's velocity to the tangential velocity
		_rigidbody.velocity = tangentialVelocity;

		// Correct the position to maintain constant radius from the rotation point
		Vector3 expectedPosition = _rotationPointPosition.position + new Vector3(Mathf.Cos(_currentAngle) * _radius, Mathf.Sin(_currentAngle) * _radius, 0f);
		_rigidbody.MovePosition(expectedPosition);

		// If rotation is allowed, apply the calculated rotation
		if (_canRotate)
			transform.rotation = Quaternion.Euler(0f, 0f, _currentAngle * Mathf.Rad2Deg);

		// If animator manager exists, update the rotation speed in the animation
		if (_animatorManager != null)
			_animatorManager.ChangeSpeedRotation(_currentAngle * Mathf.Rad2Deg);
	}

	// Draws gizmos in the scene view for debugging purposes when the object is selected
	private void OnDrawGizmosSelected()
	{
		// If debugging is off or rotation point is not set, exit the method
		if (!_debugActuator || _rotationPointPosition == null) return;

		// If the max angle is 360 degrees, draw a full circle (wire sphere)
		if (_maxAngle == 360f)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f); // Orange color for the gizmo

			// If there is no rotation point, draw the sphere at the current position
			if (_rotationPointPosition == null)
			{
				Gizmos.DrawWireSphere(transform.position, _radius);
			}
			else
			{
				// Calculate the radius as the distance from the rotation point to the object
				_radius = Vector3.Distance(_rotationPointPosition.position, transform.position);
				// Draw a wire sphere at the rotation point with the calculated radius
				Gizmos.DrawWireSphere(_rotationPointPosition.position, _radius);
			}
		}
		else // If max angle is less than 360 degrees, draw an arc instead of a full circle
		{
			Vector3 direction;

			// If the actuator is not active, use the current position
			if (!_isActive)
			{
				direction = transform.position - _rotationPointPosition.position;
				_radius = Vector3.Distance(_rotationPointPosition.position, transform.position);
			}
			else // If the actuator is active, use the starting position
			{
				direction = _startingPosition - _rotationPointPosition.position;
			}

			// Calculate the initial angle in degrees from the direction vector
			float initialAngleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			// Calculate half of the maximum angle range
			float halfAngleRange = _maxAngle / 2f;

			// Set gizmo color to orange for the arc
			Handles.color = new Color(1f, 0.5f, 0f);

			// Draw the first arc (right side of the rotation range)
			Handles.DrawWireArc(_rotationPointPosition.position, Vector3.forward,
				Quaternion.Euler(0, 0, initialAngleDegrees + halfAngleRange) * Vector3.right,
				-halfAngleRange, _radius);

			// Set gizmo color to purple for the starting angle
			Handles.color = new Color(0.5f, 0f, 0.5f);
			// Draw the second arc (left side of the rotation range)
			Handles.DrawWireArc(_rotationPointPosition.position, Vector3.forward,
				Quaternion.Euler(0, 0, initialAngleDegrees) * Vector3.right,
				-halfAngleRange, _radius);
		}
	}

	public override void DestroyActuator()
	{
		_isActive = false;
	}
}
