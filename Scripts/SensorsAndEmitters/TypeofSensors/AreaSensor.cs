using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaSensor : Sensors
{
	[SerializeField]
	private GameObject _target; 
	
	private Collider2D _detectionZone;

	
	[SerializeField, Min(0)]
	[Tooltip("Initial time the sensor will need to be active")]
	private float _startDetectingTime = 0f;
	private Timer _timer;
	private bool _timerFinished = false;
	public override void StartSensor()
	{
		_sensorActive= true; 
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

		_detectionZone = gameObject.GetComponent<Collider2D>();
		_detectionZone.isTrigger = true;
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!_sensorActive ||!_timerFinished) return;
		if (collision.gameObject == _target)
		{
			EventDetected();
		}
	}
	
	// We have to check if the sensor may be triggered just when the timer finished.
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!_sensorActive || !_timerFinished) return;
		if (collision.gameObject == _target)
		{
			EventDetected();
		}
	}
}
