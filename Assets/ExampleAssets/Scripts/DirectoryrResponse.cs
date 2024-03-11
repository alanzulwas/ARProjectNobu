using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DirectoryrResponse : MonoBehaviour
{
	[Header("Default Directory Temporary")]
	public string DirectoryUsed;

	[Header("Directory Temporary Used")]
	public string Imported3Dobject;
	public string Used3DobjectDirectory;

	void Awake()
	{
		InitializeFolderCache();
	}

	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	void InitializeFolderCache()
	{
		DirectoryUsed = Application.persistentDataPath;
		Imported3Dobject = "/Object3Dimport";

		// Initialize Folder For Object3DImport
		Used3DobjectDirectory = DirectoryUsed + Imported3Dobject;
		if (!Directory.Exists(Used3DobjectDirectory)) Directory.CreateDirectory(Used3DobjectDirectory);
	}

	private static DirectoryrResponse _instance;

	public static DirectoryrResponse Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<DirectoryrResponse>();
			}
			return _instance;
		}
	}
}
