// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="UnityWebRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityWebRequestAwaiter{T}"/>
	/// <seealso cref="UnityWebRequest"/>
	public struct UnityWebRequestAwaitable<T> where T : class
	{
		private readonly UnityWebRequestAwaiter<T> _awaiter;

		public UnityWebRequestAwaitable(UnityWebRequest op)
		{
			_awaiter = new UnityWebRequestAwaiter<T>(op);
		}

		public UnityWebRequestAwaiter<T> GetAwaiter()
		{
			return _awaiter;
		}
	}
}
