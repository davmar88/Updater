using System;
using System.IO;
using System.IO.Compression;
using Renci.SshNet;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Updater
{
    class MainClass
    {
        static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            Console.WriteLine("Updater running");

            StartChmodFiles();


            while (!tokenSource.Token.IsCancellationRequested)
            {

                Console.WriteLine("Opaque Updater running....waiting for any updates available.");
                string host = @"102.37.1.251";
                string username = "Pepla";
                string password = "Development@101";
                try
                {
                    UpdateOpenAirPackage(host,username,password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem with UpdateOpenAirPackage(); {0}", ex.InnerException.Message);
                }

                try
                {
                    UpdateObserver(host, username, password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem with UpdateObserverPackage(); {0}", ex.InnerException.InnerException.Message);
                }

                try
                {
                    UpdateInstaller(host, username, password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem with UpdateInstallerPackage(); {0}", ex.InnerException.InnerException.Message);
                }
                try
                {
                    UpdateDatabase(host, username, password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem with UpdateDatabasePackage(); {0}", ex.InnerException.InnerException.Message);
                }

                Console.WriteLine("Waiting 1 minute");
                Thread.Sleep(60000);//every 5 minutes.;
            }

            Console.ReadKey();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            tokenSource.Cancel();
        }

        public static void UpdateOpenAirPackage(string host, string username, string password)
        {

            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            string remoteDirectory = "/home/Pepla/Desktop/Opaque/";
            string localDirectory = home;
            DirectoryInfo folder = new DirectoryInfo(home);
            FileInfo[] files = folder.GetFiles();
            DateTime updatedTime = DateTime.Now;
            foreach (FileInfo file in files)
            {
                if (file.Name == "githubdownloads.zip")
                {
                    updatedTime = file.LastWriteTime.Date;
                    Console.WriteLine(file);
                    Console.WriteLine("Last updated time: "+updatedTime);
                }
            }
            using (var client = new SftpClient(host, username, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (client.IsConnected)
                {
                    var onlinefiles = client.ListDirectory(remoteDirectory);
                    foreach (var file in onlinefiles)
                    {
                        string remoteFileName = file.Name;
                        if (file.Name == "githubdownloads.zip")
                        {
                            if(file.LastWriteTime > updatedTime)
                            {
                                if (File.Exists(home + "/githubdownloads.zip"))
                                {
                                    File.Move(home + "/githubdownloads.zip", home + "/githubdownloads.zip.old");
                                }
                                var downloadfile = File.OpenWrite(localDirectory);

                                Console.WriteLine("Found a new update! Busy downloading githubdownloads.zip, please be patient");
                                client.DownloadFile(remoteDirectory + remoteFileName, downloadfile);
                                Console.WriteLine("Done Downloading!");
                                downloadfile.Close();

                                KillObserver();
                                KillLTEMMEScripts();
                                KillHSSScript();

                                if (Directory.Exists(home + "/githubdownloads"))
                                {


                                    if (Directory.Exists(home + "/githubdownloads.old"))
                                    {

                                    }
                                    else
                                    {
                                        Directory.Move(home + "/githubdownloads", home + "/githubdownloads.old");
                                    }
                                    //
                                }
                                if (File.Exists(home + "/githubdownloads.zip"))
                                {
                                    string extractPath = home + "/";
                                    Console.WriteLine("\n");
                                    string zipPath = home + "/githubdownloads.zip";
                                    Console.WriteLine("Busy extracting githubdownloads.zip and setting up environment, please be patient");
                                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                                    Console.WriteLine("\n");
                                    File.Delete(home + "/githubdownloads.zip.old");
                                    DeleteDirectory(home + "/githubdownloads.old");
                                    Console.WriteLine("Done with setup of Githubdownloads files environment, restarting computer");
                                    Console.WriteLine("-------------------------------------------------------------");
                                    Console.WriteLine("\n");

                                    StartChmodFiles();

                                    RunCommands("sudo shutdown -r now");


                                }
                            }
                            else
                            {
                                Console.WriteLine("No Githubdownload Update out yet......");
                            }

                        }

                    }
                }
                client.Disconnect();
            }

        }



        public static void UpdateObserver(string host, string username, string password)
        {

            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            string remoteDirectory = "/home/Pepla/Desktop/Opaque/";
            string localDirectory = home;
            DirectoryInfo folder = new DirectoryInfo(home);
            FileInfo[] files = folder.GetFiles();
            DateTime updatedTime = DateTime.Now;
            foreach (FileInfo file in files)
            {
                if (file.Name == "Observer.zip")
                {
                    updatedTime = file.LastWriteTime.Date;
                    Console.WriteLine(file);
                    Console.WriteLine("Last updated time: " + updatedTime);
                }
            }
            using (var client = new SftpClient(host, username, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (client.IsConnected)
                {
                    var onlinefiles = client.ListDirectory(remoteDirectory);
                    foreach (var file in onlinefiles)
                    {
                        string remoteFileName = file.Name;
                        if (file.Name == "Observer.zip")
                        {

                            if(file.LastWriteTime > updatedTime)
                            {
                                if (File.Exists(home + "/Observer.zip"))
                                {
                                    File.Move(home + "/Observer.zip", home + "/Observer.zip.old");
                                }
                                var downloadfile = File.OpenWrite(localDirectory);

                                Console.WriteLine("Found a new update! Busy downloading Observer.zip, please be patient");
                                client.DownloadFile(remoteDirectory + remoteFileName, downloadfile);
                                Console.WriteLine("Done Downloading!");
                                downloadfile.Close();

                                KillObserver();
                                KillLTEMMEScripts();
                                KillHSSScript();

                                if (Directory.Exists(home + "/Observer"))
                                {

                                    if (Directory.Exists(home + "/Observer.old"))
                                    {

                                    }
                                    else
                                    {
                                        Directory.Move(home + "/Observer", home + "/Observer.old");
                                    }
                                    //
                                }
                                if (File.Exists(home + "/Observer.zip"))
                                {
                                    string extractPath = home + "/";
                                    Console.WriteLine("\n");
                                    string zipPath = home + "/Observer.zip";
                                    Console.WriteLine("Busy extracting Observer.zip and setting up environment, please be patient");
                                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                                    Console.WriteLine("\n");
                                    File.Delete(home + "/Observer.zip.old");
                                    DeleteDirectory(home + "/Observer.old");
                                    Console.WriteLine("Done with setup of Observer files environment, restarting computer");
                                    Console.WriteLine("-------------------------------------------------------------");
                                    Console.WriteLine("\n");

                                    StartChmodFiles();
                                    RunCommands("sudo shutdown -r now");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No update out yet for Observer.zip");
                            }

                        }

                    }
                }
                client.Disconnect();
            }

        }



        public static void UpdateDatabase(string host, string username, string password)
        {

            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            string remoteDirectory = "/home/Pepla/Desktop/Opaque/";
            string localDirectory = home;
            DirectoryInfo folder = new DirectoryInfo(home);
            FileInfo[] files = folder.GetFiles();
            DateTime updatedTime = DateTime.Now;
            foreach (FileInfo file in files)
            {
                if (file.Name == "all_databases.sql")
                {
                    updatedTime = file.LastWriteTime;
                    Console.WriteLine(file);
                    Console.WriteLine("Last write time: {0}", updatedTime);
                }
            }
            using (SftpClient client = new SftpClient(host, username, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (client.IsConnected)
                {
                    var onlinefiles = client.ListDirectory(remoteDirectory);
                    foreach (var file in onlinefiles)
                    {
                        string remoteFileName = file.Name;
                        if (file.Name == "all_databases.sql")
                        {
                            if(file.LastWriteTime > updatedTime)
                            {
                                if (File.Exists(home + "/all_databases.sql"))
                                {
                                    //File.Move(home + "/all_databases.sql", home + "/all_databases.sql.old");
                                }


                                //var downloadfile = File.Create(localDirectory);

                                using (var downloadfile = File.Create(localDirectory))
                                {
                                    Console.WriteLine("Found a new update! Busy downloading all_databases.sql, please be patient");
                                    client.DownloadFile(remoteDirectory + remoteFileName, downloadfile);
                                    Console.WriteLine("Done Downloading!");
                                    downloadfile.Close();
                                    downloadfile.Dispose();
                                }





                                KillObserver();
                                KillLTEMMEScripts();
                                KillHSSScript();
                                KillInstaller();

                                if (File.Exists(home + "/all_databases.sql"))
                                {

                                    Console.WriteLine("\n");

                                    Console.WriteLine("Busy preparing database and setting up environment, please be patient");
                                    File.Delete(home + "/all_databases.sql.old");
                                    Console.WriteLine("\n");
                                    Console.WriteLine("Done with setup of Database files environment, restarting computer");
                                    Console.WriteLine("-------------------------------------------------------------");
                                    Console.WriteLine("\n");

                                    //StartChmodFiles();
                                    RunCommands("mysql < all_databases.sql");
                                    RunCommands("sudo shutdown -r now");
                                }

                            }
                            else
                            {
                                Console.WriteLine("No Database Update out yet......");
                            }

                        }

                    }
                }

                client.Disconnect();
            }

        }



        public static void UpdateInstaller(string host, string username, string password)
        {

            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            string remoteDirectory = "/home/Pepla/Desktop/Opaque/";
            string localDirectory = home;
            DirectoryInfo folder = new DirectoryInfo(home);
            FileInfo[] files = folder.GetFiles();
            DateTime updatedTime = DateTime.Now;
            foreach (FileInfo file in files)
            {
                if (file.Name == "Installer.zip")
                {
                    updatedTime = file.LastWriteTime.Date;
                    Console.WriteLine(file);
                    Console.WriteLine("Last updated time: " + updatedTime);
                }
            }
            using (var client = new SftpClient(host, username, password))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                if (client.IsConnected)
                {
                    var onlinefiles = client.ListDirectory(remoteDirectory);
                    foreach (var file in onlinefiles)
                    {
                        string remoteFileName = file.Name;
                        if (file.Name == "Installer.zip")
                        {
                            if(file.LastWriteTime > updatedTime)
                            {
                                if (File.Exists(home + "/Installer.zip"))
                                {
                                    File.Move(home + "/Installer.zip", home + "/Installer.zip.old");
                                }

                                var downloadfile = File.OpenWrite(localDirectory);
                                Console.WriteLine("Found a new update! Busy downloading Installer.zip, please be patient");
                                client.DownloadFile(remoteDirectory + remoteFileName, downloadfile);
                                Console.WriteLine("Done Downloading!");
                                downloadfile.Close();


                                KillInstaller();

                                if (Directory.Exists(home + "/Installer"))
                                {
                                    if (Directory.Exists(home + "/Installer.old"))
                                    {

                                    }
                                    else
                                    {
                                        Directory.Move(home + "/Installer", home + "/Installer.old");
                                    }
                                    //
                                }
                                if (File.Exists(home + "/Installer.zip"))
                                {
                                    string extractPath = home + "/";
                                    Console.WriteLine("\n");
                                    string zipPath = home + "/Installer.zip";
                                    Console.WriteLine("Busy extracting Installer.zip and setting up environment, please be patient");
                                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                                    Console.WriteLine("\n");
                                    DeleteDirectory(home + "/Installer.old");
                                    File.Delete(home + "/Installer.zip.old");
                                    Console.WriteLine("Done with setup of Observer files environment, restarting computer");
                                    Console.WriteLine("-------------------------------------------------------------");
                                    Console.WriteLine("\n");

                                    StartChmodFiles();

                                    RunCommands("sudo shutdown -r now");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No Installer Update out yet......");
                            }

                        }

                    }
                }
                client.Disconnect();
            }

        }



































        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }


        public static void KillObserver()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            //home+"
            string line1 = null;
            List<string> lines = new List<string>();
            StreamReader reader = new StreamReader(home + "/Observer/configfiles/pid.txt");
            // read all the lines in the file and store them in the List
            while ((line1 = reader.ReadLine()) != null)
            {
                lines.Add(line1);
            }
            reader.Close();
            string command = string.Format("sudo kill -9 {0}", lines[0]);
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine(command);
            proc.StandardInput.WriteLine("exit");
            string line = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            proc.WaitForExit();
        }

        public static void KillInstaller()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            //home+"
            string line1 = null;
            List<string> lines = new List<string>();
            StreamReader reader = new StreamReader(home + "/Installer/configfiles/pid.txt");
            // read all the lines in the file and store them in the List
            while ((line1 = reader.ReadLine()) != null)
            {
                lines.Add(line1);
            }
            reader.Close();
            string command = string.Format("sudo kill -9 {0}", lines[0]);
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine(command);
            proc.StandardInput.WriteLine("exit");
            string line = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            proc.WaitForExit();
        }

        public static void KillHSSScript()
        {
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine("killall - 9 oai_hss");
            proc.StandardInput.WriteLine("killall -9 run_hss");
            proc.StandardInput.WriteLine("exit");
            string line = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            proc.WaitForExit();
        }

        public static void KillLTEMMEScripts()
        {
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine("killall -9 lte-softmodem");
            proc.StandardInput.WriteLine("killall - 9 oai_mme");
            proc.StandardInput.WriteLine("killall -9 run_mme");
            proc.StandardInput.WriteLine("exit");
            string line = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            proc.WaitForExit();
        }

        public static void StartObserver()
        {

            string command = string.Format("{0}", "./configfiles/start_observer.sh");
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "gnome-terminal";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.Arguments = " -e  \" " + command + " \"";

            proc.Start();



        }

        public static void StartChmodFiles()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);

            string command = string.Format("{0}", home+"/start_opaque_chmod.sh");
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "gnome-terminal";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.Arguments = " -e  \" " + command + " \"";

            proc.Start();



        }

        public static void RunCommands(string command)
        {


            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();

            proc.StandardInput.WriteLine(command);

            proc.StandardInput.WriteLine("exit");
            string line = "";

            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

            proc.WaitForExit();
        }
    }
}
