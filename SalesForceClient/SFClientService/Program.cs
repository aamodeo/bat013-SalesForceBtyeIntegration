﻿namespace SFClientService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var service = new SFService();
            service.DebugStartAndStop(args);
        }
    }
}
