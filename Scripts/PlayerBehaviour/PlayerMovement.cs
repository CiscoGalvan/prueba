using UnityEngine;


// Dar créditos a Mix and Jam

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	private PlayerCollisionDetection coll;
	[HideInInspector]
	public Rigidbody2D rb;
	

	[Space]
	[Header("Stats")]
	[SerializeField]
	private float speed = 10;
	[SerializeField]
	private float jumpForce = 50;
	[SerializeField]
	private float slideSpeed = 5;

	private bool wallSlide;

	
	private bool groundTouch;

	private int side = 1;

	// Start is called before the first frame update
	void Start()
	{
		coll = GetComponent<PlayerCollisionDetection>();
		rb = GetComponent<Rigidbody2D>();
		
	}

	// Update is called once per frame
	void Update()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		float xRaw = Input.GetAxisRaw("Horizontal");
		float yRaw = Input.GetAxisRaw("Vertical");
		Vector2 dir = new Vector2(x, y);

		Walk(dir);
		

		if (coll.GetPlayerOnGround() )
		{
			PlayerBetterJumping _bestjump = GetComponent<PlayerBetterJumping>();
			if(_bestjump != null)
				_bestjump.enabled = true;
		}

		
		

		if (coll.GetPlayerOnWall() && !coll.GetPlayerOnGround())
		{
			if (x != 0)
			{
				wallSlide = true;
				WallSlide();
			}
		}

		if (!coll.GetPlayerOnWall() || coll.GetPlayerOnGround())
			wallSlide = false;

		if (Input.GetButtonDown("Jump"))
		{
			if (coll.GetPlayerOnGround())
				Jump(Vector2.up, false);
		}

		if (coll.GetPlayerOnGround() && !groundTouch)
		{
			groundTouch = true;
		}

		if (!coll.GetPlayerOnGround() && groundTouch)
		{
			groundTouch = false;
		}

		if (wallSlide)
			return;

		if (x > 0)
		{
			side = 1;
		}
		if (x < 0)
		{
			side = -1;
		}


	}

	private void WallSlide()
	{
	

		bool pushingWall = false;
		if ((rb.velocity.x > 0 && coll.GetPlayerOnRightWall()) || (rb.velocity.x < 0 && coll.GetPlayerOnLeftWall()))
		{
			pushingWall = true;
		}
		float push = pushingWall ? 0 : rb.velocity.x;

		rb.velocity = new Vector2(push, -slideSpeed);
	}

	private void Walk(Vector2 dir)
	{
		rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
	}

	private void Jump(Vector2 dir, bool wall)
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.velocity += dir * jumpForce;
	}
}
