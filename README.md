# UnityFx.Tasks

**Requires Unity 2017.1 or higher.**

## Synopsis

*UnityFx.Tasks* provides a set of extension methods and utilities to make using [async/await](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) in [Unity3d](https://unity3d.com) more convenient.

## Getting Started
### Prerequisites
You may need the following software installed in order to build/use the library:
- [Microsoft Visual Studio 2017](https://www.visualstudio.com/vs/community/).
- [Unity3d](https://store.unity.com/) (the minimum supported version is **2017.1**).

### Getting the code
You can get the code by cloning the github repository using your preffered git client UI or you can do it from command line as follows:
```cmd
git clone https://github.com/Arvtesh/UnityFx.Tasks.git
git submodule -q update --init
```
### Getting binaries
The [Unity Asset Store package](https://assetstore.unity.com/packages/tools/tt) can be installed using the editor. One can also download it directly from [Github releases](https://github.com/Arvtesh/UnityFx.Tasks/releases)

## Using the library
TODO

## Motivation
The project was initially created to help author with his [Unity3d](https://unity3d.com) projects. Unity doesn't adapt their APIs to the [async/await programming](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/) and that requires additional effors to make it usable. Having experience with that kind of stuff with [UnityFx.Async](https://github.com/Arvtesh/UnityFx.Async) I decided to make a very minimal set of tools just for this purpose.

## Documentation
Please see the links below for extended information on the product:
- [Unity forums](https://forum.unity.com/threads/tt).
- [Documentation](https://arvtesh.github.io/UnityFx.Tasks/articles/intro.html).
- [API Reference](https://arvtesh.github.io/UnityFx.Tasks/api/index.html).
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
