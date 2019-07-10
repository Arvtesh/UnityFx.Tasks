// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="ResourceRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="ResourceRequestAwaiter{T}"/>
	/// <seealso cref="ResourceRequest"/>
	public struct ResourceRequestAwaitable<T> where T : UnityEngine.Object
	{
		private readonly ResourceRequestAwaiter<T> _awaiter;

		public ResourceRequestAwaitable(ResourceRequest op)
		{
			_awaiter = new ResourceRequestAwaiter<T>(op);
		}

		public ResourceRequestAwaiter<T> GetAwaiter()
		{
			return _awaiter;
		}
	}
}
