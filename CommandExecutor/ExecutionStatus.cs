using System;

namespace CommandExecutor;

public class ExecutionStatus
{
    public string StatusMessage { get; private set; } = "Idle";
    public int Progress { get; private set; } = 0; // 0–100

    public void SetStatus(string message)
    {
        StatusMessage = message;
    }

    public void SetProgress(int progress)
    {
        Progress = Clamp(progress, 0, 100);
    }

    public void Reset()
    {
        StatusMessage = "Idle";
        Progress = 0;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}