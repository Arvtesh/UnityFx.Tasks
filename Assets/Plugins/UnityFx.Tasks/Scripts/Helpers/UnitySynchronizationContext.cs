// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
#if NET_4_6 || NET_STANDARD_2_0
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
#endif
using System.Threading;
using UnityEngine;

namespace UnityFx.Tasks.Helpers
{
#if !NET_LEGACY && !NET_2_0 && !NET_2_0_SUBSET

	/// <summary>
	/// Implementation of <see cref="SynchronizationContext"/> for Unity.
	/// </summary>
	internal sealed class UnitySynchronizationContext : SynchronizationContext
	{
		#region data

		private struct InvokeResult
		{
			private readonly SendOrPostCallback _callback;
			private readonly object _userState;

			public InvokeResult(SendOrPostCallback d, object userState)
			{
				_callback = d;
				_userState = userState;
			}

			public void Invoke(UnityEngine.Object context)
			{
				try
				{
					_callback.Invoke(_userState);
				}
				catch (Exception e)
				{
					Debug.LogException(e, context);
				}
			}
		}

		private readonly ConcurrentQueue<InvokeResult> _actionQueue = new ConcurrentQueue<InvokeResult>();

		#endregion

		#region interface

		public void Update(UnityEngine.Object context)
		{
			InvokeResult invokeResult;

			while (_actionQueue.TryDequeue(out invokeResult))
			{
				invokeResult.Invoke(context);
			}
		}

		#endregion

		#region SynchronizationContext

		public override SynchronizationContext CreateCopy()
		{
			return new UnitySynchronizationContext();
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}

			if (this == Current)
			{
				d.Invoke(state);
			}
			else
			{
				var completed = false;
				var exception = default(Exception);
				var asyncResult = new InvokeResult(
					args =>
					{
						try
						{
							d.Invoke(args);
						}
						catch (Exception e)
						{
							exception = e;
						}
						finally
						{
							completed = true;
						}
					},
					state);

				_actionQueue.Enqueue(asyncResult);

				var sw = new SpinWait();

				while (!completed)
				{
					sw.SpinOnce();
				}

				if (exception != null)
				{
					ExceptionDispatchInfo.Capture(exception).Throw();
				}
			}
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}

			_actionQueue.Enqueue(new InvokeResult(d, state));
		}

		#endregion
	}

#endif
}
