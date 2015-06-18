﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputManager))]
public class FallingPiece : MonoBehaviour {
	//public float movementTime;
	public float fastFallTime = .0000005f;
	public GameObject fallingBlock;
	public GameObject ghostPiece;
	public PlayerColorList playerColorList;
	public DifficultyManager dm;
	public CameraShakeParameters shakeOnLand;

	private int size = 1;
	private bool can_move = true;
	private Board board;
	private bool falling = true;
	private GameObject ghost;
	private GameObject tween_object;

	private InputManager input;
	private FallType fall_type;
	private PieceState state;
	
	private AbstractGoTween _tween;
	
	private GameObject[] blocks;
	
	void Awake() {
		input = GetComponent<InputManager>();
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
		tween_object = transform.GetChild(0).gameObject;
	}
	
	public void init(int player_number, PieceTemplate piece) {
		generateBlocks(piece);
		this.PlayerNumber = player_number;
		
		setState(PieceState.Next);
	}
	
	public void startFalling() {
		setState(PieceState.Falling);
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
				if (board.Collide(block.GetComponent <Block>().GamePosition + movement, PlayerNumber) == BlockCollision.Solid) {
					movement = Vector2.zero;
				}
			}
			
			if (movement.magnitude > 0) {
				// Don't use transform.translate here as it will totally mess with the rotation.
				transform.position = transform.position + movement;
				moveEffect (movement);
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
				if (board.Collide (block.GetComponent<Block>().GamePosition + Vector3.down * size, PlayerNumber) == BlockCollision.Solid) {	
					stopFalling ();
				}
			}
			
			if (falling) {
				transform.position = transform.position + Vector3.down * size;
				tween_object.SendMessage ("Move", Vector3.down * size);
			}
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
		rotateEffect(z_angle);
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
			block.GetComponent<Block>().BlockColor = piece.color;

			block.transform.parent = tween_object.transform;
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
		if (state == PieceState.Next) {
			ShowOutline(false);
			GetComponent<InputManager>().enabled = false;
		}
	
		if (state == PieceState.Falling) {
			fall_type = FallType.Normal;
			StartCoroutine("fall");
		
			ShowOutline(true);
			GetComponent<InputManager>().enabled = true;
		}
		else if (state == PieceState.Landed) {
			landEffect ();
			FlattenGameObject();
			GameObject.FindGameObjectWithTag("GameManager").SendMessage("PieceLanded", PlayerNumber);
		}
	}

	private void OnPieceStateExit(PieceState state) {
		if (state == PieceState.Falling) {
			GetComponent<InputManager>().enabled = false;
			StopCoroutine ("fall");
			
			ShowOutline (false);
		}
	}
	
	private void FlattenGameObject() {
		GameObject board = GameObject.FindGameObjectWithTag("Board");
		board.SendMessage ("AddBlocks", Blocks);
		
		ghost.SendMessage("Remove");
		Destroy (this.gameObject);
	}
	
	private void moveEffect(Vector3 movement) {
		// Sound effect
		// Tween, squash and stretch
		tween_object.SendMessage("Move", movement);
	}
	
	private void rotateEffect(float z_angle) {
		// Sound effect
		// Tween
		
		// Wobble
		_tween = Go.to (
			transform,
			.1f,
			new GoTweenConfig()
				.scale (Vector3.one * 1.2f)
				.setEaseType (GoEaseType.ElasticInOut)
				.setIterations(2, GoLoopType.PingPong)
		);
	}
	
	private void landEffect() {
		// reset!
		resetTween();
		
		tween_object.SendMessage ("Stop");
	
		// Flash
		foreach (GameObject block in Blocks) {
			block.SendMessage ("Flash");
		}
		
		// Sound effect
		
		// Screenshake
		Camera.main.SendMessage("Shake", shakeOnLand);
		
		// Squash / stretch
	}
	
	public GameObject[] Blocks { get { return blocks; } }
	
/*	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawCube (transform.position, Vector3.one);
	}*/
	
	private void resetTween() {
		if( _tween != null )
		{
			_tween.complete();
			_tween.destroy();
			_tween = null;
		}
		
		transform.localScale = Vector3.one;
	}
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
