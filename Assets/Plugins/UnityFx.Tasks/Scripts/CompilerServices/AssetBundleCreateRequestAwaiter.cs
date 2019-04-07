// Copyright (c) Alexander Bogarsukov.
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
	/// <seealso cref="AsyncOperation"/>
	public struct AssetBundleCreateRequestAwaiter : INotifyCompletion
	{
		private readonly AssetBundleCreateRequest _op;

		public AssetBundleCreateRequestAwaiter(AssetBundleCreateRequest op)
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

		public AssetBundle GetResult()
		{
			if (!_op.assetBundle)
			{
				throw new InvalidOperationException();
			}

			return _op.assetBundle;
		}

		public void OnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}
	}
}
