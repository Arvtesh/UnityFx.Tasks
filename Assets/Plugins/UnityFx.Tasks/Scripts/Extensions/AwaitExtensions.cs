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
	}
}
