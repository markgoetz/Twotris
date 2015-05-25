using UnityEngine;
using System.Collections.Generic;

// DEPRECATED.
// This used to test if you could rotate this block.
public class RotateTester : MonoBehaviour {
	private List<Vector3> collisions;
	private RotateTesterBlock[] children;
	private bool collidedWithBlock = false;
	private bool collidedWithWall  = false;
	
	public GameObject rotateTesterBlock;

	public void AddBlock(Vector3 position) {
		GameObject block = Instantiate (rotateTesterBlock, position, Quaternion.identity) as GameObject;
		block.transform.parent = transform;
	}
	
	private void checkCollisions() {
		if (collisions != null) {
			return;
		}
		
		collisions = new List<Vector3>();
		children = GetComponentsInChildren<RotateTesterBlock>();
		
		foreach (RotateTesterBlock child in children) {
			child.checkCollision();
			
			if (child.CollidedWithWall) {
				collisions.Add (child.transform.position - transform.position);
				collidedWithWall = true;
			}
			
			if (child.CollidedWithBlock) {
				collisions.Add (child.transform.position - transform.position);
				collidedWithBlock = true;
			}
		}
	}
	
	public bool CollidedWithBlock {
		get { 
			checkCollisions();
			return collidedWithBlock;
		}
	}
	
	
	public bool CollidedWithWall {
		get {
			checkCollisions();
			return collidedWithWall;
		}
	}
	
	public float FarthestCollisionPoint {
		get {
			if (collisions.Count == 0)
				throw new UnityException("No wall collisions.");
			
			float farthest_point = 0;
			foreach (Vector3 collision in collisions) {
				if (collision.x < 0 && collision.x < farthest_point)
					farthest_point = collision.x;
				if (collision.x > 0 && collision.x > farthest_point)
					farthest_point = collision.x;
			}
			
			return farthest_point;
		}
	}
}
