.NET Testing Best Practices
===========================

[![.NET Testing Best Practices](https://github.com/robrich/net-testing-xunit-moq/actions/workflows/done.yml/badge.svg)](https://github.com/robrich/net-testing-xunit-moq/actions/workflows/done.yml)

This is the code that supports the live demo of writing unit tests in C# and .NET using [xUnit](https://xunit.net/docs/getting-started/netcore/cmdline).

Along the way, we'll pull in these dependencies:
- [Shouldly](https://docs.shouldly.org/) for assertions
- [NSubstitute](https://nsubstitute.github.io/help/getting-started/) for mocking
- [AutoFixture](https://blog.ploeh.dk/2010/08/19/AutoFixtureasanauto-mockingcontainer/) as a testing IoC container


See also

- [Fluent Assertions -> Shouldly Migration Guide](https://github.com/shouldly/shouldly/issues/1034)
- [AutoFixture can't instantiate Controllers](https://github.com/AutoFixture/AutoFixture/issues/1141): `var controller = fixture.Build<SomeController>().OmitAutoProperties().Create();`


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
