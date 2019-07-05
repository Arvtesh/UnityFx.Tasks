// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="Animator"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="Animator"/>
	public struct AnimatorUntilAwaitable
	{
		private readonly AnimatorUntilAwaiter _awaiter;

		public AnimatorUntilAwaitable(Animator animator, int stateNameHash, int layer)
		{
			_awaiter = new AnimatorUntilAwaiter(animator, stateNameHash, layer);
		}

		public AnimatorUntilAwaiter GetAwaiter()
		{
			return _awaiter;
		}
	}
}
