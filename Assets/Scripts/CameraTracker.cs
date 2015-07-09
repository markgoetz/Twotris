using UnityEngine;
using System.Collections;

public class CameraTracker : MonoBehaviour {
	public Board board;
	public float boardPercentage;
	public float smoothTime;
	
	private float z;
	private Vector3 velocity;
	
	void Start() {
		z = transform.position.z;
	}
	
	void Update () {
		Vector3 board_center;
		Vector3 piece_center;
		
		try {
			board_center = board.Centroid;
		}
		catch {
			board_center = new Vector3(board.width / 2, board.height / 2, 0);
		}
		
		
		try {
			piece_center = FallingPiece.Centroid;
		}
		catch {
			piece_center = new Vector3(board.width / 2, board.height / 2, 0);
		}

		Vector3 target = boardPercentage * board_center + (1 - boardPercentage) * piece_center;
		
		transform.position = new Vector3(target.x, target.y, z);
	}
}

public class CameraTrackerException : System.Exception {}
