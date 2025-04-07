using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementActuator : Actuator
{
	[Tooltip("Is the movement accelerated?")]
	[SerializeField]
	protected bool _isAccelerated = false;
	[SerializeField,HideInInspector]
	protected EasingFunction.Ease _easingFunction;
	protected float _accelerationValue;

	public abstract override void DestroyActuator();

	public abstract override void StartActuator();

	public abstract override void UpdateActuator();

	#region Setters and Getters
	public void SetEasingFunction(EasingFunction.Ease value)
	{
		_easingFunction = value;
	}
	public EasingFunction.Ease GetEasingFunctionValue()
	{
		return _easingFunction;
	}
	public bool IsMovementAccelerated()
	{
		return _isAccelerated;
	}
	public void SetAccelerationValue(float value)
	{
		_accelerationValue = value;
	}
	public float GetAccelerationValue()
	{
		return _accelerationValue;
	}
	#endregion
}
