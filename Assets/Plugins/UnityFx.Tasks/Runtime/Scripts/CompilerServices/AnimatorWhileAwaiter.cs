﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="Animator"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="Animator"/>
	public struct AnimatorWhileAwaiter : INotifyCompletion
	{
		private readonly Animator _animator;
		private readonly int _stateNameHash;
		private readonly int _layer;

		public AnimatorWhileAwaiter(Animator animator, int stateNameHash, int layer)
		{
			_animator = animator;
			_stateNameHash = stateNameHash;
			_layer = layer;
		}

		public bool IsCompleted
		{
			get
			{
				var state = _animator.GetCurrentAnimatorStateInfo(_layer);
				return state.shortNameHash != _stateNameHash || state.normalizedTime >= 1;
			}
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			TaskUtility.StartCoroutine(WaitWhile(continuation));
		}

		private IEnumerator WaitWhile(Action continuation)
		{
			while (true)
			{
				if (IsCompleted)
				{
					break;
				}
				else
				{
					yield return null;
				}
			}

			continuation();
		}
	}
}
