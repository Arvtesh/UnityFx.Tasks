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

		public static Task SendWebRequestAsync(this UnityWebRequest request)
		{
			return SendWebRequestAsync<object>(request, null, CancellationToken.None);
		}

		public static Task SendWebRequestAsync(this UnityWebRequest request, CancellationToken cancellationToken)
		{
			return SendWebRequestAsync<object>(request, null, cancellationToken);
		}

		public static Task<T> SendWebRequestAsync<T>(this UnityWebRequest request) where T : class
		{
			return SendWebRequestAsync<T>(request, null, CancellationToken.None);
		}

		public static Task<TResult> SendWebRequestAsync<TResult>(this UnityWebRequest request, Func<DownloadHandler, TResult> requestResultParser, CancellationToken cancellationToken) where TResult : class
		{
			if (request is null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			if (!request.isModifiable)
			{
				throw new InvalidOperationException();
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<TResult>(cancellationToken);
			}
			else
			{
				// Make sure download handler is set for requests with any non-void result type.
				if (request.downloadHandler is null && typeof(TResult) != typeof(object))
				{
					if (typeof(TResult) == typeof(AssetBundle))
					{
						request.downloadHandler = new DownloadHandlerAssetBundle(request.url, 0);
					}
					else if (typeof(TResult) == typeof(Texture) || typeof(TResult) == typeof(Texture2D))
					{
						request.downloadHandler = new DownloadHandlerTexture();
					}
					else if (typeof(TResult) == typeof(AudioClip))
					{
						request.downloadHandler = new DownloadHandlerAudioClip(request.url, AudioType.UNKNOWN);
					}
					else
					{
						request.downloadHandler = new DownloadHandlerBuffer();
					}
				}

				var tcs = new TaskCompletionSource<TResult>();
				var wwwOp = request.SendWebRequest();

				if (wwwOp != null)
				{
					if (cancellationToken.CanBeCanceled)
					{
						cancellationToken.Register(
							() =>
							{
								// The delegate might be called after the request has need disposed.
								// So only cancel web request if the task is still pending.
								if (!tcs.Task.IsCompleted)
								{
									request.Abort();
									tcs.TrySetCanceled(cancellationToken);
								}
							},
							true);
					}

					wwwOp.completed += op =>
					{
						try
						{
							if (cancellationToken.IsCancellationRequested)
							{
								tcs.TrySetCanceled(cancellationToken);
							}
							else if (request.isNetworkError || request.isHttpError)
							{
								tcs.TrySetException(new UnityWebRequestException(request.error, request.responseCode));
							}
							else
							{
								var result = GetResultInternal(request, requestResultParser);
								tcs.TrySetResult(result);
							}
						}
						catch (Exception e)
						{
							tcs.TrySetException(e);
						}
					};
				}
				else
				{
					// NOTE: wwwOp might be null if the request is cancelled.
					tcs.TrySetCanceled(cancellationToken);
				}

				return tcs.Task;
			}
		}

		internal static T GetResult<T>(this UnityWebRequest request) where T : class
		{
			ThrowIfNotCompleted(request);
			ThrowIfFailed(request);

			return GetResultInternal<T>(request, null);
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

		private static T GetResultInternal<T>(UnityWebRequest request, Func<DownloadHandler, T> requestResultParser) where T : class
		{
			var dh = request.downloadHandler;

			if (dh != null)
			{
				if (requestResultParser != null)
				{
					return requestResultParser(dh);
				}
				else if (typeof(T) == typeof(string))
				{
					return dh.text as T;
				}
				else if (typeof(T) == typeof(byte[]))
				{
					return dh.data as T;
				}
				else if (typeof(T) == typeof(AssetBundle) && dh is DownloadHandlerAssetBundle downloadHandlerAssetBundle)
				{
					return downloadHandlerAssetBundle.assetBundle as T;
				}
				else if (typeof(T) == typeof(AudioClip) && dh is DownloadHandlerAudioClip downloadHandlerAudioClip)
				{
					return downloadHandlerAudioClip.audioClip as T;
				}
				else if ((typeof(T) == typeof(Texture) || typeof(T) == typeof(Texture2D)) && dh is DownloadHandlerTexture downloadHandlerTexture)
				{
					return downloadHandlerTexture.texture as T;
				}
				else if (typeof(T).IsSerializable)
				{
					return JsonUtility.FromJson<T>(dh.text);
				}
			}

			return null;
		}

		#endregion
	}
}
