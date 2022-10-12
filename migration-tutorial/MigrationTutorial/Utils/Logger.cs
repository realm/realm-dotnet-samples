using System;
namespace MigrationTutorial.Utils
{
    public static class Logger
    {
        public static void LogInfo(string message)
        {
            Console.WriteLine($"Info - {message}");
        }

        public static void LogDebug(string message)
        {
#if DEBUG
            Console.WriteLine($"Debug - {message}");
#endif
        }

        public static void LogWarning(string message)
        {
            Console.WriteLine($"Warning - {message}");
        }

        public static void LogError(string message)
        {
            Console.WriteLine($"Error - {message}");
        }

        public static string GetHelpString()
        {
            return @"
MigrationTutorial --schema_version [1-3]

--schema_version : specify the version schema from 1 to 3

Note that:
1) Schema versions need to be passed in order from 1 to 3. Any other order won't work.";
        }
    }
}
