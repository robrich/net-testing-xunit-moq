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
        var mocker = new AutoMocker();

        // Act
        LightActuator actuator = mocker.CreateInstance<LightActuator>();
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
        var mocker = new AutoMocker();

        // Act
        LightActuator actuator = mocker.CreateInstance<LightActuator>();
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
    public void MotionDetectedAndDark_TurnItOn(TimePeriod timeOfDay, bool expected)
    {
        // Arrange
        bool motionDetected = true;
        bool turnedOn = false;

        // Mocks
        AutoMocker mocker = new AutoMocker();
        SetupTimePeriodHelper(mocker, timeOfDay);
        SetupLightSwitcher_TurnOn(mocker, () => turnedOn = true);

        // Act
        LightActuator actuator = mocker.Get<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        turnedOn.Should().Be(expected);
        //mocker.GetMock<ILightSwitcher>().Verify(m => m.TurnOn(), Times.Once());
        
    }

    [Fact]
    public void NoMotionDetectedAndDark_DontTurnOn()
    {
        // Arrange
        bool motionDetected = false;
        bool turnedOn = false;
        bool expected = false;

        // Mocks
        AutoMocker mocker = new AutoMocker();
        SetupTimePeriodHelper(mocker, TimePeriod.Night);
        SetupLightSwitcher_TurnOn(mocker, () => turnedOn = true);

        // Act
        LightActuator actuator = mocker.Get<LightActuator>();
        actuator.ActuateLights(motionDetected);

        // Assert
        turnedOn.Should().Be(expected);
        //mocker.GetMock<ILightSwitcher>().Verify(m => m.TurnOn(), Times.Never());

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
        AutoMocker mocker = new AutoMocker();
        SetupTimePeriodHelper(mocker, TimePeriod.Evening);
        SetupLightSwitcher_TurnOff(mocker, () => turnedOff = true);
        SetupCurrentTimeHelper(mocker, currentTime);

        // Act
        LightActuator actuator = mocker.Get<LightActuator>();
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
        AutoMocker mocker = new AutoMocker();
        SetupTimePeriodHelper(mocker, timeOfDay);
        SetupLightSwitcher_TurnOff(mocker, () => turnedOff = true);
        
        // Act
        LightActuator actuator = mocker.Get<LightActuator>();
        actuator.ActuateLights(motionDetected: false);

        // Assert
        turnedOff.Should().Be(expected);
    }

    private void SetupTimePeriodHelper(AutoMocker mocker, TimePeriod timeOfDay)
    {
        var timePeriodHelper = new Mock<ITimePeriodHelper>();
        timePeriodHelper
            .Setup(m => m.GetTimePeriod(It.IsAny<DateTime>()))
            .Returns(timeOfDay);
        mocker.Use(timePeriodHelper);
    }

    private void SetupLightSwitcher_TurnOn(AutoMocker mocker, Action turnedOnCallback)
    {
        var lightSwitcher = new Mock<ILightSwitcher>();
        lightSwitcher
            .Setup(m => m.TurnOn())
            .Callback(turnedOnCallback);
        mocker.Use(lightSwitcher);
    }

    private void SetupLightSwitcher_TurnOff(AutoMocker mocker, Action turnedOffCallback)
    {
        var lightSwitcher = new Mock<ILightSwitcher>();
        lightSwitcher
            .Setup(m => m.TurnOff())
            .Callback(turnedOffCallback);
        mocker.Use(lightSwitcher);
    }

    private void SetupCurrentTimeHelper(AutoMocker mocker, DateTime currentTime)
    {
        var currentTimeHelper = new Mock<ICurrentTimeHelper>();
        currentTimeHelper
            .Setup(m => m.GetNow())
            .Returns(currentTime);
        mocker.Use(currentTimeHelper);
    }

}
