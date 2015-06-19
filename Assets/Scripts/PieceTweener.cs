using UnityEngine;
using System.Collections;

public class PieceTweener : MonoBehaviour {
	public float tweenFactor;
	public float acceleration;
	public float rotation_acceleration;

	private Vector3 velocity = Vector3.zero;
	private float rotation_velocity = 0;


	void Update () {
		handleMovement();
		handleRotation();
	}
	
	void handleMovement() {
		Vector2 direction = new Vector2(
			Mathf.Sign (transform.localPosition.x),
			Mathf.Sign (transform.localPosition.y)
		);
		

		velocity.x += acceleration * Time.deltaTime * -Mathf.Sign (transform.localPosition.x);
		velocity.y += acceleration * Time.deltaTime * -Mathf.Sign (transform.localPosition.y);
		
			
	
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
	}
	
	void handleRotation() {
		float z = transform.localRotation.eulerAngles.z;
		
		if (z == 0) return;
		
		float direction = Mathf.Sign (z - 180);
		rotation_velocity += rotation_acceleration * Time.deltaTime * direction;
		
	
		transform.localRotation = Quaternion.Euler (0,0,z + rotation_velocity * Time.deltaTime);
		
		if (Mathf.Sign (transform.localRotation.eulerAngles.z - 180) != direction) {
			transform.localRotation = Quaternion.identity;
			rotation_velocity = 0;
		}
	
	}
	
	public void Move(Vector3 movement) {
		transform.position = transform.position - movement;
	}
	
	public void Rotate(float z_angle) {
		transform.localRotation = Quaternion.Euler(0,0,transform.localRotation.eulerAngles.z - z_angle);
	}
	
	public bool Done {
		get {
			return (transform.localPosition == Vector3.zero && transform.localRotation == Quaternion.identity);
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
		
		Vector3 unrotated_position = transform.parent.rotation * transform.localPosition;
		
		float scale_factor = 1 + Mathf.Abs (unrotated_position.x) * tweenFactor;
		
		Vector3 scale = transform.parent.rotation * new Vector3(scale_factor, 1 / scale_factor, 1 / scale_factor);
		if (scale.x < 0)
			scale.x = -scale.x;
		if (scale.y < 0)
			scale.y = -scale.y;
			
		transform.localScale = scale;
		
	}
}
