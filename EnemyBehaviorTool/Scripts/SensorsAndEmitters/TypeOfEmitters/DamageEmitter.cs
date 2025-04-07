using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageEmitter;

[RequireComponent(typeof(Collider2D))]
public class DamageEmitter : MonoBehaviour
{
    // Enum representing different types of damage
    public enum DamageType
	{
        Instant,    // Damage is applied immediately
        Persistent, // Damage is applied over time
        Residual    // Damage is applied in multiple instances after the initial hit
    }
    [SerializeField,HideInInspector]
    private DamageType _damageType; // Stores the selected type of damage

    [SerializeField, HideInInspector]
    private bool _instaKill = false; // If true, the target is instantly killed

    [SerializeField, HideInInspector]
    private float _amountOfDamage = 0; // Damage amount for Instant, Persistent, and Residual types

    #region Persistent Damage Variables

    [SerializeField, HideInInspector]
    private float _damageCooldown = 1f; // Time interval between damage applications for Persistent damage

    #endregion
    #region Residual Damage Variables
    [SerializeField, HideInInspector]
    private int _numOfDamageApplication = 0; // Number of times residual damage is applied

    [SerializeField, HideInInspector]
    private float _residualDamageAmount = 0; // Damage amount per application for Residual damage

    #endregion


    [SerializeField,HideInInspector]
    private bool _destroyAfterDoingDamage = false;
    #region Getters and Setters

    // Returns the amount of residual damage
    public float GetResidualDamageAmount() => _residualDamageAmount;

    // Returns the damage type
    public DamageType GetDamageType() => _damageType;

    // Returns the base damage amount
    public float GetAmountOfDamage() => _amountOfDamage;

    // Returns the cooldown between persistent damage applications
    public float GetDamageCooldown() => _damageCooldown;

    // Returns the number of times residual damage is applied
    public int GetNumberOfResidualApplication() => _numOfDamageApplication;

    // Returns whether the damage is instant kill
    public bool GetInstaKill() => _instaKill;

	public bool GetDestroyAfterDoingDamage() => _destroyAfterDoingDamage;
	public void SetEmitting(bool newValue)
	{
		_isEmitting = newValue;
	}
	public bool GetEmitting() => _isEmitting;
	#endregion


	private bool _isEmitting = false;

	[SerializeField,HideInInspector]
	private bool _activeFromStart = false;

	private void Start()
	{
        if (_activeFromStart)
            _isEmitting = true;
	}

    public void SetActiveFromStart(bool newValue)
    {
        _activeFromStart = newValue;
    }
}
