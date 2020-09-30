﻿// Copyright (c) 2018-2019 Alexander Bogarsukov.
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

		private class TaskUtilityData
		{
			public SynchronizationContext MainThreadContext;
			public TaskUtilityBehaviour RootBehaviour;
			public SendOrPostCallback StartCoroutineCallback;
			public SendOrPostCallback StopCoroutineCallback;
			public SendOrPostCallback StopAllCoroutinesCallback;
		}

		private static TaskUtilityData _data = new TaskUtilityData();

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the current thread is Unity main thread.
		/// </summary>
		public static bool IsUnityThread
		{
			get
			{
				return SynchronizationContext.Current == _data.MainThreadContext;
			}
		}

		/// <summary>
		/// Awaits Unity thread.
		/// </summary>
		public static CompilerServices.UnityThreadAwaitable YieldToUnityThread()
		{
			return new CompilerServices.UnityThreadAwaitable(_data.MainThreadContext);
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
			if (sceneName is null)
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
					cancellationToken.Register(() =>
					{
						result.TrySetCanceled(cancellationToken);
					});
				}

				op.completed += o =>
				{
					var scene = default(Scene);

					// NOTE: Grab the last scene with the specified name from the list of loaded scenes.
					for (var i = SceneManager.sceneCount - 1; i >= 0; --i)
					{
						var s = SceneManager.GetSceneAt(i);

						if (string.CompareOrdinal(s.name, sceneName) == 0)
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
				return Task.FromException<Scene>(new InvalidOperationException($"Cannot load scene '{sceneName}'. Please make sure it has been added to the build settings or the source {nameof(AssetBundle)} has been loaded successfully."));
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
						_data.RootBehaviour.StopCoroutine(enumOp);
					}
				},
				true);
			}

			_data.RootBehaviour.StartCoroutine(enumOp);
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

			if (SynchronizationContext.Current == _data.MainThreadContext)
			{
				if (_data.RootBehaviour)
				{
					_data.RootBehaviour.StartCoroutine(enumerator);
				}
			}
			else
			{
				if (_data.StartCoroutineCallback == null)
				{
					_data.StartCoroutineCallback = state =>
					{
						if (_data.RootBehaviour)
						{
							_data.RootBehaviour.StartCoroutine(state as IEnumerator);
						}
					};
				}

				_data.MainThreadContext.Post(_data.StartCoroutineCallback, enumerator);
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
			if (enumerator != null && _data.RootBehaviour)
			{
				if (SynchronizationContext.Current == _data.MainThreadContext)
				{
					_data.RootBehaviour.StopCoroutine(enumerator);
				}
				else
				{
					if (_data.StopCoroutineCallback == null)
					{
						_data.StopCoroutineCallback = state => _data.RootBehaviour.StopCoroutine(state as IEnumerator);
					}

					_data.MainThreadContext.Post(_data.StopCoroutineCallback, enumerator);
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
			if (_data.RootBehaviour)
			{
				if (SynchronizationContext.Current == _data.MainThreadContext)
				{
					_data.RootBehaviour.StopAllCoroutines();
				}
				else
				{
					if (_data.StopAllCoroutinesCallback == null)
					{
						_data.StopAllCoroutinesCallback = state => _data.RootBehaviour.StopAllCoroutines();
					}

					_data.MainThreadContext.Post(_data.StopAllCoroutinesCallback, null);
				}
			}
		}

		internal static void Initialize(GameObject go, SynchronizationContext mainThreadContext)
		{
			Debug.Assert(ReferenceEquals(_data.RootBehaviour, null));

			// NOTE: Should only be called once.
			_data.MainThreadContext = mainThreadContext;
			_data.RootBehaviour = go.AddComponent<TaskUtilityBehaviour>();
		}

		internal static void AddCompletionCallback(UnityWebRequest request, Action action)
		{
			_data.RootBehaviour.AddCompletionCallback(request, action);
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
