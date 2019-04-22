// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on the Unity thread.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityThreadAwaiter"/>
	public struct UnityThreadAwaitable
	{
		private readonly UnityThreadAwaiter _awaiter;

		public UnityThreadAwaitable(SynchronizationContext unityThreadContext)
		{
			_awaiter = new UnityThreadAwaiter(unityThreadContext);
		}

		public UnityThreadAwaiter GetAwaiter()
		{
			return _awaiter;
		}
	}
}
