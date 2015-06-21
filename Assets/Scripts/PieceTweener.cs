using UnityEngine;
using System.Collections;

public class PieceTweener : MonoBehaviour {
	public float stretchFactor;
	public float tweenTime;
	public float acceleration;
	public float rotation_acceleration;

	private Vector3 velocity = Vector3.zero;
	private float rotation_velocity = 0;
	
	private AbstractGoTween _tween;
	

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
		
		StretchMovement();
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
		
		/*if( _move_tween != null )
		{
			_move_tween.complete();
			_move_tween.destroy();
			_move_tween = null;
		}
		
		
		_move_tween = Go.to (
			transform,
			tweenTime,
			new GoTweenConfig().localPosition(new Vector3(0,0,0)).setEaseType(GoEaseType.CubicOut)
		);*/
	}
	
	public void Rotate(float z_angle) {
		// Wobble
		resetTween ();
		_tween = Go.to (
			transform,
			.1f,
			new GoTweenConfig()
				.scale (Vector3.one * 1.2f)
				.setEaseType (GoEaseType.ElasticInOut)
				.setIterations(2, GoLoopType.PingPong)
		);
		
		transform.localRotation = Quaternion.Euler(0,0,transform.localRotation.eulerAngles.z - z_angle);
	}
	
	public bool Done {
		get {
			return (transform.localPosition == Vector3.zero && transform.localRotation == Quaternion.identity);
		}
	}
	
	public void Stop() {
		resetTween ();
		transform.localPosition = Vector3.zero;
		velocity = Vector3.zero;
		transform.localScale = Vector3.one;
	}
	
	
	// Handles squash and stretch.
	void StretchMovement() {
		// TODO: What if you're moving in X and Y?
		
		Vector3 unrotated_position = transform.parent.rotation * transform.localPosition;
		Vector3 scale;
		
		//if (unrotated_position.x == 0) {
			float scale_factor = 1 + Mathf.Abs (unrotated_position.x) * stretchFactor;
			scale = transform.parent.rotation * new Vector3(scale_factor, 1 / scale_factor, 1 / scale_factor);
		/*}
		else {
			float scale_factor = 1 + Mathf.Abs (unrotated_position.y) * stretchFactor;
			scale = transform.parent.rotation * new Vector3(1 / scale_factor, scale_factor, 1 / scale_factor);
		}*/
		
		
		
		
		if (scale.x < 0)
			scale.x = -scale.x;
		if (scale.y < 0)
			scale.y = -scale.y;
			
		transform.localScale = scale;
		
	}
	
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
