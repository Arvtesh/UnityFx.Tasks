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
				// Any YieldInstruction can be awaited, including WaitForSeconds, WaitUntil etc.
				Debug.Log("Time: " + Time.realtimeSinceStartup);
				await new WaitForSeconds(1);
				Debug.Log("Time after new WaitForSeconds(1): " + Time.realtimeSinceStartup);

				// Coroutines can be awaited as well.
				await StartCoroutine(TestCoroutine());
				Debug.Log("After await TestCoroutine");

				// One can easily switch context to Unity thread using 'await TaskUtility.YieldToUnityThread()'.
				await Task.Run(async () =>
				{
					Debug.Log("IsUnityThread: " + TaskUtility.IsUnityThread);
					await TaskUtility.YieldToUnityThread();
					Debug.Log("IsUnityThread after YieldToUnityThread: " + TaskUtility.IsUnityThread);
				});

				// Any AsyncOperation can be awaited.
				var textAsset = (TextAsset)await Resources.LoadAsync("Test", typeof(TextAsset));
				Debug.Log(textAsset.text);

				// There are a number of helper methods in TaskUtility. For example this one loads an asset from resources.
				var textAsset2 = await TaskUtility.LoadAssetAsync<TextAsset>("Test");
				Debug.Log(textAsset2.text);

				// Another useful helper is for loading scenes.
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
