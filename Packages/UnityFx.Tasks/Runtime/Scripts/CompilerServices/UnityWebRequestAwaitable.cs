// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="UnityWebRequestAsyncOperation"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityWebRequestAwaiter"/>
	/// <seealso cref="UnityWebRequestAsyncOperation"/>
	/// <seealso cref="UnityWebRequest"/>
	public struct UnityWebRequestAwaitable
	{
		private readonly UnityWebRequestAwaiter _awaiter;

		public UnityWebRequestAwaitable(UnityWebRequestAsyncOperation op)
		{
			_awaiter = new UnityWebRequestAwaiter(op);
		}

		public UnityWebRequestAwaiter GetAwaiter()
		{
			return _awaiter;
		}
	}
}
