using UnityEngine;
using System.Collections;

public class FallingBlock : MonoBehaviour {
	public bool BlockDetect(Vector2 movement, int layer_mask) {
	
		if (Physics.Raycast(transform.position, movement, movement.magnitude, layer_mask)) {		
			return true;
		}
		
		return false;
	}
}
