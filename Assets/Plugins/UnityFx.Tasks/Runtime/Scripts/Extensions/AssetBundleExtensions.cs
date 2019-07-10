// Copyright (c) Alexander Bogarsukov.
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
		#region interface

		/// <summary>
		/// Loads a <see cref="Scene"/> from an asset bundle.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <param name="sceneName">Name of the scene to load or <see langword="null"/> to load the any scene.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneTaskAsync(AssetBundle, LoadSceneMode, string, CancellationToken)"/>
		public static Task<Scene> LoadSceneTaskAsync(this AssetBundle assetBundle, LoadSceneMode loadMode, string sceneName = null)
		{
			return LoadSceneTaskAsync(assetBundle, loadMode, sceneName, CancellationToken.None);
		}

		/// <summary>
		/// Loads a <see cref="Scene"/> from an asset bundle.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <param name="sceneName">Name of the scene to load or <see langword="null"/> to load the any scene.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneTaskAsync(AssetBundle, LoadSceneMode, string)"/>
		public static Task<Scene> LoadSceneTaskAsync(this AssetBundle assetBundle, LoadSceneMode loadMode, string sceneName, CancellationToken cancellationToken)
		{
			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				throw new InvalidOperationException();
			}

			if (string.IsNullOrEmpty(sceneName))
			{
				var scenePaths = assetBundle.GetAllScenePaths();

				if (scenePaths != null && scenePaths.Length > 0 && !string.IsNullOrEmpty(scenePaths[0]))
				{
					sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
				}

				if (string.IsNullOrEmpty(sceneName))
				{
					throw new UnityAssetLoadException("The asset bundle does not contain scenes.", null, typeof(Scene));
				}
			}

			return TaskUtility.LoadSceneAsync(sceneName, loadMode, cancellationToken);
		}

		/// <summary>
		/// Loads an asset from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <typeparam name="T">Type of the asset to load.</typeparam>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="name">Name of the asset to load.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetTaskAsync{T}(AssetBundle, string, CancellationToken)"/>
		public static Task<T> LoadAssetTaskAsync<T>(this AssetBundle assetBundle, string name) where T : UnityEngine.Object
		{
			return LoadAssetTaskAsync<T>(assetBundle, name, CancellationToken.None);
		}

		/// <summary>
		/// Loads an asset from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <typeparam name="T">Type of the asset to load.</typeparam>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="name">Name of the asset to load.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetTaskAsync{T}(AssetBundle, string)"/>
		public static Task<T> LoadAssetTaskAsync<T>(this AssetBundle assetBundle, string name, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			return assetBundle.LoadAssetAsync(name, typeof(T)).ToTask<T>(cancellationToken);
		}

		/// <summary>
		/// Loads all assets of the specified type from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <typeparam name="T">Type of the assets to load.</typeparam>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAllAssetsTaskAsync{T}(AssetBundle, CancellationToken)"/>
		public static Task<T[]> LoadAllAssetsTaskAsync<T>(this AssetBundle assetBundle) where T : UnityEngine.Object
		{
			return LoadAllAssetsTaskAsync<T>(assetBundle, CancellationToken.None);
		}

		/// <summary>
		/// Loads all assets of the specified type from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <typeparam name="T">Type of the assets to load.</typeparam>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>Returns the <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAllAssetsTaskAsync{T}(AssetBundle)"/>
		public static Task<T[]> LoadAllAssetsTaskAsync<T>(this AssetBundle assetBundle, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			return assetBundle.LoadAllAssetsAsync(typeof(T)).ToTask<T[]>(cancellationToken);
		}

		#endregion

		#region implementation
		#endregion
	}
}
