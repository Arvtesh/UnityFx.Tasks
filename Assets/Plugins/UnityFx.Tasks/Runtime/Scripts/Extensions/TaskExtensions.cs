// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Extensions of <see cref="Task"/>.
	/// </summary>
	public static class TaskExtensions
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Converts the operation (task) to a <see cref="IEnumerator"/> instance that can be used in Unity coroutine.
		/// </summary>
		/// <param name="op">The source operation (task).</param>
		/// <returns>Returns enumerator that can be used in Unity coroutine.</returns>
		public static IEnumerator ToEnumerator(this IAsyncResult op)
		{
			if (op is IEnumerator)
			{
				return op as IEnumerator;
			}

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
}
