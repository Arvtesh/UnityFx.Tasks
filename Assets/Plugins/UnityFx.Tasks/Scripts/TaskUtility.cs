// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Utility methods.
	/// </summary>
	public static class TaskUtility
	{
		#region data

		private static SynchronizationContext _mainThreadContext;
		private static TaskUtilityBehaviour _rootBehaviour;
		private static SendOrPostCallback _startCoroutineCallback;
		private static SendOrPostCallback _stopCoroutineCallback;
		private static SendOrPostCallback _stopAllCoroutinesCallback;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the current thread is Unity main thread.
		/// </summary>
		public static bool IsUnityThread
		{
			get
			{
				return SynchronizationContext.Current == _mainThreadContext;
			}
		}

		/// <summary>
		/// Awaits Unity thread.
		/// </summary>
		public static CompilerServices.UnityThreadAwaitable YieldToUnityThread()
		{
			return new CompilerServices.UnityThreadAwaitable(_mainThreadContext);
		}

		/// <summary>
		/// Asynchronously loads an asset with the specified name from <see cref="Resources"/>.
		/// </summary>
		/// <param name="assetPath">Path the asset in the <see cref="Resources"/> folder.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetPath"/> is <see langword="null"/>.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetAsync(string, CancellationToken)"/>
		public static Task<UnityEngine.Object> LoadAssetAsync(string assetPath)
		{
			return LoadAssetAsync(assetPath, CancellationToken.None);
		}

		/// <summary>
		/// Asynchronously loads an asset with the specified name from <see cref="Resources"/>.
		/// </summary>
		/// <param name="assetPath">Path the asset in the <see cref="Resources"/> folder.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetPath"/> is <see langword="null"/>.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetAsync(string)"/>
		public static Task<UnityEngine.Object> LoadAssetAsync(string assetPath, CancellationToken cancellationToken)
		{
			if (assetPath == null)
			{
				throw new ArgumentNullException(nameof(assetPath));
			}

			var op = Resources.LoadAsync(assetPath);

			if (op == null)
			{
				return Task.FromException<UnityEngine.Object>(new InvalidOperationException($"Cannot load asset '{assetPath}' from resources."));
			}

			return op.ToTask(cancellationToken);
		}

		/// <summary>
		/// Asynchronously loads an asset with the specified name and type from <see cref="Resources"/>.
		/// </summary>
		/// <param name="assetPath">Path the asset in the <see cref="Resources"/> folder.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetPath"/> is <see langword="null"/>.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetAsync{T}(string, CancellationToken)"/>
		public static Task<T> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
		{
			return LoadAssetAsync<T>(assetPath, CancellationToken.None);
		}

		/// <summary>
		/// Asynchronously loads an asset with the specified name and type from <see cref="Resources"/>.
		/// </summary>
		/// <param name="assetPath">Path the asset in the <see cref="Resources"/> folder.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetPath"/> is <see langword="null"/>.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadAssetAsync{T}(string)"/>
		public static Task<T> LoadAssetAsync<T>(string assetPath, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			if (assetPath == null)
			{
				throw new ArgumentNullException(nameof(assetPath));
			}

			var op = Resources.LoadAsync(assetPath, typeof(T));

			if (op == null)
			{
				return Task.FromException<T>(new InvalidOperationException($"Cannot load asset '{assetPath}' from resources."));
			}

			return op.ToTask<T>(cancellationToken);
		}

		/// <summary>
		/// Asynchronously loads a scene with the specified name.
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="sceneName"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the scene hasn't been added to the build settings or the source AssetBundle hasn't been loaded.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneAsync(string, LoadSceneMode, CancellationToken)"/>
		public static Task<Scene> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			return LoadSceneAsync(sceneName, loadMode, CancellationToken.None);
		}

		/// <summary>
		/// Asynchronously loads a scene with the specified name.
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="loadMode">The scene load mode.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="sceneName"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the scene hasn't been added to the build settings or the source AssetBundle hasn't been loaded.</exception>
		/// <returns>A <see cref="Task{TResult}"/> that can be used to track the operation state.</returns>
		/// <seealso cref="LoadSceneAsync(string, LoadSceneMode)"/>
		public static Task<Scene> LoadSceneAsync(string sceneName, LoadSceneMode loadMode, CancellationToken cancellationToken)
		{
			if (sceneName == null)
			{
				throw new ArgumentNullException(nameof(sceneName));
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<Scene>(cancellationToken);
			}

			var op = SceneManager.LoadSceneAsync(sceneName, loadMode);

			// NOTE: If the scene cannot be loaded, result of the LoadSceneAsync is null.
			if (op != null)
			{
				var result = new TaskCompletionSource<Scene>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => result.TrySetCanceled(cancellationToken));
				}

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
						// NOTE: TrySetResult() failure probably means that the operation was cancelled, thus the scene should be unloaded.
						if (!result.TrySetResult(scene))
						{
							SceneManager.UnloadSceneAsync(scene);
						}
					}
					else
					{
						result.TrySetException(new UnityAssetLoadException(sceneName, typeof(Scene)));
					}
				};

				return result.Task;
			}
			else
			{
				return Task.FromException<Scene>(new InvalidOperationException($"Cannot load scene '{sceneName}'. Please make sure it has been added to the build settings or the {nameof(AssetBundle)} has been loaded successfully."));
			}
		}

		/// <summary>
		/// Starts a coroutine and wraps it with <see cref="Task{TResult}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the coroutine result value.</typeparam>
		/// <param name="coroutineFunc">The coroutine delegate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="coroutineFunc"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the scene hasn't been added to the build settings or the asset bundle hasn't been loaded.</exception>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the coroutine state.</returns>
		/// <seealso cref="FromCoroutine(Func{IAsyncCompletionSource, IEnumerator}, object)"/>
		public static Task<T> FromCoroutine<T>(Func<TaskCompletionSource<T>, IEnumerator> coroutineFunc)
		{
			return FromCoroutine(coroutineFunc, CancellationToken.None);
		}

		/// <summary>
		/// Starts a coroutine and wraps it with <see cref="Task{TResult}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the coroutine result value.</typeparam>
		/// <param name="coroutineFunc">The coroutine delegate.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="coroutineFunc"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the scene hasn't been added to the build settings or the asset bundle hasn't been loaded.</exception>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the coroutine state.</returns>
		/// <seealso cref="FromCoroutine(Func{IAsyncCompletionSource, IEnumerator}, object)"/>
		public static Task<T> FromCoroutine<T>(Func<TaskCompletionSource<T>, IEnumerator> coroutineFunc, CancellationToken cancellationToken)
		{
			if (coroutineFunc == null)
			{
				throw new ArgumentNullException(nameof(coroutineFunc));
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<T>(cancellationToken);
			}

			var result = new TaskCompletionSource<T>();
			var enumOp = CoroutineWrapperEnum(coroutineFunc(result), result);

			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(() =>
				{
					if (result.TrySetCanceled(cancellationToken))
					{
						_rootBehaviour.StopCoroutine(enumOp);
					}
				},
				true);
			}

			_rootBehaviour.StartCoroutine(enumOp);
			return result.Task;
		}

		/// <summary>
		/// Starts a coroutine. Can be called from non-Unity thread.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerator"/> is <see langword="null"/>.</exception>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				throw new ArgumentNullException(nameof(enumerator));
			}

			if (SynchronizationContext.Current == _mainThreadContext)
			{
				_rootBehaviour.StartCoroutine(enumerator);
			}
			else
			{
				if (_startCoroutineCallback == null)
				{
					_startCoroutineCallback = state => _rootBehaviour.StartCoroutine(state as IEnumerator);
				}

				_mainThreadContext.Post(_startCoroutineCallback, enumerator);
			}
		}

		/// <summary>
		/// Stops the specified coroutine. Can be called from non-Unity thread.
		/// </summary>
		/// <param name="enumerator">The coroutine to stop.</param>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StopCoroutine(IEnumerator enumerator)
		{
			if (enumerator != null && _rootBehaviour)
			{
				if (SynchronizationContext.Current == _mainThreadContext)
				{
					_rootBehaviour.StopCoroutine(enumerator);
				}
				else
				{
					if (_stopCoroutineCallback == null)
					{
						_stopCoroutineCallback = state => _rootBehaviour.StopCoroutine(state as IEnumerator);
					}

					_mainThreadContext.Post(_stopCoroutineCallback, enumerator);
				}
			}
		}

		/// <summary>
		/// Stops all coroutines. Can be called from non-Unity thread.
		/// </summary>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		public static void StopAllCoroutines()
		{
			if (_rootBehaviour)
			{
				if (SynchronizationContext.Current == _mainThreadContext)
				{
					_rootBehaviour.StopAllCoroutines();
				}
				else
				{
					if (_stopAllCoroutinesCallback == null)
					{
						_stopAllCoroutinesCallback = state => _rootBehaviour.StopAllCoroutines();
					}

					_mainThreadContext.Post(_stopAllCoroutinesCallback, null);
				}
			}
		}

		internal static void Initialize(GameObject go, SynchronizationContext mainThreadContext)
		{
			// NOTE: Should only be called once.
			if (_rootBehaviour)
			{
				throw new InvalidOperationException();
			}

			_mainThreadContext = mainThreadContext;
			_rootBehaviour = go.AddComponent<TaskUtilityBehaviour>();
		}

		internal static void AddCompletionCallback(UnityWebRequest request, Action action)
		{
			_rootBehaviour.AddCompletionCallback(request, action);
		}

		#endregion

		#region implementation

		private sealed class TaskUtilityBehaviour : MonoBehaviour
		{
			private Dictionary<UnityWebRequest, Action> _ops;
			private List<KeyValuePair<UnityWebRequest, Action>> _opsToRemove;

			public void AddCompletionCallback(UnityWebRequest op, Action cb)
			{
				if (_ops == null)
				{
					_ops = new Dictionary<UnityWebRequest, Action>();
					_opsToRemove = new List<KeyValuePair<UnityWebRequest, Action>>();
				}

				_ops.Add(op, cb);
			}

			private void Update()
			{
				if (_ops != null && _ops.Count > 0)
				{
					foreach (var item in _ops)
					{
						if (item.Key.isDone)
						{
							_opsToRemove.Add(item);
						}
					}

					foreach (var item in _opsToRemove)
					{
						_ops.Remove(item.Key);

						try
						{
							item.Value();
						}
						catch (Exception e)
						{
							Debug.LogException(e, this);
						}
					}

					_opsToRemove.Clear();
				}
			}
		}

		private static IEnumerator CoroutineWrapperEnum<T>(IEnumerator workerEnum, TaskCompletionSource<T> tcs)
		{
			yield return workerEnum;
			tcs.TrySetResult(default(T));
		}

		#endregion
	}
}
