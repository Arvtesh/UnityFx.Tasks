// Copyright (c) 2018-2019 Alexander Bogarsukov.
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

		private static SynchronizationContext _unityThreadContext;
		private static TaskUtilityBehaviour _workerBehaviour;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the current thread is Unity main thread.
		/// </summary>
		public static bool IsUnityThread => SynchronizationContext.Current == _unityThreadContext;

		/// <summary>
		/// Awaits Unity thread.
		/// </summary>
		public static CompilerServices.UnityThreadAwaitable YieldToUnityThread()
		{
			return new CompilerServices.UnityThreadAwaitable(_unityThreadContext);
		}

		/// <summary>
		/// Delays execution for the specified <paramref name="delay"/>.
		/// </summary>
		public static CompilerServices.TimeDelayAwaitable Delay(TimeSpan delay, TimerType timerType = TimerType.Scaled)
		{
			var doubleSeconds = delay.TotalSeconds;

			if (doubleSeconds > float.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(delay), "Delay value is too large.");
			}
			else if (doubleSeconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(delay), "Delay cannot be negative.");
			}

			return new CompilerServices.TimeDelayAwaitable((float)delay.TotalSeconds, timerType);
		}

		/// <summary>
		/// Delays execution for the specified number of seconds.
		/// </summary>
		public static CompilerServices.TimeDelayAwaitable DelaySeconds(float delaySeconds, TimerType timerType = TimerType.Scaled)
		{
			if (delaySeconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(delaySeconds), "Delay cannot be negative.");
			}

			return new CompilerServices.TimeDelayAwaitable(delaySeconds, timerType);
		}

		/// <summary>
		/// Delays execution for the specified number of frames.
		/// </summary>
		public static CompilerServices.SkipFramesAwaitable DelayFrames(int delayFrames)
		{
			if (delayFrames < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(delayFrames), "Delay cannot be negative.");
			}

			return new CompilerServices.SkipFramesAwaitable(delayFrames);
		}

		/// <summary>
		/// Delays execution for one frame.
		/// </summary>
		public static CompilerServices.SkipFramesAwaitable DelayFrame()
		{
			return new CompilerServices.SkipFramesAwaitable(1);
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
				var tcs = new TaskCompletionSource<Scene>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						tcs.TrySetCanceled(cancellationToken);
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
						if (!tcs.TrySetResult(scene))
						{
							SceneManager.UnloadSceneAsync(scene);
						}
					}
					else
					{
						tcs.TrySetException(new UnityAssetLoadException(sceneName, typeof(Scene)));
					}
				};

				return tcs.Task;
			}
			else
			{
				return Task.FromException<Scene>(new InvalidOperationException($"Cannot load scene '{sceneName}'. Please make sure it has been added to the build settings or the source {nameof(AssetBundle)} has been loaded successfully."));
			}
		}

		/// <summary>
		/// Starts a coroutine.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerator"/> is <see langword="null"/>.</exception>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static Coroutine StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator is null)
			{
				throw new ArgumentNullException(nameof(enumerator));
			}

			return GetUtilityBehaviour().StartCoroutine(enumerator);
		}

		/// <summary>
		/// Stops the specified coroutine. Can be called from non-Unity thread.
		/// </summary>
		/// <param name="enumerator">The coroutine to stop.</param>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StopCoroutine(Coroutine coroutine)
		{
			if (_workerBehaviour && coroutine != null)
			{
				_workerBehaviour.StopCoroutine(coroutine);
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
			if (_workerBehaviour && enumerator != null)
			{
				_workerBehaviour.StopCoroutine(enumerator);
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
			if (_workerBehaviour)
			{
				_workerBehaviour.StopAllCoroutines();
			}
		}

		internal static void Initialize(SynchronizationContext mainThreadContext)
		{
			Debug.Assert(_unityThreadContext is null);

			// NOTE: Should only be called once.
			_unityThreadContext = mainThreadContext;
		}

		internal static void Schedule(Action action, int numberOfFramesToSkip)
		{
			Debug.Assert(action != null);

			if (numberOfFramesToSkip > 0)
			{
				GetUtilityBehaviour().AddCompletionCallback(action, numberOfFramesToSkip);
			}
		}

		internal static void Schedule(Action action, float delay, TimerType timerType)
		{
			Debug.Assert(action != null);

			if (delay > 0)
			{
				GetUtilityBehaviour().AddCompletionCallback(action, delay, timerType);
			}
		}

		#endregion

		#region implementation

		private sealed class TaskUtilityBehaviour : MonoBehaviour
		{
			private class TimerData
			{
				public TimerType TimerType;
				public float Timer;
				public float StartTime;
				public Action Callback;
			}

			private Dictionary<int, List<Action>> _skipFramesMap;
			private List<TimerData> _timers;

			public void AddCompletionCallback(Action action, int numberOfFramesToSkip)
			{
				var targetFrame = Time.frameCount + numberOfFramesToSkip;

				if (_skipFramesMap is null)
				{
					_skipFramesMap = new Dictionary<int, List<Action>>();
				}

				if (_skipFramesMap.TryGetValue(targetFrame, out var actions))
				{
					actions.Add(action);
				}
				else
				{
					_skipFramesMap.Add(targetFrame, new List<Action>() { action });
				}
			}

			public void AddCompletionCallback(Action action, float delay, TimerType timerType)
			{
				if (_timers is null)
				{
					_timers = new List<TimerData>();
				}

				_timers.Add(new TimerData()
				{
					TimerType = timerType,
					Timer = delay,
					StartTime = timerType == TimerType.Realtime ? Time.realtimeSinceStartup : 0,
					Callback = action
				});
			}

			private void Update()
			{
				if (_skipFramesMap != null)
				{
					var frameNumber = Time.frameCount;

					if (_skipFramesMap.TryGetValue(frameNumber, out var actions))
					{
						_skipFramesMap.Remove(frameNumber);

						foreach (var action in actions)
						{
							try
							{
								action();
							}
							catch (Exception e)
							{
								Debug.LogException(e);
							}
						}
					}
				}

				if (_timers != null)
				{
					var i = 0;

					while (i < _timers.Count)
					{
						var timer = _timers[i];
						var timerEnd = false;

						if (timer.TimerType == TimerType.Realtime)
						{
							if (Time.realtimeSinceStartup - timer.StartTime > timer.Timer)
							{
								timerEnd = true;
							}
						}
						else
						{
							if (timer.TimerType == TimerType.Scaled)
							{
								timer.Timer -= Time.deltaTime;
							}
							else
							{
								timer.Timer -= Time.unscaledDeltaTime;
							}

							if (timer.Timer <= 0)
							{
								timerEnd = true;
							}
						}

						if (timerEnd)
						{
							_timers.RemoveAt(i);

							try
							{
								timer.Callback();
							}
							catch (Exception e)
							{
								Debug.LogException(e);
							}
						}
						else
						{
							++i;
						}
					}
				}
			}
		}

		private static TaskUtilityBehaviour GetUtilityBehaviour()
		{
			if (_workerBehaviour == null)
			{
				var go = new GameObject("UnityFx.Tasks")
				{
					hideFlags = HideFlags.HideAndDontSave
				};

				GameObject.DontDestroyOnLoad(go);

				_workerBehaviour = go.AddComponent<TaskUtilityBehaviour>();
			}

			return _workerBehaviour;
		}

		#endregion
	}
}
