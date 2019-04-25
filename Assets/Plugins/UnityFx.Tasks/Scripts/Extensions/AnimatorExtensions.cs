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
	/// Extensions methods for <see cref="Animator"/>.
	/// </summary>
	public static class AnimatorExtensions
	{
		#region interface

		/// <summary>
		/// Configures await while the specified state is playing.
		/// </summary>
		/// <param name="animator">Source animation controller.</param>
		/// <param name="stateName">Name of the state to play.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An awaitable opject.</returns>
		public static CompilerServices.AnimatorWhileAwaitable ConfigureAwait(this Animator animator, string stateName, int layer = -1)
		{
			return new CompilerServices.AnimatorWhileAwaitable(animator, Animator.StringToHash(stateName), layer);
		}

		/// <summary>
		/// Configures await while the specified state is playing.
		/// </summary>
		/// <param name="animator">Source animation controller.</param>
		/// <param name="stateNameHash">Hash of the state name.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An awaitable opject.</returns>
		public static CompilerServices.AnimatorWhileAwaitable ConfigureAwait(this Animator animator, int stateNameHash, int layer = -1)
		{
			return new CompilerServices.AnimatorWhileAwaitable(animator, stateNameHash, layer);
		}

		/// <summary>
		/// Configures await until the specified state is activated.
		/// </summary>
		/// <param name="animator">Source animation controller.</param>
		/// <param name="stateName">Name of the state to play.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An awaitable opject.</returns>
		public static CompilerServices.AnimatorWhileAwaitable ConfigureAwaitUntil(this Animator animator, string stateName, int layer = -1)
		{
			return new CompilerServices.AnimatorWhileAwaitable(animator, Animator.StringToHash(stateName), layer);
		}

		/// <summary>
		/// Configures await until the specified state is activated.
		/// </summary>
		/// <param name="animator">Source animation controller.</param>
		/// <param name="stateNameHash">Hash of the state name.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An awaitable opject.</returns>
		public static CompilerServices.AnimatorWhileAwaitable ConfigureAwaitUntil(this Animator animator, int stateNameHash, int layer = -1)
		{
			return new CompilerServices.AnimatorWhileAwaitable(animator, stateNameHash, layer);
		}

		#endregion

		#region implementation
		#endregion
	}
}
