﻿// Copyright (c) Alexander Bogarsukov.
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
	/// <seealso cref="ResourceRequest"/>
	public struct ResourceRequestAwaiter : ICriticalNotifyCompletion
	{
		private readonly ResourceRequest _op;

		public ResourceRequestAwaiter(ResourceRequest op)
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

		public UnityEngine.Object GetResult()
		{
			if (!_op.asset)
			{
				throw new UnityAssetLoadException();
			}

			return _op.asset;
		}

		public void OnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}
	}
}
