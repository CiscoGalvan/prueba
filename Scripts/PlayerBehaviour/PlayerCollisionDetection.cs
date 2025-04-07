using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Dar créditos a Mix and Jam

public class PlayerCollisionDetection : MonoBehaviour
{
	[Header("Layers")]
	[Tooltip("ESCIBID ALGO AQUI")]
	public LayerMask _detectionLayers;

	[SerializeField]
	private bool _debugBoxes = false;
	
	private bool onGround;
	private bool onWall;
	private bool onRightWall;
	private bool onLeftWall;
	private int wallSide;

	[Header("Offsets")]
	[Space]
	public Vector2 bottomOffset, rightOffset, leftOffset;


	[Header("Sizes")]
	[Space]
	public Vector2 bottomSize, rightSize, leftSize;

	// Update is called once per frame
	void Update()
	{
		onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0f, _detectionLayers);
		onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0f, _detectionLayers)
			|| Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0f, _detectionLayers);

		onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0f, _detectionLayers);
		onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0f, _detectionLayers);

		wallSide = onRightWall ? -1 : 1;
	}

	void OnDrawGizmos()
	{
		if (!_debugBoxes) return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, bottomSize);
		Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, rightSize);
		Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, leftSize);
	}


	public bool GetPlayerOnGround() => onGround;
	public bool GetPlayerOnWall() => onWall;
	public bool GetPlayerOnRightWall() => onRightWall;
	public bool GetPlayerOnLeftWall() => onLeftWall;
}
