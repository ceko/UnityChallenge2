using UnityEngine;
using System.Collections;

public class EMPEffect : MonoBehaviour {

	public ParticleSystem RingEffect;
	public EMPBlast EMPBlast;

	public void Play() {
		EMPBlast.Play();
		RingEffect.Play();
	}

}
