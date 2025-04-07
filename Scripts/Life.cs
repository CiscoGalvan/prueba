using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(DamageSensor))]
public class Life : MonoBehaviour
{
    public enum EntityType {Enemy, Player}
    [SerializeField, HideInInspector]
    private EntityType _entityType;


	[SerializeField]
	private float _initialLife = 5; // Initial life value
	[SerializeField]
	private float _maxLife = 5; // Maximun life value

    private float _currentLife;


    [SerializeField]
    private TextMeshProUGUI _lifeText; // UI text to display life value

    
    [SerializeField]
    private string _textName; // Prefix for life text display


    private bool _update = false;
	private float _amount = -1;

	private float _residualDamageAmount = 0;
	private DamageSensor _sensor;
	private DamageEmitter _damageEmitter;
	private float _actualDamageCooldown = -1;

	private float _damageCooldown = 0;
	private int _numOfDamage;
	private  void Awake()
	{
		// Validar que lifeText tenga un valor asignado
		if (_lifeText == null && _entityType == EntityType.Player)
		{
			Debug.LogWarning($"The TextMeshProUGUI reference in {gameObject.name} is not assigned. Please assign it in the inspector.", this);
			enabled = false; // Desactiva el script si no está configurado correctamente
		}
	}
	private void Start()
	{
		_currentLife = _initialLife;
		_sensor = GetComponent<DamageSensor>();
		_sensor.onEventDetected += ReceiveDamageEmitter;
		_actualDamageCooldown = 0f;
		_numOfDamage = 0;
		UpdateLifeText();
	}

	private void Update()
	{
		if (_update)
		{
			switch (_damageEmitter.GetDamageType())
			{
				case DamageEmitter.DamageType.Persistent:
					{
						_actualDamageCooldown += Time.deltaTime;
						if (_actualDamageCooldown > _damageCooldown)
						{
							DecreaseLife(_amount);
							_actualDamageCooldown = 0;
						}
					}
					break;
				case DamageEmitter.DamageType.Residual:
					{
						if (_numOfDamage > 0)
						{
							_actualDamageCooldown += Time.deltaTime;
							if (_actualDamageCooldown > _damageCooldown)
							{		
								_numOfDamage--;
								_actualDamageCooldown = 0;
								DecreaseLife(_residualDamageAmount);
							}
						}
					}
					break;
			}
		}

		if(_currentLife <= 0)
		{
			AnimatorManager _animatorManager = this.GetComponent<AnimatorManager>();

            if (_animatorManager == null || !_animatorManager.enabled)
			{
                Destroy(this.gameObject);
            }			
			else
			{
				_animatorManager.Destroy();
			}
		}
	}
	
	private void OnDestroy()
	{
		if(_sensor != null)
		_sensor.onEventDetected -= ReceiveDamageEmitter;
	}

	private void ReceiveDamageEmitter(Sensors damageSensor)
	{
		//El residual esta mal, ya que al separarse no hace el daño residual.
		_damageEmitter = (damageSensor as DamageSensor).GetDamageEmitter();
		if (_damageEmitter != null)
		{
			if (_sensor.HasCollisionOccurred())
			{
				switch (_damageEmitter.GetDamageType())
				{
					case DamageEmitter.DamageType.Instant:
						if (_damageEmitter.GetInstaKill())
							InstantKill();
						else
							DecreaseLife(_damageEmitter.GetAmountOfDamage());
						break;
					case DamageEmitter.DamageType.Persistent:
						{
							_amount = _damageEmitter.GetAmountOfDamage();
							DecreaseLife(_amount);
							_update = true;
							_damageCooldown = _damageEmitter.GetDamageCooldown();
						}
						break;
					case DamageEmitter.DamageType.Residual:
						{
							_amount = _damageEmitter.GetAmountOfDamage();
							_update = true;
							_residualDamageAmount = _damageEmitter.GetResidualDamageAmount();
							_damageCooldown = _damageEmitter.GetDamageCooldown();

							// El numero de aplicaciones se iguala o es +=
							// Imagina que te envenenan y estas recibiendo danho residual y te vuelve a golpear el mismo enemigo, reinicias las veces que te hace daño, las sumas o al gusto del disenahor?
							_numOfDamage = _damageEmitter.GetNumberOfResidualApplication();
							DecreaseLife(_amount);
							_actualDamageCooldown = 0;
						}
						break;
					default:
						break;
				}
			}
			else
			{
				// Reinciar variables.
				if(_numOfDamage <= 0)
				{
					_update = false;
					_actualDamageCooldown = 0;
				}
				
			}

		}

	}
	private void DecreaseLife(float num)
	{
		
		AnimatorManager _animatorManager = this.GetComponent<AnimatorManager>();
		_animatorManager?.Damage();
		_currentLife -= num;
		if (_currentLife < 0)
		{
            _currentLife= 0;

        }
		UpdateLifeText();
		
	}
	private void InstantKill()
	{
		_currentLife = 0;
		UpdateLifeText();
	}
    public void IncreaseLife(float num)
	{
		_currentLife += num;
        if (_currentLife > _maxLife)
        {
            _currentLife = _maxLife;

        }
        UpdateLifeText();
	}
    public void SetLife(float num)
	{
		_currentLife = num;
        if (_currentLife > _maxLife)
        {
            _currentLife = _maxLife;

        }
        UpdateLifeText();
	}
	public void ResetLife()
	{
		_currentLife = _initialLife;
		UpdateLifeText();
	}
    public float GetInitialLife()
    {
        return _initialLife;
    }
    public void SetInitialLife(float value)
    {
        _initialLife = value;
    }
    private void UpdateLifeText()
	{
		if (_lifeText != null && _entityType == EntityType.Player)
		{
			_lifeText.text = _textName + _currentLife;
		}
	}
    public bool IsLifeLessThan(int value)
	{
		return _currentLife < value;
	}

    public string GetTextName()
    {
        return _textName;
    }

    public void SetTextName(string value)
    {
        _textName = value;
    }

    public TextMeshProUGUI GetLifeText()
    {
        return _lifeText;
    }

    public void SetLifeText(TextMeshProUGUI value)
    {
        _lifeText = value;
    }
    public EntityType GetEntityType()
    {
        return _entityType;
    }

    public void SetEntityType(EntityType value)
    {
        _entityType = value;
    }
}
