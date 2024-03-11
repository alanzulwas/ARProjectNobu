using System.IO;
using UnityEngine;
using Dummiesman;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;
using Unity.Mathematics;
using System.Security.Cryptography;

public class Natipfilpick : MonoBehaviour
{
	public string FinalPath = string.Empty;
	public Transform ObjectPlace;
	public Transform ObjectHoldPlace;

	[Header("UI")]
	public GameObject StatusObjectCountText;
	public GameObject LoadingScreen;
	public TextMeshProUGUI LoadingPanelText;

	string error = string.Empty;
	GameObject loadedObject;

	public void LoadFile()
	{
		string[] allowedFileTypes;
		if (Application.platform == RuntimePlatform.Android)
			allowedFileTypes = new string[] { "*/*" };
		else
			allowedFileTypes = new string[] { "public.item", "public.content" };

		NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
		{
			if (path == null) Debug.Log("Operation cancelled");
			else
			{
				//ARToTapObject.Instance._objectPrefab.Add(loadedObject);
				try
				{
					//FinalPath = path;

					//string filePath = FinalPath;
					//string mtlPath = @"Assets/Material/Testing.mat";

					//if (loadedObject != null)
					//	Destroy(loadedObject);
					//loadedObject = new OBJLoader().Load(filePath, mtlPath);
					//error = string.Empty;
					//if (loadedObject != null)
					//{

					//	if (ObjectPlace.gameObject.active) loadedObject.transform.position = ObjectPlace.position;
					//	else if (ObjectHoldPlace.gameObject.active) loadedObject.transform.position = ObjectHoldPlace.position;
					//}
					SaveTheImport(loadedObject.name, FinalPath);
				}
				catch(Exception e)
				{
					//StartCoroutine(LoadYourAsyncScene());
					Debug.Log("Error Something");
					Debug.LogError("\n "+e);
					SaveTheImport("Default-" + UnityEngine.Random.Range(0, 10) + "-Object", FinalPath);
				}

				StatusObjectCountText.SetActive(false);
				LoadingScreen.SetActive(true);

				LoadingPanelText.text = "Loading Import 3d";

				StartCoroutine(LoadingImport3D());

				Destroy(loadedObject);
				Debug.Log("Picked file: " + FinalPath);
			}
		}, allowedFileTypes);
	}

	private void SaveTheImport(string objectImportedname, string SourcePath)
	{
		// Create folder Prefabs and set the path as within the Prefabs folder,
		// and name it as the GameObject's name with the .Prefab format
		string fileExtension = Path.GetExtension(SourcePath);

		//if (!Directory.Exists("Assets/Resources/PrefabsForSaveAssets"))
		//	AssetDatabase.CreateFolder("Assets", "Resources/PrefabsForSaveAssets");
		//FileUtil.CopyFileOrDirectory(SourcePath, "Assets/Resources/PrefabsForSaveAssets/" + objectImported+fileExtension);
		if (!Directory.Exists(Application.persistentDataPath + "/Object3Dimport"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Object3Dimport");
			//CreateFolder(Application.persistentDataPath, "Materials");
		}
		File.Copy(SourcePath, DirectoryrResponse.Instance.Used3DobjectDirectory+"/"+objectImportedname+fileExtension);

		//ARToTapObject.Instance.PrefabAsset = Resources.LoadAll<GameObject>("PrefabsForSaveAssets");
		//ARToTapObject.Instance.angkaCacheSaved = ARToTapObject.Instance.PrefabAsset.Length;

		//StartCoroutine(LoadYourAsyncScene());
	}

	IEnumerator LoadingImport3D()
	{
		yield return new WaitForSeconds(1);

		StatusObjectCountText.SetActive(true);
		LoadingScreen.SetActive(false);

		LoadingPanelText.text = "Null Description";
		ARToTapObject.Instance.UpdateObjectPrefab();
	}
}
 