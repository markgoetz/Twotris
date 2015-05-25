using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	private BlockState state;
	
	public void Transition(BlockState new_state) {
		OnBlockStateLeave(state);
		state = new_state;
		OnBlockStateEnter(new_state);
	}
	
	private void OnBlockStateLeave(BlockState new_state) {
	
	}
	
	private void OnBlockStateEnter(BlockState new_state) {
	
	}
}

public enum BlockState {
	Falling,
	Stopped,
	Cleared,
	
}