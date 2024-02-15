using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ModelDownloader : MonoBehaviour
{
	public string Email;
	public string Password;
	public string ModeIUIDToDownload;

	private void Start()
	{
		SketchfabAPI.GetAccessToken(Email, Password, (SketchfabResponse<SketchfabAccessToken> answer) =>
		{
			if (answer.Success)
			{
				//AccessToken = answer.Object.AccessToken;
				SketchfabAPI.AuthorizeWithAccessToken(answer.Object);

				DownloadModel();
			}
			else
			{
				Debug.LogError(answer.ErrorMessage);
			}

		});
	}

	void DownloadModel()
	{
		// This first call will get the model information
		SketchfabAPI.GetModel(ModeIUIDToDownload, (resp) =>
		{
			// This second call will get the model information, download it and instantiate it
			SketchfabModelImporter.Import(resp.Object, (obj) =>
			{
				if (obj != null)
				{
					// Here you can do anything you like to obj (A unity game object containing the sketchfab model)
				}
			});
		});
	}
}