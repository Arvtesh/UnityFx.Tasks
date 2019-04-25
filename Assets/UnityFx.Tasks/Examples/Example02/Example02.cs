// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
					var op = www.SendWebRequest();

					// Await op completion.
					await op;

					// Can await www as well; if www.SendWebRequest() hasn't been called the await would never complete.
					await www;

					// Await op completion and get text result (can do the same with www).
					var text = await op.ConfigureAwait<string>();
					Debug.Log(text);
				}

				using (var www = UnityWebRequest.Get("yahoo.com"))
				{
					// Another way to await a web request is convert it to a Task instance. Note that SendWebRequest() is called automatically in this case.
					var task = www.ToTask<string>();
					var text = await task;
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
