using System;


/// <summary>
/// This class provides simple text logging from the core module to 
/// any listening front end app
/// </summary>    
public class Logging
{
    public enum LogLevels
    {
        DEBUG,
        WARNING,
        ERROR
    }

    public delegate void LogEventHandler(string s, LogLevels l);

    public event LogEventHandler Logged;

    public void Log(string s)
    {
        Logged(s, LogLevels.DEBUG);
    }

    public void LogWarn(string s)
    {
        Logged(s, LogLevels.WARNING);
    }

    public void LogError(string s)
    {
        Logged(s, LogLevels.ERROR);
    }

    private static Logging instance = null;
    public static Logging Instance
    {
        get
        {
            if (instance == null)
                instance = new Logging();

            return instance;
        }            
    }
}

