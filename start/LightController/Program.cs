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

public class LightSwitcherInstance : LightSwitcher.ILightSwitcher
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
        LightSwitcher.Init(new LightSwitcherInstance());
        IMotionSensor motionSensor = new MotionSensorInstance();
        ILightActuator lightActuator = new LightActuator();
        LightController controller = new LightController(motionSensor, lightActuator);
        while(true)
        {
            Thread.Sleep(100);
        }
    }
}
