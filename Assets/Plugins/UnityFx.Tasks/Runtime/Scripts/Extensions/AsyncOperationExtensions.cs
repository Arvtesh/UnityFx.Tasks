// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Extension methods for <see cref="AsyncOperation"/>.
	/// </summary>
	public static class AsyncOperationExtensions
	{
		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the Unity <see cref="AsyncOperation"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(AsyncOperation, CancellationToken)"/>
		public static Task ToTask(this AsyncOperation op)
		{
			return ToTask(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the Unity <see cref="AsyncOperation"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(AsyncOperation)"/>
		public static Task ToTask(this AsyncOperation op, CancellationToken cancellationToken)
		{
			if (op.isDone)
			{
				return Task.CompletedTask;
			}
			else
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled(cancellationToken);
				}

				var result = new TaskCompletionSource<object>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => result.TrySetCanceled(cancellationToken));
				}

				op.completed += o =>
				{
					result.TrySetResult(null);
				};

				return result.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(ResourceRequest)"/>
		public static Task<UnityEngine.Object> ToTask(this ResourceRequest op)
		{
			return ToTask<UnityEngine.Object>(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(ResourceRequest, CancellationToken)"/>
		public static Task<UnityEngine.Object> ToTask(this ResourceRequest op, CancellationToken cancellationToken)
		{
			return ToTask<UnityEngine.Object>(op, cancellationToken);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns a <see cref="Task"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(ResourceRequest, CancellationToken)"/>
		public static Task<T> ToTask<T>(this ResourceRequest op) where T : UnityEngine.Object
		{
			return ToTask<T>(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(ResourceRequest)"/>
		public static Task<T> ToTask<T>(this ResourceRequest op, CancellationToken cancellationToken) where T : UnityEngine.Object
		{
			if (op.isDone)
			{
				if (op.asset is T)
				{
					return Task.FromResult(op.asset as T);
				}
				else
				{
					return Task.FromException<T>(new UnityAssetLoadException(typeof(T)));
				}
			}
			else
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<T>(cancellationToken);
				}

				var result = new TaskCompletionSource<T>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => result.TrySetCanceled(cancellationToken));
				}

				op.completed += o =>
				{
					var asset = op.asset as T;

					if (asset)
					{
						// NOTE: TrySetResult() failure means that the operation was cancelled, thus the resource should be unloaded.
						if (!result.TrySetResult(asset))
						{
							Resources.UnloadAsset(asset);
						}
					}
					else
					{
						result.TrySetException(new UnityAssetLoadException(typeof(T)));
					}
				};

				return result.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(AssetBundleRequest, CancellationToken)"/>
		public static Task<T> ToTask<T>(this AssetBundleRequest op) where T : UnityEngine.Object
		{
			return ToTask<T>(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask{T}(AssetBundleRequest)"/>
		public static Task<T> ToTask<T>(this AssetBundleRequest op, CancellationToken cancellationToken) where T : class
		{
			if (op.isDone)
			{
				var result = GetResult<T>(op);

				if (result != null)
				{
					return Task.FromResult(result);
				}
				else
				{
					return Task.FromException<T>(new UnityAssetLoadException(typeof(T)));
				}
			}
			else
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<T>(cancellationToken);
				}

				var tcs = new TaskCompletionSource<T>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
				}

				op.completed += o =>
				{
					var result = GetResult<T>(op);

					if (result != null)
					{
						// NOTE: TrySetResult() failure means that the operation was cancelled, thus all assets should be unloaded.
						if (!tcs.TrySetResult(result))
						{
							if (op.allAssets != null)
							{
								foreach (var obj in op.allAssets)
								{
									UnityEngine.Object.Destroy(obj);
								}
							}

							if (op.asset != null)
							{
								UnityEngine.Object.Destroy(op.asset);
							}
						}
					}
					else
					{
						tcs.TrySetException(new UnityAssetLoadException(typeof(T)));
					}
				};

				return tcs.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="AssetBundleCreateRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(AssetBundleCreateRequest, CancellationToken)"/>
		public static Task<AssetBundle> ToTask(this AssetBundleCreateRequest op)
		{
			return ToTask(op, CancellationToken.None);
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="AssetBundleCreateRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the request.</param>
		/// <returns>Returns a <see cref="Task{TResult}"/> instance that can be used to track the operation state.</returns>
		/// <seealso cref="ToTask(AssetBundleCreateRequest)"/>
		public static Task<AssetBundle> ToTask(this AssetBundleCreateRequest op, CancellationToken cancellationToken)
		{
			if (op.isDone)
			{
				if (op.assetBundle)
				{
					return Task.FromResult(op.assetBundle);
				}
				else
				{
					return Task.FromException<AssetBundle>(new UnityAssetLoadException(typeof(AssetBundle)));
				}
			}
			else
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled<AssetBundle>(cancellationToken);
				}

				var result = new TaskCompletionSource<AssetBundle>(op);

				if (cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => result.TrySetCanceled(cancellationToken));
				}

				op.completed += o =>
				{
					if (!op.assetBundle)
					{
						result.TrySetException(new UnityAssetLoadException(typeof(AssetBundle)));
					}
					else if (!result.TrySetResult(op.assetBundle))
					{
						// NOTE: TrySetResult() failure means that the operation was cancelled, thus the asset bundle should be unloaded.
						op.assetBundle.Unload(true);
					}
				};

				return result.Task;
			}
		}

		internal static T GetResult<T>(AssetBundleRequest op) where T : class
		{
			if (typeof(T).IsArray)
			{
				var assets = op.allAssets;

				// NOTE: Cannot just cast op.allAssets to T (this would return null), have to create new array.
				if (assets != null && assets.Length >= 0)
				{
					var elementType = typeof(T).GetElementType();
					var result = Array.CreateInstance(elementType, assets.Length);
					assets.CopyTo(result, 0);
					return result as T;
				}
			}
			else if (op.asset != null)
			{
				return op.asset as T;
			}
			else if (op.allAssets != null && op.allAssets.Length > 0)
			{
				return op.allAssets[0] as T;
			}

			return default(T);
		}
	}
}
