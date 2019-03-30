// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="UnityWebRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityWebRequest"/>
	public struct UnityWebRequestAwaiter<T> : INotifyCompletion where T : class
	{
		private readonly UnityWebRequest _request;

		public UnityWebRequestAwaiter(UnityWebRequest op)
		{
			_request = op;
		}

		public bool IsCompleted
		{
			get
			{
				return _request.isDone;
			}
		}

		public T GetResult()
		{
			return _request.GetResult<T>();
		}

		public void OnCompleted(Action continuation)
		{
			_request.ThrowIfSent();
			_request.SendWebRequest().completed += op => continuation();
		}
	}
}
