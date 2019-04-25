// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks.Examples
{
	/// <summary>
	/// The sample demonstrates basic async/await usage in Unity.
	/// </summary>
	public class Example01 : MonoBehaviour
	{
		private async void Start()
		{
			try
			{
				// await new WaitForSeconds()
				Debug.Log("Time: " + Time.realtimeSinceStartup);
				await new WaitForSeconds(1);
				Debug.Log("Time after new WaitForSeconds(1): " + Time.realtimeSinceStartup);

				// await StartCoroutine()
				await StartCoroutine(TestCoroutine());
				Debug.Log("After await TestCoroutine");

				// await TaskUtility.YieldToUnityThread()
				await Task.Run(async () =>
				{
					Debug.Log("IsUnityThread: " + TaskUtility.IsUnityThread);
					await TaskUtility.YieldToUnityThread();
					Debug.Log("IsUnityThread after YieldToUnityThread: " + TaskUtility.IsUnityThread);
				});

				// await Resources.LoadAsync
				var textAsset = (TextAsset)await Resources.LoadAsync("Test", typeof(TextAsset));
				Debug.Log(textAsset.text);

				// await TaskUtility.LoadAssetAsync
				var textAsset2 = await TaskUtility.LoadAssetAsync<TextAsset>("Test");
				Debug.Log(textAsset2.text);

				// await LoadSceneAsync
				var scene = await TaskUtility.LoadSceneAsync("TestScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
				Debug.Log("TestScene.isLoaded: " + scene.isLoaded);
			}
			catch (Exception e)
			{
				Debug.LogException(e, this);
			}
		}

		private IEnumerator TestCoroutine()
		{
			Debug.Log("TestCoroutine");
			yield return null;
			Debug.Log("TestCoroutine end");
		}
	}
}
