using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    [Header("FSM Configuration")]
    [Tooltip("Defines the initial state of the FSM.")]
    [SerializeField]
    private State initialState;
    
    private State _currentState; // Stores the current active state

    void Awake()
    {
        // Set the initial state and execute its start logic
        _currentState = initialState;
		if (_currentState != null)
        	_currentState.StartState();
    }

    void Update()
    {
        // Executes the logic of the current state in each frame
        if(_currentState != null ) 
            _currentState.UpdateState();
    }
  
     private void OnDestroy()
    {
		// Executes the exit actions of the last state when the FSM is destroyed
		if (_currentState != null)
			_currentState.DestroyState();
    }

    void LateUpdate()
    {
        // Checks for state transitions after all updates are processed
        if (_currentState == null) return;

		State newState = _currentState.CheckTransitions();
        if (newState != null && newState != _currentState)
        {
            ChangeState(newState);
        }
    }

    /// <summary>
    /// Changes the current state to a new state.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    private void ChangeState(State newState)
    {
        if (_currentState == null) return;
        // Execute exit actions of the current state
        _currentState.DestroyState();
        // Set the new state and execute its start logic
        _currentState = newState;
        AnimatorManager _animatorManager = this.gameObject.GetComponent<AnimatorManager>(); 
        if (_animatorManager != null)
        {
            _animatorManager.ChangeState();
        }
        _currentState.StartState();
    }
    public void DeactivateCurrentStateActuators()
    {
        if (_currentState != null)
        {
            _currentState.DeactivateAllActuators();
        }
        Rigidbody2D rb = this.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

	private void OnValidate() 
	{
		if(initialState != null)
        {
            var damageEmitters = initialState.GetDamageEmitters();
            foreach(var damageEmitter in damageEmitters)
            {
                damageEmitter.SetActiveFromStart(true);
            }

		}
	
	}

}


