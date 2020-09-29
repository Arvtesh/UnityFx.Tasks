// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="YieldInstruction"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="YieldInstruction"/>
	public class YieldInstructionAwaiter : IEnumerator, ICriticalNotifyCompletion
	{
		#region data

		private YieldInstruction _yieldValue;
		private Action _callback;
		private object _current;

		#endregion

		#region interface

		public YieldInstructionAwaiter(YieldInstruction op)
		{
			_yieldValue = op;
		}

		public bool IsCompleted
		{
			get
			{
				return _current == null && _yieldValue == null;
			}
		}

		public void GetResult()
		{
		}

		#endregion

		#region INotifyCompletion

		public void OnCompleted(Action continuation)
		{
			_callback = continuation;

			// NOTE: This call always schedules the continuation on the Unity thread. This differs from Task awaiter behavior (continue on the same thread).
			TaskUtility.StartCoroutine(this);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}

		#endregion

		#region IEnumerator

		object IEnumerator.Current
		{
			get
			{
				return _current;
			}
		}

		bool IEnumerator.MoveNext()
		{
			if (_yieldValue != null)
			{
				if (_current == null)
				{
					_current = _yieldValue;
					return true;
				}
				else
				{
					_current = null;
					_yieldValue = null;
					_callback();
				}
			}

			return false;
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
