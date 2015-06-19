using UnityEngine;
using System.Collections;

public class PieceTweener : MonoBehaviour {
	public float tweenFactor;
	public float acceleration;

	private Vector3 velocity = Vector3.zero;


	void Update () {
		Vector2 direction = new Vector2(
			Mathf.Sign (transform.localPosition.x),
			Mathf.Sign (transform.localPosition.y)
		);
		

		velocity.x += acceleration * Time.deltaTime * -Mathf.Sign (transform.localPosition.x);
		velocity.y += acceleration * Time.deltaTime * -Mathf.Sign (transform.localPosition.y);
		
		/*else {
			velocity.x = 0;
			transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
		}
		
		if (Mathf.Abs (transform.localPosition.y) > .05f) {
			velocity.y += acceleration * Time.deltaTime * -Mathf.Sign (transform.localPosition.y);
		}
		else {
			velocity.y = 0;
			transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
		}*/
		
	
	
	
		transform.localPosition = transform.localPosition + velocity * Time.deltaTime;
		
		if (Mathf.Sign (transform.localPosition.x) != direction.x) {
			transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
			velocity.x = 0;
		}
		
		if (Mathf.Sign (transform.localPosition.y) != direction.y) {
			transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
			velocity.y = 0;
		}
		
		TweenMovement();			
					
		/*if (Mathf.Sign (transform.localPosition.x) == Mathf.Sign (velocity.x)) {
			velocity.x = 0;
			transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
		}
		
		if (Mathf.Sign (transform.localPosition.y) == Mathf.Sign (velocity.y)) {
			velocity.y = 0;
			transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
		}*/
	}
	
	public void Move(Vector3 movement) {
		transform.position = transform.position - movement;
	}
	
	public bool Done {
		get {
			return (transform.localPosition == Vector3.zero);
		}
	}
	
	public void Stop() {
		transform.localPosition = Vector3.zero;
		velocity = Vector3.zero;
		transform.localScale = Vector3.one;
	}
	
	
	// Handles squash and stretch.
	void TweenMovement() {
		// TODO: What if you're moving in X and Y?
		
		Vector3 unrotated_position = transform.rotation * transform.localPosition;
		
		float scale_factor = 1 + Mathf.Abs (unrotated_position.x) * tweenFactor;
		
		Vector3 scale = transform.rotation * new Vector3(scale_factor, 1 / scale_factor, 1 / scale_factor);
		if (scale.x < 0)
			scale.x = -scale.x;
		if (scale.y < 0)
			scale.y = -scale.y;
			
		transform.localScale = scale;
		
	}
}
