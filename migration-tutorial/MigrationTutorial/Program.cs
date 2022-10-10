using System;
using MigrationTutorial.Services;
using MigrationTutorial.Utils;

namespace MigrationTutorial
{
    class Program
    {
        static int Main()
        {
            try
            {
                RealmService.Init();
                SeedData.Seed();
                return 0;
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception {e} was encountered.");
                return 1;
            }
        }
    }
}
