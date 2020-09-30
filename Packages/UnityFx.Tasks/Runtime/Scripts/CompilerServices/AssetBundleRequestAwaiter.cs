// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="AssetBundleRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="AssetBundleRequest"/>
	public struct AssetBundleRequestAwaiter : ICriticalNotifyCompletion
	{
		private readonly AssetBundleRequest _op;

		public AssetBundleRequestAwaiter(AssetBundleRequest op)
		{
			_op = op;
		}

		public bool IsCompleted => _op.isDone;

		public UnityEngine.Object GetResult()
		{
			return _op.asset;
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
