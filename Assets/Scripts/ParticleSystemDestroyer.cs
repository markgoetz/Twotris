using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ParticleSystem))]
public class ParticleSystemDestroyer : MonoBehaviour {

	private ParticleSystem ps;

	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if (!ps.IsAlive())
			Destroy (gameObject);
	}
}
