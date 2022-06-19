using Microsoft.Extensions.DependencyInjection;

namespace LightController;

public class LightController
{
    private readonly IMotionSensor MotionSensor;
    private readonly ILightActuator LightActuator;
    private readonly Timer timer;

    public LightController(IMotionSensor motionSensor, ILightActuator lightActuator)
    {
        MotionSensor = motionSensor;
        LightActuator = lightActuator;

        timer = new Timer
        {
            Enabled = true,
            Interval = 1000 // ms
        };
        timer.Elapsed += Poll;
    }

    public void Poll(object? source, EventArgs? e)
    {
        LightActuator.ActuateLights(MotionSensor.DetectMotion());
    }
}

public class LightSwitcherInstance : ILightSwitcher
{
    public void TurnOff()
    {
        Console.WriteLine("Setting lights to off");
    }

    public void TurnOn()
    {
        Console.WriteLine("Setting lights to on");
    }
}

public class MotionSensorInstance : IMotionSensor
{
    private bool motion;
    public bool DetectMotion()
    {
        motion = !motion;
        return motion;
    }
}

public class Program
{
    public static void Main()
    {

        var services = new ServiceCollection();

        services.AddSingleton<ILightSwitcher, LightSwitcherInstance>();
        services.AddSingleton<IMotionSensor, MotionSensorInstance>();
        services.AddSingleton<ITimePeriodHelper, TimePeriodHelper>();
        services.AddSingleton<ILightActuator, LightActuator>();
        services.AddSingleton<ICurrentTimeHelper, CurrentTimeHelper>();
        services.AddSingleton<LightController>();

        var container =  services.BuildServiceProvider();

        LightController controller = container.GetRequiredService<LightController>();
        
        while (true)
        {
            Thread.Sleep(100);
        }
    }
}
