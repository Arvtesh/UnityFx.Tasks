// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
		/// Starts a coroutine. Can be called from non-Unity thread.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		/// <returns>Returns the coroutine handle.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerator"/> is <see langword="null"/>.</exception>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				throw new ArgumentNullException("enumerator");
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

		#endregion

		#region implementation

		private sealed class TaskUtilityBehaviour : MonoBehaviour
		{
			private Helpers.UnitySynchronizationContext _context;

			private void Awake()
			{
				_context = _mainThreadContext as Helpers.UnitySynchronizationContext;
			}

			private void Update()
			{
				_context?.Update(this);
			}
		}

		#endregion
	}
}
