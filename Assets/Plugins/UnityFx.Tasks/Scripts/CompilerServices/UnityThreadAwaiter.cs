// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on the Unity thread.
	/// This type is intended for compiler use only.
	/// </summary>
	public struct UnityThreadAwaiter : ICriticalNotifyCompletion
	{
		private readonly SynchronizationContext _unityThreadContext;

		public UnityThreadAwaiter(SynchronizationContext unityThreadContext)
		{
			_unityThreadContext = unityThreadContext;
		}

		public bool IsCompleted
		{
			get
			{
				return SynchronizationContext.Current == _unityThreadContext;
			}
		}

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			_unityThreadContext.Post(args => ((Action)args).Invoke(), continuation);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}
	}
}
