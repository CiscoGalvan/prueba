using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDistanceAttack : MonoBehaviour
{
	[SerializeField]
	private GameObject _bulletPrefab;

	[SerializeField]
	private float _shootingCooldown = 0.5f; 

	private float _nextShootTime = 0f;

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && Time.time >= _nextShootTime)
		{
			_nextShootTime = Time.time + _shootingCooldown;

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0f; 
			
			Vector2 direction = (mouseWorldPos - transform.position).normalized;
			
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

			GameObject bullet = Instantiate(_bulletPrefab, transform.position, rotation);
			var directionalMovement = bullet.GetComponent<Directional_Actuator>();
			directionalMovement.SetAngle(angle);
		}
	}
}
