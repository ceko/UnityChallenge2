using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DetonateButton : MonoBehaviour {

	public EMPTower Tower;
	public Image ChargingImage;
	public Image ReadyImage;
	private Button Button;

	// Use this for initialization
	void Start () {
		Button = GetComponent<Button>();
		Button.onClick.AddListener(() => {
			Tower.Fire();
		});
	}
	
	// Update is called once per frame
	void Update () {
		ChargingImage.fillAmount = Tower.ChargeAmount / 2.5f;
		ReadyImage.fillAmount = (Tower.ChargeAmount - 2.5f) / .5f;

		Button.interactable = Tower.ChargeAmount >= 2.5f;
	}
}
