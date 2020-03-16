using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAllAnimations : MonoBehaviour {

	private int animInterval = 0;
	public AnimationClip[] animations;
	public Animator animator;
	public Text displayText;

	void Start () {
		PlayNextAnimation ();
	}
	public void PlayNextAnimation() {
		AnimationClip myAnim = animations [animInterval];
		animator.Play (myAnim.name);
		displayText.text = myAnim.name;
		if (myAnim.isLooping) {
			StartCoroutine (WaitForAnimation (animations [animInterval].length * 2));
		} else {
			StartCoroutine (WaitForAnimation (animations [animInterval].length));
		}
	}
	public IEnumerator WaitForAnimation(float timer) {
		while (timer > 0) {
			timer -= 1 * Time.deltaTime;
			yield return null;
		}
		animInterval++;
		if (animInterval < animations.Length) {
			PlayNextAnimation ();
		} else {
			displayText.text = "";
			print ("Done.");
		}
	}
}
