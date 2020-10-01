// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="ResourceRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="ResourceRequestAwaitable{T}"/>
	/// <seealso cref="ResourceRequest"/>
	public struct ResourceRequestAwaiter<T> : ICriticalNotifyCompletion where T : UnityEngine.Object
	{
		private readonly ResourceRequest _op;

		public ResourceRequestAwaiter(ResourceRequest op)
		{
			_op = op;
		}

		public bool IsCompleted => _op.isDone;

		public T GetResult()
		{
			return (T)_op.asset;
		}

		public void OnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}
	}
}
