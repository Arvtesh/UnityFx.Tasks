// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows skipping a specified number of frames.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="TimeDelayAwaiter"/>
	public struct TimeDelayAwaitable
	{
		private readonly TimeDelayAwaiter _awaiter;

		public TimeDelayAwaitable(float delay, TimerType timerType)
		{
			_awaiter = new TimeDelayAwaiter(delay, timerType);
		}

		public TimeDelayAwaiter GetAwaiter()
		{
			return _awaiter;
		}
	}
}
