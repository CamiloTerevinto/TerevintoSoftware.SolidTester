# TerevintoSoftware.SolidTester
[![Nuget version](https://img.shields.io/nuget/v/TerevintoSoftware.SolidTester.Tool)](https://www.nuget.org/packages/TerevintoSoftware.SolidTester.Tool/)

This project aims to provide a way for c# developers to quickly create Unit Tests for an entire project at once.  
This is meant to help people that have a solution without any tests to quickly get up and running, avoiding hours of writing repetitive boilerplate code.

This currently supports only NUnit (v3) and Moq mocks, and it was tested with fairly simple/common scenarios (see the SampleLibrary folder).

## Packages

This project is divided in two packages:

| Package | Purpose |
| ------- | ------- |
| [TerevintoSoftware.SolidTester][1] | Contains the main logic of the project. |
| [TerevintoSoftware.SolidTester.Tool][2] | Contains a .NET Tool that can be invoked to perform the generation. |

## Sample usage

1. Install the tool: `dotnet tool install TerevintoSoftware.SolidTester.Tool`
2. (optional) See the available options with `solid-tester -h`
3. Run the static site generation: `solid-tester --assembly "path-to-assembly" --output "path-to-output" --base-namespace YourLibrary.Tests`

## How to build

* Install Visual Studio 2022 (.NET 6 required), if needed. 
* Install git, if needed.
* Clone this repository.
* Build from Visual Studio or through `dotnet build`.

### Running tests

Once the solution is compiled, tests can be run either from Visual Studio's Test Explorer window, or through `dotnet test`.

## License

The .NET Tool and this solution are licensed under the [MIT license](/LICENSE).

## Bug reports and feature requests

Please use the [issue tracker](https://github.com/CamiloTerevinto/TerevintoSoftware.SolidTester/issues) and ensure your question/feedback was not previously reported.

[1]: https://www.nuget.org/packages/TerevintoSoftware.SolidTester/
[2]: https://www.nuget.org/packages/TerevintoSoftware.SolidTester.Tool/
