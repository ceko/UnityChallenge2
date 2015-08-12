using UnityEngine;
using System.Collections;

public class PumpTower : MonoBehaviour {

	public GameObject Panel;
	public MeshRenderer BeamRenderer;
	public Sun Sun;
	public float Charge {
		get; 
		private set;
	}
	private Vector2 beamTextureOffset = Vector2.zero;

	void Update () {
		Quaternion desiredRotation;
		if(Sun.Power > 0) {
			desiredRotation = Quaternion.LookRotation(Sun.transform.position - Panel.transform.position, Vector3.up);
			Charge += Time.smoothDeltaTime / 2f;
		}else{
			desiredRotation = Quaternion.identity;
			Charge -= Time.smoothDeltaTime / 3f;
		}

		Panel.transform.rotation = Quaternion.Slerp(Panel.transform.rotation, desiredRotation, Time.smoothDeltaTime);
		Charge = Mathf.Clamp01(Charge);

		BeamRenderer.material.SetColor("_Emission", Color.Lerp(Color.black, Color.green, Charge) * Mathf.LinearToGammaSpace(Charge*30f));
		BeamRenderer.material.SetColor("_Color", Color.Lerp(Color.black, Color.white, Charge));
		BeamRenderer.material.SetFloat("_Churn", Charge);
		beamTextureOffset = new Vector2(beamTextureOffset.x - Charge * Time.smoothDeltaTime, beamTextureOffset.y);
		BeamRenderer.material.SetTextureOffset("_MainTex", beamTextureOffset);
	}
}
