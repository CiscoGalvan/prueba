using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Ensures that this component has both a Collider2D and a Rigidbody2D
[RequireComponent(typeof(Collider2D))]
public class CollisionSensor : Sensors
{
    [Tooltip("Layers that, in case of collision, will activate the sensor.")]
    [SerializeField]
    private LayerMask _layersToCollide =~0;

	// Boolean to track if a collision has occurred
	private bool _col;

    // Stores the latest collision event
    private Collision2D _collisionObject;



	[SerializeField, Min(0)]
	[Tooltip("Initial time the sensor will need to be active")]
	private float _startDetectingTime = 0f;
	private Timer _timer;
	private bool _timerFinished = false;

	// Handles the collision event when the object enters a collision
	private void OnCollisionEnter2D(Collision2D collision)
    {
		// Only process the collision if the sensor is active
		if (!_sensorActive || !_timerFinished) return;
		
		if ((_layersToCollide.value & (1 << collision.gameObject.layer)) == 0)
        {
            _col = true;
			Debug.Log("CAMBIO");
			Debug.Break();
            _collisionObject = collision;
            EventDetected(); // Call the event handler method
        }
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
		// Only process the collision if the sensor is active
		if (!_sensorActive || !_timerFinished) return;

		if ((_layersToCollide.value & (1 << collision.gameObject.layer)) != 0)
		{
			_col = true;
			_collisionObject = collision;
			EventDetected(); // Call the event handler method
		}
	}
	// Activates the sensor. Is the firts method call
	public override void StartSensor()
    {
        _sensorActive = true;
        _col = false;

		_timer = new Timer(_startDetectingTime);

		if (_startDetectingTime > 0)
		{
			_timer.Start();
			_timerFinished = false;
		}
		else
		{
			_timerFinished = true;
		}

	}
	public override void StopSensor()
	{
		_sensorActive = false;
	}

	private void Update()
	{
		if (!_sensorActive) return;
		if (!_timerFinished)
		{
			_timer.Update(Time.deltaTime);
		
			if (_timer.GetTimeRemaining() <= 0)
			{
				_timerFinished = true;
			}
			else
			{
				return;
			}
		}
	}
}
