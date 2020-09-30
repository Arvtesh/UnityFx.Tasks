// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Extensions of <see cref="AssetBundle"/> and related.
	/// </summary>
	public static class AssetBundleExtensions
	{
		/// <summary>
		/// Loads a <see cref="Scene"/> from an asset bundle.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneAsync(AssetBundle, LoadSceneMode, CancellationToken)"/>
		public static Task<Scene> LoadSceneAsync(this AssetBundle assetBundle, LoadSceneMode loadMode)
		{
			return LoadSceneAsync(assetBundle, loadMode, CancellationToken.None);
		}

		/// <summary>
		/// Loads a <see cref="Scene"/> from an asset bundle.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneAsync(AssetBundle, LoadSceneMode)"/>
		public static Task<Scene> LoadSceneAsync(this AssetBundle assetBundle, LoadSceneMode loadMode, CancellationToken cancellationToken)
		{
			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				throw new InvalidOperationException();
			}

			var scenePaths = assetBundle.GetAllScenePaths();

			if (scenePaths != null && scenePaths.Length > 0 && !string.IsNullOrEmpty(scenePaths[0]))
			{
				var sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
				return TaskUtility.LoadSceneAsync(sceneName, loadMode, cancellationToken);
			}
			else
			{
				throw new UnityAssetLoadException("The asset bundle does not contain scenes.", null, typeof(Scene));
			}
		}
	}
}
