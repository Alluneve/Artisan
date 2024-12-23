using ECommons.DalamudServices;
using ECommons.Reflection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Artisan.RawInformation
{
    internal static class DalamudInfo
    {
        public static bool IsOnStaging()
        {
            return false;
        }
    }
}
