// Copyright (c) 2018-2019 Alexander Bogarsukov.
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
	public struct AnimatorWhileAwaitable
	{
		private readonly AnimatorWhileAwaiter _awaiter;

		public AnimatorWhileAwaitable(Animator animator, int stateNameHash, int layer)
		{
			_awaiter = new AnimatorWhileAwaiter(animator, stateNameHash, layer);
		}

		public AnimatorWhileAwaiter GetAwaiter()
		{
			return _awaiter;
		}
	}
}
