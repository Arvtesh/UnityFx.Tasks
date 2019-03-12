// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Tasks.CompilerServices
{
#if NET_4_6 || NET_STANDARD_2_0

	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="YieldInstruction"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="YieldInstruction"/>
	public class YieldInstructionAwaiter : IEnumerator, INotifyCompletion
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
			TaskUtility.StartCoroutine(this);
		}

		#endregion

		#region IEnumerator

		public object Current
		{
			get
			{
				return _current;
			}
		}

		public bool MoveNext()
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

		public void Reset()
		{
			throw new NotSupportedException();
		}

		#endregion
	}

#endif
}
