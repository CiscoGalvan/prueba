using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class SensorStatePair
{
    public Sensors sensor;
    public State targetState;
}
public class State : MonoBehaviour
{
    
    [SerializeField]
    public List<Actuator> actuatorList = new List<Actuator>();

    private int _numElementsActuator = -1;

    //hashset con todos los sensores
    public HashSet<Sensors> sensorHashSet = new HashSet<Sensors>();
    //Lista con los sensores que pueden transicionara transicionar
    [SerializeField]
    private List<SensorStatePair> _sensorTransitions = new List<SensorStatePair>();

    private State _nextState = null;

   
	[SerializeField]
	private List<DamageEmitter> _damageEmittersInState = new List<DamageEmitter>();

	[SerializeField]
	[Tooltip("It determines whether the debug elements from the actuators and sensors included in this state are visible or not")]
	private bool _debugState = true;


	public void StartState()
    {
        foreach (var actuator in actuatorList)
        {
            if (actuator)
            {
                actuator.StartActuator();
                sensorHashSet.UnionWith(actuator.GetSensors());
            }
               
        }
        // Iniciar todos los sensores de _sensorTransitions
        foreach (var pair in _sensorTransitions)
        {
            if (pair.sensor != null)
            {
                sensorHashSet.Add(pair.sensor); // Opcional, si quieres que tambi�n est�n en sensorHashSet
            }
        }
        foreach (var sensor in sensorHashSet)
        {
            // This conditional is used to check when the list size is not zero and there is no sensor in it
            if(sensor)
                sensor.StartSensor();
        }
    
		foreach (var damageEmitter in _damageEmittersInState)
		{
            if (damageEmitter)
                damageEmitter.SetEmitting(true);
		}
		SubscribeToSensorEvents();
    }
	public void DestroyState()
	{
		
		foreach (var actuator in actuatorList)
		{
            actuator.DestroyActuator();
		}
        _nextState = null;
        UnsubscribeFromSensorEvents();
        foreach (var sensor in sensorHashSet)
        {
			if (sensor)
				sensor.StopSensor();
		}
		foreach (var damageEmitter in _damageEmittersInState)
		{
			if (damageEmitter)
				damageEmitter.SetEmitting(false);
		}
	}

	// Update is called once per frame
	public void UpdateState()
    {
        foreach (Actuator a in actuatorList)
        {
            a.UpdateActuator();
        }
        
    }
    public void AddActuator( Actuator act)
    {
        actuatorList.Add(act);
    }
    public void AddSensor(Sensors sen)
    {
        sensorHashSet.Add(sen);
    }
    public State CheckTransitions()
    {
        return _nextState;
    }
    private void SubscribeToSensorEvents()
    {
        foreach (var pair in _sensorTransitions)
        {
            if (pair.sensor != null && pair.targetState != null) //si los datos no son nulos
            {
                pair.sensor.onEventDetected += SensorTriggeredWrapper;
            }
        }
    }

    private void UnsubscribeFromSensorEvents()
    {
        foreach (var pair in _sensorTransitions)
        {
            if (pair.sensor != null)
            {
                pair.sensor.onEventDetected -= SensorTriggeredWrapper;
            }
        }
    }
    private void SensorTriggeredWrapper(Sensors sensor)
    {
        foreach (var pair in _sensorTransitions)
        {
            if (pair.sensor == sensor)
            {
                SensorTriggered(pair);
                break;
            }
        }
    }
    private void SensorTriggered(SensorStatePair pair)
    {
        _nextState = pair.targetState;
    }
    public void DeactivateAllActuators()
    {
        foreach (var actuator in actuatorList)
        {
            if (actuator != null)
            {
                actuator.enabled= false;
            }
        }
    }
    #region Editor
    private void OnValidate() //metodo que se llama cuandocambiamosalgo del editor
    {
        // queremos comprobar que no existan duplicados en actuadores y sensores si la lista se ha modificado
        if(actuatorList.Count != _numElementsActuator) _numElementsActuator = VerificarLista(actuatorList, "actuatorList");
		
		foreach (var actuator in actuatorList)
		{
			if (actuator != null)
			{
				actuator.SetDebug(_debugState);
                #if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(actuator);
                #endif
			}
		}
        foreach (var sensor in _sensorTransitions)
		{
			if (sensor.sensor != null)
			{
				sensor.sensor.SetDebug(_debugState);
                #if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(sensor.sensor);
                #endif
			}
		}
	}

    private int VerificarLista<T>(List<T> lista, string nombreLista)
    {
        if (lista == null || lista.Count <= 1)// No hay suficientes elementos para verificar
        {
            return -1;
        }
      
	//cogemos el ultimo elemento
	Type ultimoTipo = lista[lista.Count - 1]?.GetType();

        if (ultimoTipo == null) //si es nulo, entonces nada
        {
            return -1; 
        }

        for (int i = 0; i < lista.Count - 1; i++) //comprobamos si es igual a algun otro tipo de la lista
        {
            if (lista[i] != null && lista[i].GetType() == ultimoTipo)
            {
                Debug.LogWarning($"Se ha intentado agregar un segundo {nombreLista.TrimEnd('s')} del tipo {ultimoTipo.Name}");
                lista[lista.Count - 1] = default; // Dejarlo creado pero sin tipo
                 
                return lista.Count;
            }
        }
        return lista.Count;
    }
    #endregion

    public List<DamageEmitter> GetDamageEmitters() => _damageEmittersInState;
}
