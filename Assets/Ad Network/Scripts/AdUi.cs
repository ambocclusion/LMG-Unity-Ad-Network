using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdUi : MonoBehaviour {

	public GameObject adTemplate;

	private bool initialized = false;

	void Start(){

		this.transform.parent.gameObject.SetActive(false);

	}

	public void Open(){

		this.transform.parent.gameObject.SetActive(true);

		StartCoroutine(OpenRoutine());

	}

	IEnumerator OpenRoutine(){

		adTemplate.SetActive(false);

		while(AdLoader.Instance.loadedAds.loaded == false){
			yield return null;
		}

		if(!initialized){
			StartCoroutine(InitAds(AdLoader.Instance.loadedAds));
		}

	}

	public IEnumerator InitAds(AdList ads){

		yield return StartCoroutine(LoadImages(ads));

		adTemplate.SetActive(true);

		foreach(Ad a in ads.values){

			GameObject obj = Instantiate(adTemplate) as GameObject;
			obj.transform.SetParent(this.transform.Find("Ads"));

			obj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=> {
				Application.OpenURL(a.linkedUrl);
			});

			obj.transform.Find("Game Image").GetComponent<UnityEngine.UI.Image>().sprite = a.sprites[0];
			obj.transform.Find("Game Name").GetComponent<UnityEngine.UI.Text>().text = a.gameName;

		}

		adTemplate.SetActive(false);

		initialized = true;

	}

	private IEnumerator LoadImages(AdList ads){

		foreach(Ad a in ads.values){

			yield return StartCoroutine(a.LoadImages());

		}

	}

}
