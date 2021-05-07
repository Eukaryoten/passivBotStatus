using System;
using System.Diagnostics;
using System.IO;

namespace passivBotCheck {
    class Program {

        static DateTime lastStatusUpdate = DateTime.MinValue;
        static DateTime lastStatusCheck = DateTime.MinValue;
        static string windowTitle;
        static bool exitLoop = false;

        static void MonitorHost(Config config) {

            if (!File.Exists(config.HostStatusFile())) {
                LoopErrorExit("Host status file not found");
                return;
            }

            var lastWrite = File.GetLastWriteTime(config.HostStatusFile());
            var elapsedUpdateSeconds = DateTime.Now.Subtract(lastWrite).TotalSeconds;
            var elapsedCheckSeconds = DateTime.Now.Subtract(lastStatusCheck).TotalSeconds;

            if (elapsedCheckSeconds > config.CheckFrequency) {

                if (elapsedUpdateSeconds > config.Timeout) {
                    Log.Alert($"Timeout exceeded ({config.Timeout} seconds), executing {config.BatchPath}");
                    Process.Start(config.BatchPath);
                    config.Backup = false;
                } else {
                    lastStatusCheck = DateTime.Now;
                    Log.Status("Host running bot");
                }
            }

        }

        static void RunLocalBot(Config config) {

            var elapsedUpdateSeconds = DateTime.Now.Subtract(lastStatusUpdate).TotalSeconds;

            if (elapsedUpdateSeconds > config.UpdateFrequency) {

                Process[] myProcs = Process.GetProcesses();

                foreach (var entry in myProcs) {
                    if (entry.MainWindowTitle.Contains(windowTitle)) {
                        File.WriteAllText(config.LocalStatusFile(), config.Timeout.ToString());
                        Log.Status("Running Bot");
                        break;
                    }
                }

                lastStatusUpdate = DateTime.Now;
            }
        }

        static string CheckDirectory(string path) {
            if (Directory.Exists(path)) {
                return "FOUND";
            } else {
                return "MISSING";
            }
        }

        static string CheckFile(string path) {
            if (File.Exists(path)) {
                return "FOUND";
            }
            else {
                return "MISSING";
            }
        }

        static void LoopErrorExit(string message) {
            Log.Error($"{message}, exiting");
            exitLoop = true;
        }

        static void Main(string[] args) {

            Config config = Global.InitOptions<Config>();
            
            Console.WriteLine($"Batch Path: {config.BatchPath} = {CheckFile(config.BatchPath)}");
            Console.WriteLine($"Local Shared Folder: {config.LocalSharedFolder}");
            Console.WriteLine($"External Shared Folder: {config.ExternalSharedFolder} = {CheckDirectory(config.ExternalSharedFolder)}");
            Console.WriteLine($"Check Frequency: {config.CheckFrequency} seconds");
            Console.WriteLine($"Update Frequency: {config.UpdateFrequency} seconds");

            // Exit if we can't find shared folder

            if (!Directory.Exists(config.ExternalSharedFolder)) {
                Log.Error($"No folder {config.ExternalSharedFolder} found, exiting");
                goto InitializeFailure;
            }

            if (!File.Exists(config.BatchPath)) {
                Log.Error($"No batch file found, exiting");
                goto InitializeFailure;
            }

            windowTitle = File.ReadAllText(config.BatchPath);
            Log.Init($"Batch command read as {windowTitle}");

            // Initialize as backup

            if (config.Backup) {

                if (File.Exists(config.HostTimeoutFile())) {
                    var hostTimeoutString = File.ReadAllText(config.HostTimeoutFile());
                    int timeout;
                    if (int.TryParse(hostTimeoutString, out timeout)) {
                        config.Timeout = timeout;
                        Log.Init($"Timeout set to host timeout {config.Timeout} seconds");
                    }
                    else {
                        Log.Warning($"Timeout defaulted to {config.Timeout} seconds");
                    }
                }
                else {
                    Log.Error($"No host timout file found, exiting");
                    goto InitializeFailure;
                }
                Log.Init($"Initializing machine as backup");
            } 
            
            // Initialize as host
            
            else {
                File.WriteAllText(config.LocalTimeoutFile(), config.Timeout.ToString());
                Log.Init($"Initializing machine as host");
            }

            while (!exitLoop) {
                if (config.Backup) {
                    MonitorHost(config);
                }
                else {
                    RunLocalBot(config);
                }
            }

        InitializeFailure:
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
