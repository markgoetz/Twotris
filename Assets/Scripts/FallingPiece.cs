﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputManager))]
public class FallingPiece : MonoBehaviour {
	public float fastFallTime = .0000005f;
	public GameObject fallingBlock;
	public GameObject ghostPiece;
	public PlayerColorList playerColorList;
	public GameObject dustCloud;
	public SleepSettings landSleepSettings;
	public int points;
	
	private int size = 1;
	private bool can_move = true;
	private bool falling = true;
	
	private Board board;
	private DifficultyManager dm;	
	private SleepManager sleepManager;
	
	private GameObject ghost;
	private PieceTweener tweener;
	private PieceSpawner piece_spawner;

	private InputManager input;
	private FallType fall_type;
	private PieceSoundManager audio_manager;
	private PieceState state;
		
	private GameObject[] blocks;
	
	
	void Awake() {
		input = GetComponent<InputManager>();
		audio_manager = GetComponent<PieceSoundManager>();	
		board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
		dm = GameObject.FindGameObjectWithTag("DifficultyManager").GetComponent<DifficultyManager>();
		sleepManager = GameObject.FindGameObjectWithTag("SleepManager").GetComponent<SleepManager>();
		
		tweener = GetComponentInChildren<PieceTweener>();
	}
	
	public void init(PieceSpawner spawner, PieceTemplate piece) {
		generateBlocks(piece);
		PlayerNumber = spawner.playerNumber;
		piece_spawner = spawner;
		
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
		if (state == PieceState.Falling) {	
			fixOverlap();
				
			handleKeyInput();
			Vector3 movement = getMovement();
			
			if (movement.x != 0) {
				movement = testMovement(movement);
				
				if (movement.magnitude > 0) {
					// Don't use transform.translate here as it will totally mess with the rotation.
					transform.position = transform.position + movement;
					moveEffect (movement);
				}
			}
		}
		
		else if (state == PieceState.WaitingForEffect) {
			if (GetComponentInChildren<PieceTweener>().Done) {
				setState (PieceState.Landed);
			}
		}
	}
	
	void fixOverlap() {
		bool overlap = true;
	
		while (overlap) {
			overlap = false;
			foreach (GameObject block in blocks) {				
				if (board.Collide(block.GetComponent <Block>().GamePosition, PlayerNumber) == BlockCollision.Solid) {
					transform.position = transform.position + new Vector3(0, 1, 0);
					overlap = true;
				}
			}
		}
	}
	
	Vector3 testMovement(Vector3 movement) {
		foreach (GameObject block in blocks) {				
			if (board.Collide(block.GetComponent <Block>().GamePosition + movement, PlayerNumber) == BlockCollision.Solid) {
				movement = Vector2.zero;
			}
		}
		
		return movement;
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
			fixOverlap();
			foreach (GameObject block in blocks) {
				if (board.Collide (block.GetComponent<Block>().GamePosition + Vector3.down * size, PlayerNumber) == BlockCollision.Solid) {	
					stopFalling ();
				}
			}
			
			if (falling) {
				transform.position = transform.position + Vector3.down * size;
				tweener.Move (Vector3.down * size);
			}
		}
		
		setState(PieceState.WaitingForEffect);
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

			block.transform.parent = tweener.gameObject.transform;
			block.transform.localPosition = location;
			
			blocks[count++] = block.gameObject;
		}
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
			tweener.AppearEffect();
		}
	
		if (state == PieceState.Falling) {
			tweener.StartFallEffect();	
			generateGhost();
			fall_type = FallType.Normal;
			StartCoroutine("fall");
		
			ShowOutline(true);
			GetComponent<InputManager>().enabled = true;
		}
		
		else if (state == PieceState.WaitingForEffect) {
			if (fall_type == FallType.Instant) {
				audio_manager.PlaySound(PlayerSounds.Fall);
				tweener.Fastfall = true;
				foreach (GameObject block in blocks) {
					block.SendMessage ("FastfallEffect");
				}
			}
		}
		
		else if (state == PieceState.Landed) {
			landed();
		}
	}
	
	private void landed() {
		landEffect();
		piece_spawner.SpawnPieceWithDelay();
		board.AddBlocks (Blocks);
		ghost.SendMessage("Remove");
		GameObject.FindGameObjectWithTag("ScoreKeeper").SendMessage("AddScore", points);
		
		Destroy (this.gameObject);
	}

	private void OnPieceStateExit(PieceState state) {
		if (state == PieceState.Falling) {
			GetComponent<InputManager>().enabled = false;
			StopCoroutine ("fall");
			
			ShowOutline (false);
		}
	}
	
	private void moveEffect(Vector3 movement) {
		// Sound effect
		audio_manager.PlaySound(PlayerSounds.Move);
	
		foreach (GameObject block in blocks) {
			block.SendMessage ("MoveEffect");
		}
		
		// Tween, squash and stretch
		tweener.Move (movement);
	}
	
	private void rotateEffect(float z_angle) {
		// Sound effect
		audio_manager.PlaySound(PlayerSounds.Rotate);
		
		foreach (GameObject block in blocks) {
			block.SendMessage ("RotateEffect");
		}
		
		// Tween, wobble
		tweener.Rotate(z_angle);
	}
	
	private void landEffect() {
		tweener.Stop ();
		
		sleepManager.SendMessage("Sleep", landSleepSettings);
	
		// Flash
		foreach (GameObject block in Blocks) {
			block.SendMessage ("Flash");
		}
		
		// Sound effect
		audio_manager.PlaySound(PlayerSounds.Land);
		
		
		// Screenshake
		Camera.main.SendMessage("Shake", CameraShakeType.Land);
		
		spawnDustClouds();
	}
	
	private void spawnDustClouds() {
		foreach (GameObject block in blocks) {
			if (board.Collide (block.transform.position + Vector3.down, PlayerNumber) != BlockCollision.NoCollision)
				Instantiate (dustCloud, block.transform.position + new Vector3(0,0,-1), Quaternion.identity);
		}
	}
	
	public GameObject[] Blocks { get { return blocks; } }
	public bool Falling { get { return state == PieceState.Falling || state == PieceState.WaitingForEffect; } }
	
	public static Vector3 Centroid {
		get {
			Vector3 centroid = Vector3.zero;
			int piece_count = 0;
		
			GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
			foreach (GameObject piece_obj in pieces) {
				FallingPiece piece = piece_obj.GetComponent<FallingPiece>();
				if (!piece.Falling) continue;
				
				piece_count++;
				centroid += piece.transform.position;
			}
			
			if (piece_count > 0)
				return centroid / piece_count;
			else
				throw new CameraTrackerException();

		}
	}
	
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
	WaitingForEffect,
	Landed
}
