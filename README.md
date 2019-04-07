# UnityFx.Tasks

**Requires Unity 2017.2 or higher.**

## Synopsis

*UnityFx.Tasks* provides a set of extension methods and utilities to make [async/await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) in [Unity3d](https://unity3d.com) available.

## Getting Started
### Prerequisites
You may need the following software installed in order to build/use the library:
- [Microsoft Visual Studio 2017](https://www.visualstudio.com/vs/community/).
- [Unity3d](https://store.unity.com/) (the minimum supported version is **2017.2**).

### Getting the code
You can get the code by cloning the github repository using your preffered git client UI or you can do it from command line as follows:
```cmd
git clone https://github.com/Arvtesh/UnityFx.Tasks.git
git submodule -q update --init
```
### Getting binaries
The [Unity Asset Store package](https://assetstore.unity.com/packages/slug/143705) can be installed using the editor. One can also download it directly from [Github releases](https://github.com/Arvtesh/UnityFx.Tasks/releases)

## Using the library
Import the namespace:
```csharp
using UnityFx.Tasks;
```
The following sample demonstrates loading a scene packed in an asset bundle:
```csharp
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityFx.Tasks;

public static async Task<Scene> LoadSceneFromAssetBundle(string url)
{
	using (var www = UnityWebRequestAssetBundle.GetAssetBundle(url))
	{
		var assetBundle = await www.ConfigureAwait<AssetBundle>();

		try
		{
			return await assetBundle.LoadSceneTaskAsync(LoadSceneMode.Single);
		}
		finally
		{
			assetBundle.Unload(false);
		}
	}
}
```

Converting a coroutine to a task is easy:
```csharp
private IEnumerator SomeCoroutine(TaskCompletionSource<int> completionSource)
{
	yield return new WaitForSeconds(1);
	completionSource.TrySetResult(10);
}

// Start the coroutine. Note that you do not require a MonoBehaviour instance to do this.
var task = TaskUtility.FromCoroutine(SomeCoroutine);
```

## Motivation
The project was initially created to help author with his [Unity3d](https://unity3d.com) projects. Unity doesn't adapt their APIs to the [async/await programming](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) and that requires additional effors to make it usable. Having experience with that kind of stuff with [UnityFx.Async](https://github.com/Arvtesh/UnityFx.Async) I decided to make a very minimal set of tools just for this purpose.

## Documentation
Please see the links below for extended information on the product:
- [Unity forums](https://forum.unity.com/threads/tt).
- [CHANGELOG](CHANGELOG.md).
- [SUPPORT](.github/SUPPORT.md).

## Useful links
- [Task-based Asynchronous Pattern (TAP)](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap).
- [Asynchronous programming with async and await (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/).
- [.NET Task reference source](https://referencesource.microsoft.com/#mscorlib/System/threading/Tasks/Task.cs).

## Contributing
Please see [contributing guide](.github/CONTRIBUTING.md) for details.

## Versioning
The project uses [SemVer](https://semver.org/) versioning pattern. For the versions available, see [tags in this repository](https://github.com/Arvtesh/UnityFx.Tasks/tags).

## License
Please see the [![license](https://img.shields.io/github/license/Arvtesh/UnityFx.Tasks.svg)](LICENSE.md) for details.

## Acknowledgments
Working on this project is a great experience. Please see below a list of my inspiration sources (in no particular order):
* [.NET reference source](https://referencesource.microsoft.com/mscorlib/System/threading/Tasks/Task.cs.html). A great source of knowledge and good programming practices.
* [Another Unity async/await helpers project](https://github.com/modesttree/Unity3dAsyncAwaitUtil).
* Everyone who ever commented or left any feedback on the project. It's always very helpful.
