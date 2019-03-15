// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks
{
#if !NET_LEGACY && !NET_2_0 && !NET_2_0_SUBSET

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

		#endregion

		#region implementation
		#endregion
	}

#endif
}
