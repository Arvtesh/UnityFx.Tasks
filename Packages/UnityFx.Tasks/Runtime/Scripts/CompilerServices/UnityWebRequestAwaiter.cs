// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks.CompilerServices
{
	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="UnityWebRequestAsyncOperation"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityWebRequestAwaitable"/>
	/// <seealso cref="UnityWebRequestAsyncOperation"/>
	/// <seealso cref="UnityWebRequest"/>
	public struct UnityWebRequestAwaiter : ICriticalNotifyCompletion
	{
		private readonly UnityWebRequestAsyncOperation _op;

		public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation op)
		{
			_op = op;
		}

		public bool IsCompleted => _op.webRequest.isDone;

		public void GetResult()
		{
			var request = _op.webRequest;

			if (request.isNetworkError || request.isHttpError)
			{
				throw new UnityWebRequestException(request.error, request.responseCode);
			}
		}

		public void OnCompleted(Action continuation)
		{
			_op.completed += op => continuation();
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			OnCompleted(continuation);
		}
	}
}
