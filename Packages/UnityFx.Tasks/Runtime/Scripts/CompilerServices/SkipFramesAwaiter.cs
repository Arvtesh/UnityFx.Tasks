// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows skipping frames.
	/// This type is intended for compiler use only.
	/// </summary>
	public struct SkipFramesAwaiter : ICriticalNotifyCompletion
	{
		private readonly int _numberOfFramesToSkip;

		public SkipFramesAwaiter(int numberOfFramesToSkip)
		{
			_numberOfFramesToSkip = numberOfFramesToSkip;
		}

		public bool IsCompleted => _numberOfFramesToSkip > 0;

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			TaskUtility.Schedule(continuation, _numberOfFramesToSkip);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}
	}
}
