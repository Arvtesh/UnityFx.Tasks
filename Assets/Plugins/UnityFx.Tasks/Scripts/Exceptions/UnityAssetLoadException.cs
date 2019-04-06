// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace UnityFx.Tasks
{
	/// <summary>
	/// Represents an asset loading error.
	/// </summary>
	public class UnityAssetLoadException : Exception
	{
		#region data

		private readonly Type _assetType;
		private readonly string _assetName;

		#endregion

		#region interface

		/// <summary>
		/// Gets an asset name.
		/// </summary>
		public string AssetName
		{
			get
			{
				return _assetName;
			}
		}

		/// <summary>
		/// Gets an asset type.
		/// </summary>
		public Type AssetType
		{
			get
			{
				return _assetType;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException()
			: base("Failed to load an asset.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string message, string assetName, Type assetType)
			: base(message)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string message, string assetName, Type assetType, Exception innerException)
			: base(message, innerException)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string assetName, Type assetType)
			: base(string.Format("Failed to load {0} with name {1}.", assetType, assetName))
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(string assetName, Type assetType, Exception innerException)
			: base(string.Format("Failed to load {0} with name {1}.", assetType, assetName), innerException)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(Type assetType)
			: base(string.Format("Failed to load {0}.", assetType))
		{
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UnityAssetLoadException"/> class.
		/// </summary>
		public UnityAssetLoadException(Type assetType, Exception innerException)
			: base(string.Format("Failed to load {0}.", assetType), innerException)
		{
			_assetType = assetType;
		}

		#endregion
	}
}
