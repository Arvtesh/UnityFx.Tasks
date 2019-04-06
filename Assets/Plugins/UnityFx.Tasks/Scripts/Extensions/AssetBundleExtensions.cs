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
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <param name="op">The operation to wrap.</param>
		public static Task<T> ToTask<T>(this AssetBundleRequest op) where T : UnityEngine.Object
		{
			return ToTask<T>(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <param name="op">The operation to wrap.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		public static Task<T> ToTask<T>(this AssetBundleRequest op, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			if (cancellationToken.IsCancellationRequested)
			{
				if (op.isDone)
				{
					DestroyAssets(op);
				}
				else
				{
					op.completed += OnCancelledOperationCompleted;
				}

				return Task.FromCanceled<T>(cancellationToken);
			}

			if (op.isDone)
			{
				if (op.asset is T)
				{
					return Task.FromResult(op.asset as T);
				}
				else
				{
					return Task.FromException<T>(new InvalidCastException());
				}
			}
			else
			{
				var result = new TaskCompletionSource<T>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						if (result.TrySetCanceled(cancellationToken))
						{
							op.completed += OnCancelledOperationCompleted;
						}
					},
					true);
				}

				op.completed += o =>
				{
					if (op.asset is T)
					{
						result.TrySetResult(op.asset as T);
					}
					else
					{
						result.TrySetException(new InvalidCastException());
					}
				};

				return result.Task;
			}
		}

		/// <summary>
		/// Loads a <see cref="Scene"/> from an asset bundle.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <param name="sceneName">Name of the scene to load or <see langword="null"/> to load the any scene.</param>
		/// <param name="userState">User-defined data to pass to the resulting <see cref="Task{TResult}"/>.</param>
		public static Task<Scene> LoadSceneTaskAsync(this AssetBundle assetBundle, LoadSceneMode loadMode, string sceneName = null, object userState = null)
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

			var result = new TaskCompletionSource<Scene>(userState);
			var op = SceneManager.LoadSceneAsync(sceneName, loadMode);

			op.completed += o =>
			{
				var scene = default(Scene);

				// NOTE: Grab the last scene with the specified name from the list of loaded scenes.
				for (var i = SceneManager.sceneCount - 1; i >= 0; --i)
				{
					var s = SceneManager.GetSceneAt(i);

					if (s.name == sceneName)
					{
						scene = s;
						break;
					}
				}

				if (scene.isLoaded)
				{
					result.TrySetResult(scene);
				}
				else
				{
					result.TrySetException(new UnityAssetLoadException(sceneName, typeof(Scene)));
				}
			};

			return result.Task;
		}

		/// <summary>
		/// Loads an asset from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="name">Name of the asset to load.</param>
		public static Task<T> LoadAssetTaskAsync<T>(this AssetBundle assetBundle, string name) where T : UnityEngine.Object
		{
			return assetBundle.LoadAssetAsync(name, typeof(T)).ToTask<T>();
		}

		/// <summary>
		/// Loads an asset from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <param name="assetBundle">The source asset bundle.</param>
		/// <param name="name">Name of the asset to load.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		public static Task<T> LoadAssetTaskAsync<T>(this AssetBundle assetBundle, string name, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			return assetBundle.LoadAssetAsync(name, typeof(T)).ToTask<T>(cancellationToken);
		}

		#endregion

		#region implementation

		private static void OnCancelledOperationCompleted(AsyncOperation op)
		{
			if (op is AssetBundleRequest)
			{
				DestroyAssets(op as AssetBundleRequest);
			}
		}

		private static void DestroyAssets(AssetBundleRequest op)
		{
			if (op.allAssets != null)
			{
				foreach (var asset in op.allAssets)
				{
					UnityEngine.Object.Destroy(asset);
				}
			}
			else if (op.asset)
			{
				UnityEngine.Object.Destroy(op.asset);
			}
		}

		#endregion
	}
}
