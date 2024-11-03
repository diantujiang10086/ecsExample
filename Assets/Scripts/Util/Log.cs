using System;

public static class Log
{
    public static void Debug(string msg)
    {
        Unity.Logging.Log.Debug(msg);
    }
    public static void Error(string msg)
    {
        Unity.Logging.Log.Error(msg);
    }
    public static void Error(Exception msg)
    {
        Unity.Logging.Log.Error(msg);
    }
    public static void Warning(string msg)
    {
        Unity.Logging.Log.Warning(msg);
    }
}