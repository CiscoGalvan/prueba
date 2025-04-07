using System;
using UnityEditor;
using UnityEngine;
public class Timer
{
    // Tiempo requerido para que se active la detección
    private float _detectionTime;
    private float _timer;
    private bool _isRunning;

    public Timer(float detectionTime)
    {
        _detectionTime = Mathf.Max(0, detectionTime);
        _isRunning = false;
    }

    // Inicia el temporizador
    public void Start()
    {
        _timer = 0f;
        _isRunning = true;
    }

    // Reinicia el temporizador
    public void Reset()
    {
        _timer = 0f;
        _isRunning = false;
    }

    // Actualiza el temporizador, debe llamarse en un Update externo
    public void Update(float deltaTime)
    {
        if (!_isRunning)
            return;

        _timer += deltaTime;

        if (_timer >= _detectionTime)
        {
            _isRunning = false;
           
           
        }
    }

    // Obtiene el tiempo restante
    public float GetTimeRemaining()
    {
        return Mathf.Max(0, _detectionTime - _timer);
    }
}

