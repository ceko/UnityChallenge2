using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {

	private Light light;
	public Color NoonColor;
	public Color DuskColor;

	public Color NightColor;
	public Color DayColor;

	public float Power {
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(Vector3.zero);
		transform.RotateAround(Vector3.zero, Vector3.forward, 30f * Time.smoothDeltaTime);
		Power = Mathf.Clamp01(Vector3.Dot(transform.forward, Vector3.down));
		light.intensity = 1.5f * Power;
		light.color = Color.Lerp(DuskColor, NoonColor, Power);

		Camera.main.backgroundColor = Color.Lerp(NightColor, DayColor, Power);
	}
}
