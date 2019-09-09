// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks.Examples
{
	/// <summary>
	/// The sample demonstrates async/await usage with Animator.
	/// </summary>
	public class Example03 : MonoBehaviour
	{
		[SerializeField]
		private Animator _animator;

		private async void Start()
		{
			try
			{
				// Using ConfigureAwait extensin method one can await for a completion of an animator state.
				Debug.Log("Animation started");
				await _animator.ConfigureAwait("Bounce");
				Debug.Log("Animation completed");
			}
			catch (Exception e)
			{
				Debug.LogException(e, this);
			}
		}
	}
}
