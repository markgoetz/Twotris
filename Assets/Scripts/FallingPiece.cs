using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputManager))]
public class FallingPiece : MonoBehaviour {
	static private int[,,] block_types = {
		{{0, 0},{0,1},{1,0},{ 1,1}}, // square
	    {{0,-1},{0,0},{0,1},{ 0,2}}, // line
		{{0,-1},{0,0},{0,1},{ 1,1}}, // L
		{{0,-1},{0,0},{0,1},{-1,1}}, // other L		
		{{0,-1},{0,0},{0,1},{ 1,0}}, // T	
		{{0,-1},{0,0},{1,0},{ 1,1}}, // Z		
		{{1,-1},{1,0},{0,0},{ 0,1}} // other Z					
	};

	//public float movementTime;
	public float fastFallTime = .06f;
	public GameObject fallingBlock;
	public GameObject ghostPiece;
	public PlayerColorList playerColorList;
	public DifficultyManager dm;

	private int size = 1;
	private bool can_move = true;
	private Board board;
	private bool falling = true;

	private InputManager input;
	private FallType fall_type;
	
	private GameObject[] blocks;
	
	void Awake() {
		//size = GetComponent<BoxCollider>().size;
		generateBlocks();
		input = GetComponent<InputManager>();	
		
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();

		
		// HACK: Probably a better way to get all the children.
		blocks = new GameObject[transform.childCount];
		int count = 0;
		foreach (Transform block in transform) {
			blocks[count++] = block.gameObject;
		}
		
		generateGhost();
		
		fall_type = FallType.Normal;
		
		StartCoroutine ("fall");
	}
	
	private Vector3 getMovement() {
		if (!can_move) return Vector3.zero;
	
		return new Vector3(input.Move, 0, 0);
	}
	
	private void handleKeyInput() {			
		if (input.Rotation != 0) {
			tryToRotate (input.Rotation * 90);
		}
		
		if (input.FastFall) {
			fall_type = FallType.Fast;
		}
		else if (input.InstantFall) {
			fall_type = FallType.Instant;
		}
		else {
			fall_type = FallType.Normal;
		}
	}
	
	void Update () {
		handleKeyInput();
		Vector3 movement = getMovement();
		
		if (movement.x != 0) {

			foreach (GameObject block in blocks) {
				if (board.Collide(block.transform.position + movement, PlayerNumber) == BlockCollision.Solid) {
					movement = Vector2.zero;
				}
			}
			
			if (movement.magnitude > 0) {
				// Don't use transform.translate here as it will totally mess with the rotation.
				transform.position = transform.position + movement;
				//StartCoroutine ("LerpMovement", movement);
			}
		}
	}
	
	/*private IEnumerator LerpMovement(Vector2 movement) {
		if (movement.magnitude == 0) yield break;
		
		float start_x = transform.position.x;
		startMoving();
		
		float elapsed_time = 0;
		
		while (elapsed_time < movementTime) {
			float x = Mathf.Lerp(start_x, start_x + movement.x, Mathf.Sqrt(elapsed_time / movementTime));
			transform.position = new Vector2(x, transform.position.y);
			elapsed_time += Time.deltaTime;
			yield return null;
		}
		
		//transform.position = movement_start_position + movement;		
		stopMoving();
		
		yield break;
	}*/
	
	private void startMoving() {
		can_move = false;
		//animator.SetBool("walking", true);
		/*audiomgr.PlaySound(
			(movementType == PlayerMovementType.Jump) ? PlayerSounds.JumpSound : PlayerSounds.WalkSound
			);*/
	}
	
	private void stopMoving() {
		can_move = true;
	}
	
	private float getFallTime() {
		if (fall_type == FallType.Normal)
			return dm.PieceFallDelay;
		else if (fall_type == FallType.Fast)
			return fastFallTime;
		else if (fall_type == FallType.Instant)
			return 0;
			
		return 0;
	}
	
	private IEnumerator fall() {
		while (falling) {
		
			// STEP 1: Wait for the appropriate time that's passed before you fall.
			float elapsed_time = 0;
			
			while (elapsed_time < getFallTime ()) {
				yield return null;
				elapsed_time += Time.deltaTime;
			}
			
			// STEP 2: See if you landed.
			foreach (GameObject block in blocks) {
				if (board.Collide (block.transform.position + Vector3.down * size, PlayerNumber) == BlockCollision.Solid) {	
					stopFalling ();
				}
			}
			
			if (falling)
				transform.position = transform.position + Vector3.down * size;
		}
		
		land ();
		yield break;
	}
	
	private void tryToRotate(float z_angle) {
		Quaternion rotation = Quaternion.Euler(0,0, z_angle + transform.rotation.eulerAngles.z);
	
		float farthest_collision_point = 0;
		foreach (GameObject block in blocks) {
			Vector3 rotated_position = block.transform.localPosition;
			rotated_position = transform.position + (rotation * rotated_position);
			
			if (board.CollideWithBlock(rotated_position, PlayerNumber) == BlockCollision.Solid)
				return;
			
			if (board.CollideWithWall(rotated_position) == BlockCollision.Solid) {
				float gap = 0;
			
				if (rotated_position.x < transform.position.x) {
					gap = rotated_position.x - transform.position.x;
					farthest_collision_point = Mathf.Min (gap, farthest_collision_point);			
				}
				
				if (rotated_position.x > transform.position.x) {
					gap = rotated_position.x - transform.position.x;
					farthest_collision_point = Mathf.Max (gap, farthest_collision_point);			
				}
			}
		}
		
		transform.Rotate(0,0,z_angle);
		transform.position = transform.position - new Vector3(farthest_collision_point, 0, 0);	
	}

	private void stopFalling() {
		falling = false;
	}
			
	private void land() {
		GameObject.FindGameObjectWithTag("GameManager").SendMessage("PieceLanded", this);
	}
	
	private void generateBlocks() {
		int block_shape = Random.Range(0, block_types.GetLength(0));
		
		for (int block_count = 0; block_count < 4; block_count++) {
			int x = block_types[block_shape, block_count, 0];
			int y = block_types[block_shape, block_count, 1];
			
			GameObject block = Instantiate (fallingBlock) as GameObject;
			block.transform.parent = transform;
			block.transform.localPosition = new Vector3(x, y, 0);
		}
	}
	
	private void generateGhost() {
		GameObject ghost = Instantiate(ghostPiece);
		ghost.GetComponent<GhostPiece>().Piece = this;
	}
	
	public int PlayerNumber {
		get { return input.playerNumber; }
		set {
			input.playerNumber = value;
			setOutline(value);
		}
	}
	
	private void setOutline(int player_number) {
		Color player_color = playerColorList.Colors[player_number];
	
		// Set the glow effect to the player's color
		foreach (GameObject block in Blocks) {		
			MeshRenderer renderer = block.GetComponent<MeshRenderer>();
			Material material = renderer.material;
			material.SetColor ("_OutlineColor", player_color);
		}
	}

	
	public GameObject[] Blocks { get { return blocks; } }
	
/*	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawCube (transform.position, Vector3.one);
	}*/
}

enum FallType {
	Normal,
	Fast,
	Instant
}
