using UnityEngine;
using System.Collections;

public class EMPTower : MonoBehaviour {

	public GameObject UpperRing;
	public GameObject LowerRing;
	public SkinnedMeshRenderer EMPCanisterRenderer;
	public EMPEffect EMPEffect;
	public PumpTower PumpTower;
	public float ChargeAmount {
		get;
		private set;
	}

	private Animator TowerAnimator;
	private float ringRotationSpeed = 1f;
	private float targetRingRotationSpeed = 1f;

	public enum TowerStates {
		Dead,
		Charging,
		Firing
	}

	private TowerStates State = TowerStates.Dead;
	
	void Start() {
		TowerAnimator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		switch(State) {
			case TowerStates.Dead:
				DeadStateLoop();
				break;
			case TowerStates.Charging:
				ChargingStateLoop();
				break;
			case TowerStates.Firing:
				FiringStateLoop();
				break;
		}

		ringRotationSpeed = Mathf.Lerp(ringRotationSpeed, targetRingRotationSpeed, Time.smoothDeltaTime);
		UpperRing.transform.RotateAround(UpperRing.transform.position, Vector3.up, 90 * Time.deltaTime * ringRotationSpeed);
		LowerRing.transform.RotateAround(LowerRing.transform.position, Vector3.up, -90 * Time.deltaTime * ringRotationSpeed);
	}

	#region Tower States
	private void TransitionToDead() {
		TowerAnimator.SetBool("Open", false);
		State = TowerStates.Dead;
	}

	private void DeadStateLoop() {
		//Animate the rings, they just rotate in this state and bob up and down.
		ChargeAmount = 0f;

		targetRingRotationSpeed = 1f;
		UpperRing.transform.position += Vector3.down/15f * Mathf.Sin(Time.time*5f%360f+180f) * Time.smoothDeltaTime;
		LowerRing.transform.position += Vector3.down/15f * Mathf.Sin(Time.time*5f%360f) * Time.smoothDeltaTime;

		if(PumpTower.Charge > .1f)
			TransitionToCharging();
	}

	private void TransitionToCharging() {	
		targetRingRotationSpeed = 10f;
		State = TowerStates.Charging;
		TowerAnimator.SetBool("Open", true);
	}

	private void ChargingStateLoop() {
		if(PumpTower.Charge > 0) {
			ChargeAmount += Time.smoothDeltaTime * PumpTower.Charge;
		}else{
			ChargeAmount -= Time.smoothDeltaTime * 2f;
		}

		ChargeAmount = Mathf.Min(ChargeAmount, 3f);
		EMPCanisterRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.green, ChargeAmount / 5) * Mathf.LinearToGammaSpace(1 + ChargeAmount*ChargeAmount*30f));
		if(ChargeAmount <= 0f) {
			TransitionToDead();
		}
	}

	private void TransitionToFiring() {
		targetRingRotationSpeed = 1f;
		State = TowerStates.Firing;
		TowerAnimator.SetBool("Open", false);
	}

	private void FiringStateLoop() {
		ChargeAmount = 0f;
		EMPEffect.Play();
		TransitionToDead();
	}

	public void Fire() {
		if(ChargeAmount > 2f)
			TransitionToFiring();
	}
	#endregion

}
