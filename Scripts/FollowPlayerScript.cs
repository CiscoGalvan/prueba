using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour
{
	public Transform target; // Referencia al jugador
	public float smoothSpeed = 0.125f; // Velocidad de suavizado
	public float xOffset = 2f; // Desplazamiento en X
	private float lastXPosition; // �ltima posici�n X de la c�mara

	void Start()
	{
		lastXPosition = transform.position.x;
	}

	void LateUpdate()
	{
		if (target == null)
		{
			Debug.LogWarning("No se ha asignado un objetivo a la c�mara");
			return;
		}

		// Determinar la nueva posici�n en X solo si el jugador avanza
		float targetX = target.position.x + xOffset;
		if (targetX > lastXPosition) // Solo avanza si el jugador se mueve hacia adelante
		{
			Vector3 desiredPosition = new Vector3(targetX, transform.position.y, transform.position.z);
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
			transform.position = smoothedPosition;
			lastXPosition = transform.position.x;
		}
	}
}
