// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Extensions of Unity API.
	/// </summary>
	public static class UnityExtensions
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Returns the <see cref="AsyncOperation"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
		{
			return new CompilerServices.AsyncOperationAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="YieldInstruction"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.YieldInstructionAwaiter GetAwaiter(this YieldInstruction op)
		{
			return new CompilerServices.YieldInstructionAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="ResourceRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.ResourceRequestAwaiter GetAwaiter(this ResourceRequest op)
		{
			return new CompilerServices.ResourceRequestAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="AssetBundleRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.AssetBundleRequestAwaiter GetAwaiter(this AssetBundleRequest op)
		{
			return new CompilerServices.AssetBundleRequestAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="AssetBundleCreateRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest op)
		{
			return new CompilerServices.AssetBundleCreateRequestAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="UnityWebRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.UnityWebRequestAwaiter GetAwaiter(this UnityWebRequest op)
		{
			return new CompilerServices.UnityWebRequestAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="UnityWebRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.UnityWebRequestAwaiter<T> GetAwaiter<T>(this UnityWebRequest op) where T : class
		{
			return new CompilerServices.UnityWebRequestAwaiter<T>(op);
		}

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task ToTask(this UnityWebRequest request)
		{
			return ToTask(request, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
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
					return Task.FromException(new Helpers.UnityWebRequestException(request.error, request.responseCode));
				}
				else
				{
					return Task.CompletedTask;
				}
			}
			else
			{
				ThrowIfSent(request);

				var result = new TaskCompletionSource<object>(request);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						request.Abort();
						result.TrySetCanceled(cancellationToken);
					},
					true);
				}

				request.SendWebRequest().completed += op => OnTaskCompleted(result, request);
				return result.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task<T> ToTask<T>(this UnityWebRequest request) where T : class
		{
			return ToTask<T>(request, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		public static Task<T> ToTask<T>(this UnityWebRequest request, CancellationToken cancellationToken) where T : class
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
					return Task.FromException<T>(new Helpers.UnityWebRequestException(request.error, request.responseCode));
				}
				else
				{
					return Task.FromResult(GetResultInternal<T>(request));
				}
			}
			else
			{
				ThrowIfSent(request);

				var result = new TaskCompletionSource<T>(request);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() =>
					{
						request.Abort();
						result.TrySetCanceled(cancellationToken);
					},
					true);
				}

				request.SendWebRequest().completed += op => OnTaskCompleted(result, request);
				return result.Task;
			}
		}

		/// <summary>
		/// Attempts to get result of the supplied <see cref="UnityWebRequest"/> instance.
		/// </summary>
		/// <param name="request">The source web request.</param>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="request"/> is not completed.</exception>
		/// <exception cref="Helpers.UnityWebRequestException">Thrown is the <paramref name="request"/> has failed.</exception>
		public static T GetResult<T>(this UnityWebRequest request) where T : class
		{
			ThrowIfNotCompleted(request);
			ThrowIfFailed(request);

			return GetResultInternal<T>(request);
		}

		/// <summary>
		/// Thrown an <see cref="InvalidOperationException"/> if the web request is not completed.
		/// </summary>
		/// <param name="request">The source web request.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotCompleted(this UnityWebRequest request)
		{
			if (!request.isDone)
			{
				throw new InvalidOperationException("The request is expected to be completed.");
			}
		}

		/// <summary>
		/// Thrown an <see cref="InvalidOperationException"/> if <see cref="UnityWebRequest.SendWebRequest"/> has been called on the request instance.
		/// </summary>
		/// <param name="request">The source web request.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSent(this UnityWebRequest request)
		{
			if (!request.isModifiable)
			{
				throw new InvalidOperationException("SendWebRequest() has already been called on the web request.");
			}
		}

		/// <summary>
		/// Thrown an exception if the web request has failed.
		/// </summary>
		/// <param name="request">The source web request.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfFailed(this UnityWebRequest request)
		{
			if (request.isHttpError || request.isNetworkError)
			{
				throw new Helpers.UnityWebRequestException(request.error, request.responseCode);
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

		private static void OnTaskCompleted<T>(TaskCompletionSource<T> tcs, UnityWebRequest request) where T : class
		{
			try
			{
				tcs.TrySetResult(GetResult<T>(request));
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		}

		#endregion
	}
}
