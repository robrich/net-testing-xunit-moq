namespace LightController.Tests;

public class LightActuator_ActuateLights
{

    [Fact]
    public void MotionDetected_UpdateLastMotionTime()
    {
        // Arrange
        bool motionDetected = true;
        DateTime startTime = new DateTime(2000, 1, 1); // random value

        // Mocks
        var ioc = new AutoMocker();

        // Act
        LightActuator actuator = ioc.CreateInstance<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected);
        DateTime actual = actuator.LastMotionTime;

        // Assert
        actual.Should().NotBe(startTime);
    }

    [Fact]
    public void MotionNotDetected_LastMotionTimeUnchanged()
    {
        // Arrange
        bool motionDetected = false;
        DateTime startTime = new DateTime(2000, 1, 1); // random value

        // Mocks
        var ioc = new AutoMocker();

        // Act
        LightActuator actuator = ioc.CreateInstance<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected);
        DateTime actual = actuator.LastMotionTime;

        // Assert
        actual.Should().Be(startTime);
    }

    [Theory]
    [InlineData(TimePeriod.Evening, true)]
    [InlineData(TimePeriod.Night, true)]
    [InlineData(TimePeriod.Morning, false)]
    [InlineData(TimePeriod.Afternoon, false)]
    public void DarkAndMotionDetected_TurnOn(TimePeriod timePeriod, bool expectedTurnedOn)
    {
        // Arrange
        bool motionDetected = true;
        bool actualTurnedOn = false;

        // Mocks
        AutoMocker ioc = new AutoMocker();
        SetupTimePeriodHelper(ioc, timePeriod);
        SetupLightSwitcher_TurnOn(ioc, () => actualTurnedOn = true);

        // Act
        LightActuator actuator = ioc.CreateInstance<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        actualTurnedOn.Should().Be(expectedTurnedOn);
        //ioc.GetMock<ILightSwitcher>().Verify(m => m.TurnOn(), Times.Once());
        
    }

    [Fact]
    public void NoMotionDetectedAndDark_DontTurnOn()
    {
        // Arrange
        bool motionDetected = false;
        bool turnedOn = false;
        bool expected = false;

        // Mocks
        AutoMocker ioc = new AutoMocker();
        SetupTimePeriodHelper(ioc, TimePeriod.Night);
        SetupLightSwitcher_TurnOn(ioc, () => turnedOn = true);

        // Act
        LightActuator actuator = ioc.Get<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        turnedOn.Should().Be(expected);
        //ioc.GetMock<ILightSwitcher>().Verify(m => m.TurnOn(), Times.Never());

    }

    [Theory]
    [InlineData(61, true)]
    [InlineData(500, true)]
    [InlineData(60, false)]
    [InlineData(30, false)]
    [InlineData(1, false)]
    public void BeenLongEnough_TurnItOff(int seconds, bool expected)
    {
        // Arrange
        bool turnedOff = false;

        // Mocks
        DateTime startTime = new DateTime(2000, 1, 1, 20, 0, 0);
        DateTime currentTime = startTime.AddSeconds(seconds);
        AutoMocker ioc = new AutoMocker();
        SetupTimePeriodHelper(ioc, TimePeriod.Evening);
        SetupLightSwitcher_TurnOff(ioc, () => turnedOff = true);
        SetupCurrentTimeHelper(ioc, currentTime);

        // Act
        LightActuator actuator = ioc.Get<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected: false);

        // Assert
        turnedOff.Should().Be(expected);
    }

    [Theory]
    [InlineData(TimePeriod.Evening, false)]
    [InlineData(TimePeriod.Night, false)]
    [InlineData(TimePeriod.Morning, true)]
    [InlineData(TimePeriod.Afternoon, true)]
    public void Light_TurnItOff(TimePeriod timeOfDay, bool expected)
    {
        // Arrange
        bool turnedOff = false;

        // Mocks
        AutoMocker ioc = new AutoMocker();
        SetupTimePeriodHelper(ioc, timeOfDay);
        SetupLightSwitcher_TurnOff(ioc, () => turnedOff = true);
        
        // Act
        LightActuator actuator = ioc.Get<LightActuator>();
        actuator.ActuateLights(motionDetected: false);

        // Assert
        turnedOff.Should().Be(expected);
    }

    private void SetupTimePeriodHelper(AutoMocker ioc, TimePeriod timeOfDay)
    {
        var timePeriodHelper = new Mock<ITimePeriodHelper>();
        timePeriodHelper
            .Setup(m => m.GetTimePeriod(It.IsAny<DateTime>()))
            .Returns(timeOfDay);
        ioc.Use(timePeriodHelper);

        /* or
        var timePeriodHelper = mocker.GetMock<ITimePeriodHelper>();
        timePeriodHelper.Setup(m => m.GetTimePeriod(It.IsAny<DateTime>())).Returns(timePeriod);
        */
    }

    private void SetupLightSwitcher_TurnOn(AutoMocker ioc, Action turnedOnCallback)
    {
        var lightSwitcher = new Mock<ILightSwitcher>();
        lightSwitcher
            .Setup(m => m.TurnOn())
            .Callback(turnedOnCallback);
        ioc.Use(lightSwitcher);

        /* or
        var lightSwitcher = mocker.GetMock<ILightSwitcher>();
        lightSwitcher.Setup(m => m.TurnOn()).Callback(callback);
        */
    }

    private void SetupLightSwitcher_TurnOff(AutoMocker ioc, Action turnedOffCallback)
    {
        var lightSwitcher = new Mock<ILightSwitcher>();
        lightSwitcher
            .Setup(m => m.TurnOff())
            .Callback(turnedOffCallback);
        ioc.Use(lightSwitcher);
    }

    private void SetupCurrentTimeHelper(AutoMocker ioc, DateTime currentTime)
    {
        var currentTimeHelper = new Mock<ICurrentTimeHelper>();
        currentTimeHelper
            .Setup(m => m.GetNow())
            .Returns(currentTime);
        ioc.Use(currentTimeHelper);
    }

}
