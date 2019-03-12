// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks
{
#if !NET_LEGACY && !NET_2_0 && !NET_2_0_SUBSET

	/// <summary>
	/// Extensions of <see cref="Task"/>.
	/// </summary>
	public static class TaskExtensions
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Converts the operation (task) to a <see cref="IEnumerator"/> instance that can be used as Unity coroutine.
		/// </summary>
		/// <param name="op">The source operation (task).</param>
		public static IEnumerator ToCoroutine(this IAsyncResult op)
		{
			return new TaskEnumerator(op);
		}

		#endregion

		#region implementation

		private class TaskEnumerator : IEnumerator
		{
			private readonly IAsyncResult _task;

			public TaskEnumerator(IAsyncResult task)
			{
				_task = task;
			}

			public object Current
			{
				get
				{
					return null;
				}
			}

			public bool MoveNext()
			{
				return !_task.IsCompleted;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		#endregion
	}

#endif
}
