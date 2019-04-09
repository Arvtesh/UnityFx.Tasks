// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEngine;

namespace UnityFx.Tasks.Helpers
{
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
				throw new ArgumentNullException(nameof(d));
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
				throw new ArgumentNullException(nameof(d));
			}

			_actionQueue.Enqueue(new InvokeResult(d, state));
		}

		#endregion
	}
}
