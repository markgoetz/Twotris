using UnityEngine;
using System.Collections;

// DEPRECATED.
// This used to test if you could rotate the piece.
public class RotateTesterBlock : MonoBehaviour {
	private bool collidedWithBlock = false;
	private bool collidedWithWall  = false;
	private Vector3 raycastLength = new Vector3(1.3f,1.3f,0);
	
	private int layer_mask;
	
	void Start() {
		layer_mask = 1 << LayerMask.NameToLayer("Wall");
	}
	
	public void checkCollision() {
		RaycastHit hit;
		Ray ray = new Ray(transform.position - raycastLength / 2, raycastLength);
		
		Debug.DrawRay (ray.origin, ray.direction, Color.red, 2f);
		Debug.DrawRay(transform.position, ray.direction / 2, Color.green, 2f);
		
		if (Physics.Raycast(ray, out hit, layer_mask)) {
			Debug.Log ("Collided at " + transform.localPosition + " with " + hit.collider.gameObject.tag);
				
			string tag = hit.collider.gameObject.tag;
			
			if (tag == "Block")
				collidedWithBlock = true;
			else if (tag == "Wall")
				collidedWithWall = true;
		}
	}
	
	public bool CollidedWithBlock {
		get { 
			return collidedWithBlock;
		}
	}
	
	
	public bool CollidedWithWall {
		get {
			return collidedWithWall;
		}
	}
}
