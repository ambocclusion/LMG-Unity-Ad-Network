using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdLoader : MonoBehaviour {

	protected static AdLoader instance;

	public static AdLoader Instance {

		get{

			if (instance == null){

				instance = (AdLoader)FindObjectOfType(typeof(AdLoader));

				if (instance == null){

					Debug.LogError("An instance of " + typeof(AdLoader) + " is needed in the scene, but there is none.");

				}

			}

			return instance;

		}

	}

	public delegate void ImageLoadCallback(Sprite spriteLoaded);

	public AdList loadedAds;

	private string URL = "";

	void OnEnable(){

		TextAsset urlFile = Resources.Load("adnetworkurl") as TextAsset;

		AdUrl ad = JsonUtility.FromJson<AdUrl>(urlFile.text);

		URL = ad.url;

		InitializeAds();

	}

	public void InitializeAds(){

		StartCoroutine(LoadAds());

	}

	public IEnumerator LoadAds(){

		if(string.IsNullOrEmpty(URL)){
			throw new System.Exception("Error getting URL for ads! Check to make sure 'adnetworkurl.json' exists in Assets/Resources/");
		}

		UnityWebRequest www = UnityWebRequest.Get(URL);
		
		yield return www.Send();

		string json = www.downloadHandler.text;
		AdList list = JsonUtility.FromJson<AdList>(json);

		loadedAds = list;

		loadedAds.loaded = true;

	}

	public IEnumerator LoadImageFromUrl(string url, ImageLoadCallback callback){

		UnityWebRequest www = UnityWebRequest.GetTexture(url);
		
		yield return www.Send();

		Texture2D tex = DownloadHandlerTexture.GetContent(www);
		Sprite sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), Vector2.zero);

		callback(sprite);

	}

}

[System.SerializableAttribute]
public class AdUrl{

	public string url = "";

}

[System.SerializableAttribute]
public class AdList {

	public bool loaded = false;
	public Ad[] values;

}

[System.SerializableAttribute]
public class Ad {

	public string linkedUrl = "";
	public string gameName = "";
	public string[] imageUrls = new string[0];

	public List<Sprite> sprites = new List<Sprite>();

	private bool imagesLoaded = false;

	public IEnumerator LoadImages(){

		if(imagesLoaded){
			yield break;
		}

		foreach(string url in imageUrls){

			yield return AdLoader.Instance.StartCoroutine(AdLoader.Instance.LoadImageFromUrl(url, OnSpriteLoaded));

		}

		imagesLoaded = true;

	}

	public void OnSpriteLoaded(Sprite loadedSprite){

		sprites.Add(loadedSprite);

	}

}
