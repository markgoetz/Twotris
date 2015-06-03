using UnityEngine;

/******
 * This class abstracts the input keys into actions that the FallingPiece object needs to take.
 * The main reason for doing this is that Up and Down on a Dualshock 4 D-Pad is represented as an axis, rather than as buttons.
 */

public class InputManager : MonoBehaviour {
	public int playerNumber;
	
	private int move;
	private bool fast_fall;
	private bool instant_fall;
	private int rotation;
	
	// Used internally to make sure that the left and right arrow keys are only processed once per press.
	private bool read_move = false;
	
	void Update() {
		// First, read in the horizontal axis.
		// Use the read_move flag to catch when the left and right arrows are pressed for the first time.
		if (read_move == true) {
			move = 0;
			
			if (Input.GetAxisRaw (HorizontalAxis) == 0)
				read_move = false;
		}
		else {	
			move = Mathf.RoundToInt(Input.GetAxisRaw (HorizontalAxis));
			if (move != 0) read_move = true;
		}
		
		
		// Read in the vertical axis.  Use a dead zone.
		fast_fall = (Input.GetAxisRaw (VerticalAxis) > .05f);
		instant_fall = (Input.GetAxisRaw (VerticalAxis) < -.05f);
		
		
		
		// finally, read in the rotation buttons.
		if (Input.GetButtonDown (Clockwise)) {
			rotation = 1;
		}
		else if (Input.GetButtonDown (Counterclockwise)) {
			rotation = -1;
		}
		else {
			rotation = 0;
		}
	}
	
	// These methods create the Axis labels that were set up in Unity.
	private string PlayerName {
		get { return "P" + (playerNumber) + " "; }
	}
	private string HorizontalAxis { get { return PlayerName + "Horizontal"; }}
	private string VerticalAxis { get { return PlayerName + "Vertical"; }}
	private string Clockwise { get { return PlayerName + "Clockwise"; }}
	private string Counterclockwise { get { return PlayerName + "Counterclockwise"; }}
	
	// Public facing properties that FallingPiece can use to get the abstracted actions.
	public int Move { get { return move; } }
	public bool FastFall { get { return fast_fall; } }
	public bool InstantFall { get { return instant_fall; } }
	public int Rotation { get { return rotation; } }
	
/*	void OnDrawGizmos() {
		Gizmos.DrawCube (new Vector3(-5 + playerNumber, 8, 0), Vector3.one / 3f);
	
		Gizmos.color = Color.green;
		Gizmos.DrawRay(new Vector3(-5 + playerNumber, 8, 0), new Vector3(Input.GetAxisRaw (PlayerName + "Horizontal"), Input.GetAxisRaw (PlayerName + "Vertical")));
	
		if (Input.GetButtonDown (PlayerName + "Clockwise")) {
			Gizmos.color = Color.blue;
			Gizmos.DrawCube (new Vector3(-5 + playerNumber, 9, 0), Vector3.one);
		}
		
		if (Input.GetButtonDown (PlayerName + "Counterclockwise")) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube (new Vector3(-5 + playerNumber, 10, 0), Vector3.one);
		}
	}*/
}
