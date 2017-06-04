using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomAd : MonoBehaviour {

	private AdUi adui;

	void Awake(){

		adui = FindObjectOfType<AdUi>();

	}

	IEnumerator Start(){

		if(adui == null){
			transform.Find("More Games Button").gameObject.SetActive(false);
		}

		while(AdLoader.Instance.loadedAds.loaded == false){
			yield return null;
		}

		int random = Random.Range(0, AdLoader.Instance.loadedAds.values.Length);

		StartCoroutine(InitAd(AdLoader.Instance.loadedAds.values[random]));

		if(adui != null){
			transform.Find("More Games Button").gameObject.SetActive(true);
			transform.Find("More Games Button").GetComponent<Button>().onClick.AddListener(()=> {
				adui.Open();
			});
		}

	}

	public IEnumerator InitAd(Ad ad){

		yield return StartCoroutine(ad.LoadImages());

		transform.Find("Game Image").GetComponent<Button>().onClick.AddListener(()=> {
				Application.OpenURL(ad.linkedUrl);
		});

		transform.Find("Game Image").GetComponent<Image>().sprite = ad.sprites[0];
		transform.Find("Game Name").GetComponent<Text>().text = ad.gameName;

	}

}
