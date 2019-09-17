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
		/// <seealso cref="ToTask(UnityWebRequest, CancellationToken, UnityWebRequestOptions)"/>
		public static Task ToTask(this UnityWebRequest request)
		{
			return ToTask(request, CancellationToken.None, UnityWebRequestOptions.None);
		}

		/// <summary>
		/// Creates a cancellable <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(UnityWebRequest)"/>
		/// <seealso cref="ToTask(UnityWebRequest, CancellationToken, UnityWebRequestOptions)"/>
		public static Task ToTask(this UnityWebRequest request, CancellationToken cancellationToken)
		{
			return ToTask(request, cancellationToken, UnityWebRequestOptions.None);
		}

		/// <summary>
		/// Creates a cancellable <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <param name="options">Options mask.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(UnityWebRequest)"/>
		/// <seealso cref="ToTask(UnityWebRequest, CancellationToken)"/>
		public static Task ToTask(this UnityWebRequest request, CancellationToken cancellationToken, UnityWebRequestOptions options)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				request.Abort();

				// Dispose the request is needed.
				if ((options & UnityWebRequestOptions.AutoDispose) != 0)
				{
					request.Dispose();
				}

				return Task.FromCanceled(cancellationToken);
			}

			if (request.isDone)
			{
				Task result;

				// NOTE: Short path for completed requests.
				if (request.isHttpError || request.isNetworkError)
				{
					result = Task.FromException(new UnityWebRequestException(request.error, request.responseCode));
				}
				else
				{
					result = Task.CompletedTask;
				}

				// Dispose the request is needed.
				if ((options & UnityWebRequestOptions.AutoDispose) != 0)
				{
					request.Dispose();
				}

				return result;
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

							// Dispose the request is needed.
							if ((options & UnityWebRequestOptions.AutoDispose) != 0)
							{
								request.Dispose();
							}
						}
					},
					true);
				}

				if (request.isModifiable)
				{
					request.SendWebRequest().completed += op => OnTaskCompleted(result, request, null, options);
				}
				else
				{
					TaskUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request, null, options));
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
		/// <seealso cref="ToTask{T}(UnityWebRequest, Func{UnityWebRequest, T})"/>
		/// <seealso cref="ToTask{T}(UnityWebRequest, Func{UnityWebRequest, T}, CancellationToken, UnityWebRequestOptions)"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request) where T : class
		{
			return ToTask<T>(request, null, CancellationToken.None, UnityWebRequestOptions.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <param name="resultDelegate">An optional delegate that is used to get the request result.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(UnityWebRequest)"/>
		/// <seealso cref="ToTask{T}(UnityWebRequest, Func{UnityWebRequest, T}, CancellationToken, UnityWebRequestOptions)"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate) where T : class
		{
			return ToTask(request, resultDelegate, CancellationToken.None, UnityWebRequestOptions.None);
		}

		/// <summary>
		/// Creates a cancellable <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>. The method calls <see cref="UnityWebRequest.SendWebRequest"/> (if not called).
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <param name="resultDelegate">An optional delegate that is used to get the request result.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <param name="options">Options mask.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(UnityWebRequest)"/>
		/// <seealso cref="ToTask{T}(UnityWebRequest, Func{UnityWebRequest, T})"/>
		public static Task<T> ToTask<T>(this UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate, CancellationToken cancellationToken, UnityWebRequestOptions options) where T : class
		{
			if (cancellationToken.IsCancellationRequested)
			{
				request.Abort();

				// Dispose the request is needed.
				if ((options & UnityWebRequestOptions.AutoDispose) != 0)
				{
					request.Dispose();
				}

				return Task.FromCanceled<T>(cancellationToken);
			}

			if (request.isDone)
			{
				Task<T> result;

				// NOTE: Short path for completed requests.
				if (request.isHttpError || request.isNetworkError)
				{
					result = Task.FromException<T>(new UnityWebRequestException(request.error, request.responseCode));
				}
				else if (resultDelegate != null)
				{
					result = Task.FromResult(resultDelegate(request));
				}
				else
				{
					result = Task.FromResult(GetResultInternal<T>(request));
				}

				// Dispose the request is needed.
				if ((options & UnityWebRequestOptions.AutoDispose) != 0)
				{
					request.Dispose();
				}

				return result;
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

							// Dispose the request is needed.
							if ((options & UnityWebRequestOptions.AutoDispose) != 0)
							{
								request.Dispose();
							}
						}
					},
					true);
				}

				if (request.isModifiable)
				{
					request.SendWebRequest().completed += op => OnTaskCompleted(result, request, resultDelegate, options);
				}
				else
				{
					TaskUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request, resultDelegate, options));
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

		private static void OnTaskCompleted<T>(TaskCompletionSource<T> tcs, UnityWebRequest request, Func<UnityWebRequest, T> resultDelegate, UnityWebRequestOptions options) where T : class
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
			finally
			{
				if ((options & UnityWebRequestOptions.AutoDispose) != 0)
				{
					request.Dispose();
				}
			}
		}

		#endregion
	}
}
