using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RandomDelayFall : MonoBehaviour {
	public float maxDelayTime;
	
	void Start() {
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().useGravity = false;
	}
	
	public void Fall() {
		StartCoroutine ("FallCoroutine");
	}
	
	private IEnumerator FallCoroutine() {
		float time = Random.Range (0, maxDelayTime);
		yield return new WaitForSeconds(time);
		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().useGravity = true;
	}
}
