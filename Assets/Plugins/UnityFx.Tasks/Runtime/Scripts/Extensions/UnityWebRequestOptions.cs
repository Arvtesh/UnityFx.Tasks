// Copyright (c) 2018-2019 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Enumerates <see cref="UnityWebRequest"/>-related options.
	/// </summary>
	[Flags]
	public enum UnityWebRequestOptions
	{
		None = 0,
		AutoDispose = 1
	}
}
