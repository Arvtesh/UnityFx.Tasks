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
	/// <seealso cref="TimeDelayAwaitable"/>
	public struct TimeDelayAwaiter : ICriticalNotifyCompletion
	{
		private readonly float _delay;
		private readonly TimerType _timerType;

		public TimeDelayAwaiter(float delay, TimerType timerType)
		{
			_delay = delay;
			_timerType = timerType;
		}

		public bool IsCompleted => _delay < float.Epsilon;

		public void GetResult()
		{
		}

		public void OnCompleted(Action continuation)
		{
			TaskUtility.Schedule(continuation, _delay, _timerType);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}
	}
}
