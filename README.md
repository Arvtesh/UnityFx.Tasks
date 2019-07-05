# UnityFx.Tasks

Channel  | UnityFx.Tasks |
---------|---------------|
Github | [![GitHub release](https://img.shields.io/github/release/Arvtesh/UnityFx.Task.svg?logo=github)](https://github.com/Arvtesh/UnityFx.Task/releases)
Unity Asset Store | [![Task extensions for Unity](https://img.shields.io/badge/tools-v0.2.0-green.svg)](https://assetstore.unity.com/packages/slug/143705)

**Requires Unity 2017.2 or higher.**

## Synopsis

At this moment [Unity3d](https://unity3d.com) does not provide support neither for [Tasks](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming) nor for [async/await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/). The goal of the library is closing this gap and make [async/await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) usage in [Unity3d](https://unity3d.com) a viable option.

## Getting Started
### Prerequisites
You may need the following software installed in order to use the library:
- [Unity3d](https://store.unity.com/) (the minimum supported version is **2017.2**).

### Getting the code
You can get the code by cloning the github repository using your preffered git client UI or you can do it from command line as follows:
```cmd
git clone https://github.com/Arvtesh/UnityFx.Tasks.git
git submodule -q update --init
```
### Getting binaries
The [Unity Asset Store package](https://assetstore.unity.com/packages/slug/143705) can be installed using the editor. One can also download it directly from [Github releases](https://github.com/Arvtesh/UnityFx.Tasks/releases).

### UPM package

UPM package source is available at [Github](https://github.com/Arvtesh/UnityFx.Tasks/tree/upm). To use it, add the following line to dependencies section of your `manifest.json`. Unity should download and link the package automatically:
```json
{
  "dependencies": {
    "com.arvtesh.tasks": "https://github.com/Arvtesh/UnityFx.Tasks.git#upm"
  }
}
```

## Using the library
The library tools are locates in a single namespace:
```csharp
using UnityFx.Tasks;
```
An essential part of the code is implementation of awaiters for built-in Unity async operations:
```csharp
await new WaitForSeconds(1);
await UnityWebRequestAssetBundle.GetAssetBundle(url);
await StartCoroutine(SomeCoroutine());
await SceneManager.LoadSceneAsync("myScene");
```
There are also [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) conversions for standard operations:
```csharp
var task = UnityWebRequestAssetBundle.GetAssetBundle(url).ToTask<AssetBundle>();
var assetBundle = await task;
```
There are a number of `ConfigureAwait` extensions that allow for `await` configuration. It is more effective to use them instead of `ToTask` conversions when all you need is an awaitable object:
```csharp
public static async Task<AssetBundle> LoadAssetBundleAsync(string url)
{
	using (var www = UnityWebRequestAssetBundle.GetAssetBundle(url))
	{
		return await www.SendWebRequest().ConfigureAwait<AssetBundle>();
	}
}
```
There are a lot of utility methods provided. For instance, the following sample demonstrates loading a scene packed in an asset bundle:
```csharp
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityFx.Tasks;

public static async Task<Scene> LoadSceneFromAssetBundleAsync(string url)
{
	using (var www = UnityWebRequestAssetBundle.GetAssetBundle(url))
	{
		var assetBundle = await www.SendWebRequest().ConfigureAwait<AssetBundle>();

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
A scene can be loaded like this:
```csharp
var scene = await TaskUtility.LoadSceneAsync("myScene");
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
