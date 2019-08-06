/*
 * Created by SharpDevelop.
 * User: mifus_000
 * Date: 20/05/2017
 * Time: 15:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

/// <summary>
/// Description of Logger.
/// </summary>
public class Logger
{
    public Logger()
    {
    }


    public static void loggerindicators(string value)
    {
        try
        {
            value = "[" + DateTime.Now.ToString() + "] - " + value;
            Console.WriteLine(value);

            System.IO.StreamWriter w = new System.IO.StreamWriter(Program.location + @"\logIndicators\" + DateTime.Now.ToString("yyyyMMdd") + "loggerIndicators.txt", true);
            w.WriteLine(value);
            w.Close();
            w.Dispose();
            w = null;
        }
        catch
        {

        }
    }

    public static void log(string value)
    {
        value = "[" + DateTime.Now.ToString() + "] - " + value;

        Console.WriteLine(value);

        try
        {

            System.IO.StreamWriter w = new StreamWriter(Program.location + @"\log\" + DateTime.Now.ToString("yyyyMMdd") + "_log.txt", true);
            w.WriteLine(value);
            w.Close();
            w.Dispose();
            w = null;

        }
        catch
        { }

    }
}
