.NET Testing Best Practices
===========================

[![.NET Testing Best Practices](https://github.com/robrich/net-testing-xunit-moq/actions/workflows/done.yml/badge.svg)](https://github.com/robrich/net-testing-xunit-moq/actions/workflows/done.yml)

This is the code that supports the live demo of writing unit tests in C# and .NET using [xUnit](https://xunit.net/docs/getting-started/netcore/cmdline).

Along the way, we'll pull in these dependencies:
- [MoQ](https://github.com/Moq/moq4) for mocking,
- [AutoMocker](https://github.com/moq/Moq.AutoMocker) as a testing IoC container

About this code
---------------

At the beginning of the talk we start with the `start` folder.

At the end of the talk we'll finish with the `done` folder.

Running the tests
-----------------

```sh
cd done
dotnet restore
dotnet build
dotnet test
```

License
-------

License: MIT
