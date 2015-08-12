using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private bool alive = true;
	private float jumpInterval;
	public AnimationCurve JumpMotion;
	public GameObject Tower;
	private Vector3 startPosition;

	void Start() {
		startPosition = transform.position;
		Tower = GameObject.Find("/EMPTower");
		jumpInterval = Random.Range(1.5f, 2.5f);
		StartCoroutine(JumpForward());
	}

	void Update() {
	}

	void OnTriggerEnter(Collider other) {
		if(alive) {
			GetComponent<Animator>().SetTrigger("Die");
			StartCoroutine(FlickerAndRebirth());
			alive = false;
		}
	}

	private IEnumerator JumpForward() {
		yield return new WaitForSeconds(Random.Range(1f, 1.5f));

		while(alive) {
			GetComponent<Animator>().SetTrigger("Jump");

			if(Mathf.Abs(Vector3.Distance(transform.position, Tower.transform.position)) > 2f)
				StartCoroutine(MoveForward());
			yield return new WaitForSeconds(jumpInterval);
		}
	}

	private IEnumerator MoveForward() {
		Vector3 startPosition = transform.position;

		for(float i = 0;i<1f;i+=Time.smoothDeltaTime) {
			transform.position = startPosition + transform.forward * JumpMotion.Evaluate(JumpMotion.Evaluate(i));
			yield return null;
		}
	}

	private IEnumerator FlickerAndRebirth() {
		//wait for death animation to complete
		yield return new WaitForSeconds(2f);

		SkinnedMeshRenderer renderer = transform.Find("Cube").GetComponent<SkinnedMeshRenderer>();
		Color startingColor = renderer.material.color;
		for(int i=1;i<=6;i++) {
			if(i%2==0) {
				renderer.enabled = true;
			}else{
				renderer.enabled = false;
			}
			yield return new WaitForSeconds(.25f);
		}

		renderer.enabled = false;
		yield return new WaitForSeconds(1f);

		GameObject.Instantiate(Resources.Load("Enemy"), startPosition, transform.rotation);
		GameObject.Destroy(gameObject);
	}

}
