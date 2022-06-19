//This code is part of an imaginary home automation system.
//It is used as part of a light controller that polls a motion sensor
//and then calls the `ActuateLights` method with the results from the
//motion sensor to determine if the lights should be turned on or off.

namespace LightController;

public interface ILightSwitcher
{
    void TurnOn();

    void TurnOff();
}

public interface IMotionSensor
{
    bool DetectMotion();
}

public interface ICurrentTimeHelper
{
    DateTime GetNow();
}

public class CurrentTimeHelper : ICurrentTimeHelper
{
    public DateTime GetNow() => DateTime.Now;
}

public interface ILightActuator
{
    void ActuateLights(bool motionDetected);
}

public class LightActuator : ILightActuator
{
    private readonly ILightSwitcher lightSwitcher;
    private readonly ITimePeriodHelper timePeriodHelper;
    private readonly ICurrentTimeHelper currentTimeHelper;

    public LightActuator(ILightSwitcher lightSwitcher, ITimePeriodHelper timePeriodHelper, ICurrentTimeHelper currentTimeHelper)
    {
        this.lightSwitcher = lightSwitcher ?? throw new ArgumentNullException(nameof(lightSwitcher));
        this.timePeriodHelper = timePeriodHelper ?? throw new ArgumentNullException(nameof(timePeriodHelper));
        this.currentTimeHelper = currentTimeHelper ?? throw new ArgumentNullException(nameof(currentTimeHelper));
    }

    // public for testing, not part of interface
    public DateTime LastMotionTime { get; set; }

    public void ActuateLights(bool motionDetected)
    {
        DateTime time = currentTimeHelper.GetNow();

        // Update the time of last motion.
        if (motionDetected)
        {
            LastMotionTime = time;
        }

        // If motion was detected in the evening or at night, turn the light on.
        TimePeriod timePeriod = timePeriodHelper.GetTimePeriod(time);
        if (motionDetected && (timePeriod == TimePeriod.Evening || timePeriod == TimePeriod.Night))
        {
            lightSwitcher.TurnOn();
        }
        // If no motion is detected for one minute, or if it is morning or day, turn the light off.
        else if (time.Subtract(LastMotionTime) > TimeSpan.FromMinutes(1) || (timePeriod == TimePeriod.Morning || timePeriod == TimePeriod.Afternoon))
        {
            lightSwitcher.TurnOff();
        }
    }

}

public interface ITimePeriodHelper
{
    TimePeriod GetTimePeriod(DateTime dateTime);
}

public enum TimePeriod
{
    Morning,
    Afternoon,
    Evening,
    Night
}

public class TimePeriodHelper : ITimePeriodHelper
{

    public TimePeriod GetTimePeriod(DateTime dateTime)
    {
        if (dateTime.Hour >= 0 && dateTime.Hour < 6)
        {
            return TimePeriod.Night;
        }
        if (dateTime.Hour >= 6 && dateTime.Hour < 12)
        {
            return TimePeriod.Morning;
        }
        if (dateTime.Hour >= 12 && dateTime.Hour < 18)
        {
            return TimePeriod.Afternoon;
        }
        return TimePeriod.Evening;
    }
}
