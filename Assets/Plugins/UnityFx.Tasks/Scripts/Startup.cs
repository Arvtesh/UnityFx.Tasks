// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

#if !UNITY_2017_2_OR_NEWER
#error UnityFx.Tasks requires Unity 2017.2 or newer.
#endif

#if NET_LEGACY || NET_2_0 || NET_2_0_SUBSET
#error UnityFx.Tasks does not support .NET 3.5. Please set Scripting Runtime Version to .NET 4.x Equivalent in Unity Player Settings.
#endif

using System;
using System.Threading;
using UnityEngine;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Manages initialization of library services. For internal use only.
	/// </summary>
	internal static class Startup
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			var context = SynchronizationContext.Current;

			if (context == null)
			{
				// Create custom SynchronizationContext for the main thread.
				context = new Helpers.UnitySynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(context);
			}

			// Create helper GameObject
			var go = new GameObject("UnityFx.Tasks")
			{
				hideFlags = HideFlags.HideAndDontSave
			};

			GameObject.DontDestroyOnLoad(go);

			// Initialize library components.
			TaskUtility.Initialize(go, context);
		}
	}
}
