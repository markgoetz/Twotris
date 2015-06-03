﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputManager))]
public class FallingPiece : MonoBehaviour {
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
	private GameObject ghost;

	private InputManager input;
	private FallType fall_type;
	private PieceState state;
	
	private GameObject[] blocks;
	
	void Awake() {
		input = GetComponent<InputManager>();	
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
	}
	
	public void init(int player_number, PieceTemplate piece) {
		generateBlocks(piece);
		this.PlayerNumber = player_number;
		
		setState(PieceState.Falling);
		
		fall_type = FallType.Normal;
		StartCoroutine("fall");
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
		
		setState(PieceState.Landed);
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

	private void generateBlocks(PieceTemplate piece) {	
		blocks = new GameObject[piece.locations.Length];
	
		int count = 0;
		foreach (Vector2 location in piece.locations) {
			GameObject block = Instantiate (fallingBlock) as GameObject;
			block.SendMessage ("setColor", piece.color);

			block.transform.parent = transform;
			block.transform.localPosition = location;
			
			blocks[count++] = block.gameObject;
		}
		
		generateGhost ();
	}
	
	private void generateGhost() {
		ghost = Instantiate(ghostPiece);
		ghost.GetComponent<GhostPiece>().Piece = this;
	}
	
	public int PlayerNumber {
		get { return input.playerNumber; }
		set { input.playerNumber = value; }
	}
	
	public void ShowOutline(bool status) {
		if (status)
			setOutline (playerColorList.Colors[PlayerNumber]);
		else
			setOutline (new Color(0,0,0,0));
	}
	
	private void setOutline(Color c) {
		foreach (GameObject block in Blocks) {		
			block.SendMessage("setOutline", c);
		}
	}
	
	private void setState(PieceState new_state) {
		OnPieceStateExit(state);
		state = new_state;
		OnPieceStateEnter(state);
	}
	
	private void OnPieceStateEnter(PieceState state) {	
		if (state == PieceState.Falling) {
			ShowOutline(true);
			GetComponent<InputManager>().enabled = true;
		}
		else if (state == PieceState.Landed) {
			FlattenGameObject();
			GameObject.FindGameObjectWithTag("GameManager").SendMessage("PieceLanded", PlayerNumber);
		}
	}

	private void OnPieceStateExit(PieceState state) {
		if (state == PieceState.Falling) {
			GetComponent<InputManager>().enabled = false;
			ShowOutline (false);
		}
	}
	
	private void FlattenGameObject() {
		GameObject block_root = GameObject.FindGameObjectWithTag("BlockRoot");
		foreach (GameObject block in Blocks) {
			block.transform.parent = block_root.transform;
		}
		ghost.SendMessage("Remove");
		Destroy (this.gameObject);
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

enum PieceState {
	Next,
	Falling,
	Landed
}
