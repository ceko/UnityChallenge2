using UnityEngine;
using System.Collections;

public class EMPBlast : MonoBehaviour {

	private MeshRenderer meshRenderer;

	public float EffectLifetime = 1f;
	public float StartScale = .1f;
	public float EndScale = 5f;
	public float StartRimWidth = 1f;
	public float StartDistortion = 128f;
	public Color StartColor = Color.white;
	private float effectStartTime;
	private bool playing = false;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer>();
		effectStartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(!playing) return;

		float elapsedTime = Time.time - effectStartTime;
		float effectProgress = Mathf.Min ((Time.time - effectStartTime) / EffectLifetime, 1);;
		gameObject.transform.localScale = Vector3.one * Interpolate.EaseOutQuint(StartScale, EndScale, elapsedTime, EffectLifetime);
		meshRenderer.material.SetFloat("_RimWidth", Mathf.Clamp(StartRimWidth - (Time.time - effectStartTime), .001f, 1f));
		meshRenderer.material.SetFloat("_BumpAmt", Mathf.Lerp(StartDistortion, 0f, effectProgress));
		meshRenderer.material.SetColor("_RimColor", Color.Lerp(StartColor, Color.white, effectProgress));

		if(elapsedTime > EffectLifetime) {
			playing = false;
			meshRenderer.enabled = false;
			GetComponent<SphereCollider>().enabled = false;
		}
	}

	public void Play() {
		meshRenderer.enabled = true;
		playing = true;
		Reset();
	}

	private void Reset() {
		gameObject.transform.localScale = StartScale * Vector3.one;
		GetComponent<SphereCollider>().enabled = true;
		effectStartTime = Time.time;
	}
}
