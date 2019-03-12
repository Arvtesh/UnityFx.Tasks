﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
#if !NET_LEGACY && !NET_2_0 && !NET_2_0_SUBSET && UNITY_2017_2_OR_NEWER

	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="AsyncOperation"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="AsyncOperation"/>
	public struct AsyncOperationAwaiter : INotifyCompletion
	{
		private readonly AsyncOperation _op;

		public AsyncOperationAwaiter(AsyncOperation op)
		{
			_op = op;
		}

		public bool IsCompleted
		{
			get
			{
				return _op.isDone;
			}
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}
	}

#endif
}
