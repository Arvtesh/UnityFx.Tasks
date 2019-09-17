// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Extensions of <see cref="UnityWebRequest"/>.
	/// </summary>
	public static class UnityWebRequestExtensions
	{
		#region interface

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(UnityWebRequest, CancellationToken)"/>
		public static Task ToTask(this UnityWebRequest request)
		{
			return ToTask(request, CancellationToken.None);
		}

		/// <summary>
		/// Creates a cancellable <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(UnityWebRequest)"/>
		public static Task ToTask(this UnityWebRequest request, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				request.Abort();
				return Task.FromCanceled(cancellationToken);
			}

			if (request.isDone)
			{
				// NOTE: Short path for completed requests.
				if (request.isHttpError || request.isNetworkError)
				{
					return Task.FromException(new UnityWebRequestException(request.error, request.responseCode));
				}
				else
				{
					return Task.CompletedTask;
				}
			}
			else
			{
				var result = new TaskCompletionSource<object>(request);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						if (result.TrySetCanceled(cancellationToken))
						{
							request.Abort();
						}
					},
					true);
				}

				if (request.isModifiable)
				{
					request.SendWebRequest().completed += op => OnTaskCompleted(result, request, null);
				}
				else
				{
					TaskUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request, null));
				}

				return result.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(UnityWebRequest, CancellationToken)"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request) where T : class
		{
			return ToTask<T>(request, null, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <param name="resultDelegate">An optional delegate that is used to get the request result.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(UnityWebRequest, CancellationToken)"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate) where T : class
		{
			return ToTask<T>(request, resultDelegate, CancellationToken.None);
		}

		/// <summary>
		/// Creates a cancellable <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <param name="resultDelegate">An optional delegate that is used to get the request result.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(UnityWebRequest)"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate, CancellationToken cancellationToken) where T : class
		{
			if (cancellationToken.IsCancellationRequested)
			{
				request.Abort();
				return Task.FromCanceled<T>(cancellationToken);
			}

			if (request.isDone)
			{
				// NOTE: Short path for completed requests.
				if (request.isHttpError || request.isNetworkError)
				{
					return Task.FromException<T>(new UnityWebRequestException(request.error, request.responseCode));
				}
				else if (resultDelegate != null)
				{
					return Task.FromResult(resultDelegate(request));
				}
				else
				{
					return Task.FromResult(GetResultInternal<T>(request));
				}
			}
			else
			{
				var result = new TaskCompletionSource<T>(request);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						if (result.TrySetCanceled(cancellationToken))
						{
							request.Abort();
						}
					},
					true);
				}

				if (request.isModifiable)
				{
					request.SendWebRequest().completed += op => OnTaskCompleted(result, request, resultDelegate);
				}
				else
				{
					TaskUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request, resultDelegate));
				}

				return result.Task;
			}
		}

		internal static UnityWebRequest ValidateResultType<T>(this UnityWebRequest request) where T : class
		{
			if (request.downloadHandler == null)
			{
				throw new InvalidOperationException("Result type is only available for download requests.");
			}

			if (request.downloadHandler is DownloadHandlerAssetBundle && typeof(T) != typeof(AssetBundle))
			{
				throw new InvalidOperationException($"{nameof(AssetBundle)} result expected, while {typeof(T).Name} supplied.");
			}

			if (request.downloadHandler is DownloadHandlerTexture && typeof(T) != typeof(Texture) && typeof(T) != typeof(Texture2D))
			{
				throw new InvalidOperationException($"{nameof(Texture2D)} result expected, while {typeof(T).Name} supplied.");
			}

			if (request.downloadHandler is DownloadHandlerAudioClip && typeof(T) != typeof(AudioClip))
			{
				throw new InvalidOperationException($"{nameof(AudioClip)} result expected, while {typeof(T).Name} supplied.");
			}

			if (request.downloadHandler is DownloadHandlerBuffer && typeof(T) != typeof(string) && typeof(T) != typeof(byte[]))
			{
				throw new InvalidOperationException($"{nameof(String)} or byte array result expected, while {typeof(T).Name} supplied.");
			}

			return request;
		}

		internal static T GetResult<T>(this UnityWebRequest request) where T : class
		{
			ThrowIfNotCompleted(request);
			ThrowIfFailed(request);

			return GetResultInternal<T>(request);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfNotCompleted(this UnityWebRequest request)
		{
			if (!request.isDone)
			{
				throw new InvalidOperationException("The request is expected to be completed.");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfFailed(this UnityWebRequest request)
		{
			if (request.isHttpError || request.isNetworkError)
			{
				throw new UnityWebRequestException(request.error, request.responseCode);
			}
		}

		#endregion

		#region implementation

		private static T GetResultInternal<T>(UnityWebRequest request) where T : class
		{
			if (request.downloadHandler != null)
			{
				if (request.downloadHandler is DownloadHandlerAssetBundle)
				{
					return ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle as T;
				}
				else if (request.downloadHandler is DownloadHandlerTexture)
				{
					return ((DownloadHandlerTexture)request.downloadHandler).texture as T;
				}
				else if (request.downloadHandler is DownloadHandlerAudioClip)
				{
					return ((DownloadHandlerAudioClip)request.downloadHandler).audioClip as T;
				}
				else if (typeof(T) == typeof(byte[]))
				{
					return request.downloadHandler.data as T;
				}
				else if (typeof(T) == typeof(string))
				{
					return request.downloadHandler.text as T;
				}
			}

			return null;
		}

		private static void OnTaskCompleted<T>(TaskCompletionSource<T> tcs, UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate) where T : class
		{
			try
			{
				T requestResult;

				if (resultDelegate != null)
				{
					requestResult = resultDelegate(request);
				}
				else
				{
					requestResult = GetResult<T>(request);
				}

				tcs.TrySetResult(requestResult);
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		}

		#endregion
	}
}
