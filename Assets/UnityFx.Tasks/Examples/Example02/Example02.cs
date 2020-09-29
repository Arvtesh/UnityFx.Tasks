// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.Examples
{
	/// <summary>
	/// The sample demonstrates async/await usage with UnityWebRequest.
	/// </summary>
	public class Example02 : MonoBehaviour
	{
		private async void Start()
		{
			try
			{
				using (var www = UnityWebRequest.Get("google.com"))
				{
					var text = await www.SendWebRequestAsync<string>();
					Debug.Log(text);
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e, this);
			}
		}
	}
}
