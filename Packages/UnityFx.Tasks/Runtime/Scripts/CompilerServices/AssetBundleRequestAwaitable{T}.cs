// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="AssetBundleRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="AssetBundleRequestAwaiter{T}"/>
	/// <seealso cref="AssetBundleRequest"/>
	public struct AssetBundleRequestAwaitable<T> where T : UnityEngine.Object
	{
		private readonly AssetBundleRequestAwaiter<T> _awaiter;

		public AssetBundleRequestAwaitable(AssetBundleRequest op)
		{
			_awaiter = new AssetBundleRequestAwaiter<T>(op);
		}

		public AssetBundleRequestAwaiter<T> GetAwaiter()
		{
			return _awaiter;
		}
	}
}
