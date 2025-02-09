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
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());

        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected);
        DateTime actual = actuator.LastMotionTime;

        // Assert
        actual.ShouldNotBe(startTime);
    }

    [Fact]
    public void MotionNotDetected_LastMotionTimeUnchanged()
    {
        // Arrange
        bool motionDetected = false;
        DateTime startTime = new DateTime(2000, 1, 1); // random value

        // Mocks
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());

        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected);
        DateTime actual = actuator.LastMotionTime;

        // Assert
        actual.ShouldBe(startTime);
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
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());
        SetupTimePeriodHelper(ioc, timePeriod);
        SetupLightSwitcher_TurnOn(ioc, a => actualTurnedOn = true);

        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        actualTurnedOn.ShouldBe(expectedTurnedOn);
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
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());
        SetupTimePeriodHelper(ioc, TimePeriod.Night);
        SetupLightSwitcher_TurnOn(ioc, a => turnedOn = true);

        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        turnedOn.ShouldBe(expected);
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
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());
        SetupTimePeriodHelper(ioc, TimePeriod.Evening);
        SetupLightSwitcher_TurnOff(ioc, a => turnedOff = true);
        SetupCurrentTimeHelper(ioc, currentTime);

        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.LastMotionTime = startTime;
        actuator.ActuateLights(motionDetected: false);

        // Assert
        turnedOff.ShouldBe(expected);
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
        var ioc = new Fixture().Customize(new AutoNSubstituteCustomization());
        SetupTimePeriodHelper(ioc, timeOfDay);
        SetupLightSwitcher_TurnOff(ioc, a => turnedOff = true);
        
        // Act
        LightActuator actuator = ioc.Create<LightActuator>();
        actuator.ActuateLights(motionDetected: false);

        // Assert
        turnedOff.ShouldBe(expected);
    }

    private void SetupTimePeriodHelper(IFixture ioc, TimePeriod timeOfDay)
    {
        ITimePeriodHelper timePeriodHelper = Substitute.For<ITimePeriodHelper>();
        timePeriodHelper.GetTimePeriod(Arg.Any<DateTime>())
            .Returns(timeOfDay);
        ioc.Register<ITimePeriodHelper>(() => timePeriodHelper);

        /* or
        var timePeriodHelper = ioc.Freeze<ITimePeriodHelper>();
        timePeriodHelper.GetTimePeriod(Arg.Any<DateTime>()).Returns(timePeriod);
        */
    }

    private void SetupLightSwitcher_TurnOn(IFixture ioc, Action<CallInfo> turnedOnCallback)
    {
        var lightSwitcher = ioc.Freeze<ILightSwitcher>();
        lightSwitcher
            .When(m => m.TurnOn())
            .Do(turnedOnCallback);
    }

    private void SetupLightSwitcher_TurnOff(IFixture ioc, Action<CallInfo> turnedOffCallback)
    {
        var lightSwitcher = ioc.Freeze<ILightSwitcher>();
        lightSwitcher
            .When(m => m.TurnOff())
            .Do(turnedOffCallback);
    }

    private void SetupCurrentTimeHelper(IFixture ioc, DateTime currentTime)
    {
        var currentTimeHelper = ioc.Freeze<ICurrentTimeHelper>();
        currentTimeHelper.GetNow()
            .Returns(currentTime);
    }

}
