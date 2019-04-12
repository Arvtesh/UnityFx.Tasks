// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Async/asait extensions.
	/// </summary>
	public static class AwaitExtensions
	{
		/// <summary>
		/// Returns the <see cref="AsyncOperation"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
		{
			return new CompilerServices.AsyncOperationAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="YieldInstruction"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.YieldInstructionAwaiter GetAwaiter(this YieldInstruction op)
		{
			return new CompilerServices.YieldInstructionAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="ResourceRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.ResourceRequestAwaiter GetAwaiter(this ResourceRequest op)
		{
			return new CompilerServices.ResourceRequestAwaiter(op);
		}

		/// <summary>
		/// Creates a configurable awaitable object for the <see cref="ResourceRequest"/> instance.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns an awaitable object that can track the operation completion.</returns>
		public static CompilerServices.ResourceRequestAwaitable<T> ConfigureAwait<T>(this ResourceRequest op) where T : UnityEngine.Object
		{
			return new CompilerServices.ResourceRequestAwaitable<T>(op);
		}

		/// <summary>
		/// Returns the <see cref="AssetBundleRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.AssetBundleRequestAwaiter GetAwaiter(this AssetBundleRequest op)
		{
			return new CompilerServices.AssetBundleRequestAwaiter(op);
		}

		/// <summary>
		/// Creates a configurable awaitable object for the <see cref="AssetBundleRequest"/> instance.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns an awaitable object that can track the operation completion.</returns>
		public static CompilerServices.AssetBundleRequestAwaitable<T> ConfigureAwait<T>(this AssetBundleRequest op) where T : UnityEngine.Object
		{
			return new CompilerServices.AssetBundleRequestAwaitable<T>(op);
		}

		/// <summary>
		/// Returns the <see cref="AssetBundleCreateRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.AssetBundleCreateRequestAwaiter GetAwaiter(this AssetBundleCreateRequest op)
		{
			return new CompilerServices.AssetBundleCreateRequestAwaiter(op);
		}

		/// <summary>
		/// Returns the <see cref="UnityWebRequest"/> awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		/// <returns>Returns the operation awaiter.</returns>
		public static CompilerServices.UnityWebRequestAwaiter GetAwaiter(this UnityWebRequest op)
		{
			return new CompilerServices.UnityWebRequestAwaiter(op);
		}

		/// <summary>
		/// Creates a configurable awaitable object for the <see cref="UnityWebRequest"/> instance.
		/// </summary>
		/// <typeparam name="T">Type of the request result value.</typeparam>
		/// <param name="request">The source web request.</param>
		/// <returns>Returns an awaitable object that can track the request completion.</returns>
		public static CompilerServices.UnityWebRequestAwaitable<T> ConfigureAwait<T>(this UnityWebRequest request) where T : class
		{
			return new CompilerServices.UnityWebRequestAwaitable<T>(request);
		}

		/// <summary>
		/// Creates a configurable awaitable object for the <see cref="UnityWebRequestAsyncOperation"/> instance.
		/// </summary>
		/// <typeparam name="T">Type of the operation result value.</typeparam>
		/// <param name="op">The source operation.</param>
		/// <returns>Returns an awaitable object that can track the operation completion.</returns>
		public static CompilerServices.UnityWebRequestAwaitable<T> ConfigureAwait<T>(this UnityWebRequestAsyncOperation op) where T : class
		{
			return new CompilerServices.UnityWebRequestAwaitable<T>(op.webRequest);
		}
	}
}
