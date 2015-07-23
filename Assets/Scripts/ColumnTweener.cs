using UnityEngine;
using System.Collections;

public class ColumnTweener : MonoBehaviour {

	public float tweenTime;
	public float tweenAmount;
	
	public void Tween() {
		int height = Height;
		float scale_factor = (height > 0) ? (height - tweenAmount) / height : .7f;

	
		GoTweenConfig squash_config  = new GoTweenConfig().scale(new Vector3(1 / scale_factor, scale_factor, 1 / scale_factor));
		GoTweenConfig restore_config = new GoTweenConfig().scale(Vector3.one);
			
		GoTween squash_tween  = new GoTween( transform, tweenTime, squash_config);
		GoTween restore_tween = new GoTween( transform, tweenTime, restore_config);
			
		var flow = new GoTweenFlow();
		flow.insert( 0, squash_tween ).insert( tweenTime, restore_tween );
		flow.play();
	}
	
	public int Height {
		get {
			float height = 0;
			foreach (Transform child in transform) {
				if (child.position.y > height)
					height = child.position.y;
			}
			return Mathf.FloorToInt(height);
		}
	}
}
