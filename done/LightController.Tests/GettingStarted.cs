namespace LightSensorApp.Tests;

public class GettingStarted
{
    [Fact]
    public void HelloWorldTest()
    {
        true.Should().Be(true);
    }

    [Fact]
    public void ArrangeActAssertPattern()
    {

        // Arrange
        int x = 2;
        int y = 3;
        int expected = 5;

        // Act
        int actual = x + y;

        // Assert
        //Assert.Equal(expected, actual);
        actual.Should().Be(expected);

    }
}
