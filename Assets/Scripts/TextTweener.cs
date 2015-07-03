using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TextTweener : MonoBehaviour {
	public float tweenStartTime;
	public float tweenEndTime;
	public int tweenTextSize;
	public Color tweenTextColor;
	
	private Text text;
	private int start_text_size;
	private Color start_text_color;
	
	void Start() {
		text = GetComponent<Text>();
		start_text_size = text.fontSize;
		start_text_color = text.color;
	}
	
	public void Tween() {
		StartCoroutine("TweenCoroutine");
	}
	
	private IEnumerator TweenCoroutine() {
		float elapsed = 0;
		while (elapsed <= tweenStartTime + tweenEndTime) {
			if (elapsed < tweenStartTime) {
				SetTween(elapsed / tweenStartTime);
			}
			else {
				SetTween (1 - ((elapsed - tweenStartTime) / tweenEndTime));
			}
		
			yield return null;
			elapsed += Time.deltaTime;
		}
	}
	
	private void SetTween(float factor) {
		text.fontSize = Mathf.RoundToInt(Mathf.Lerp (start_text_size, tweenTextSize, factor));
		
		// TODO: text color is tweened along RGB axis.  It would look better if it were tweened along HSB axis.
		text.color = new Color(
			Mathf.Lerp (start_text_color.r, tweenTextColor.r, factor),
			Mathf.Lerp (start_text_color.g, tweenTextColor.g, factor),
			Mathf.Lerp (start_text_color.b, tweenTextColor.b, factor)
		);
	}
}
