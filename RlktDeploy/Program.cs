using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RlktDeploy
{
    internal class Program
    {
        private const string strConfigFile = ".\\RevInc.ini";
        private const string strDeployConfigFile = ".\\RlktDeploy.ini";
        private const string finalBuildDir = "Latest\\";

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string Section,
            string Key,
            string Default,
            StringBuilder RetVal,
            int Size,
            string FilePath);

        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                for (int index = 0; index < 3; ++index)
                    Console.WriteLine("!! ERROR !!");
                Console.WriteLine("Invalid arguments. Use program.exe <PATH_TO_PDB> <SERVICE_TYPE>\n");
            }
            else
            {
                int num = int.Parse(args[1]);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (!File.Exists(".\\RevInc.ini"))
                {
                    for (int index = 0; index < 3; ++index)
                        Console.WriteLine("!! ERROR !!");
                    Console.WriteLine(".\\RevInc.ini Not found!");
                }
                else if (!File.Exists(".\\RlktDeploy.ini"))
                {
                    for (int index = 0; index < 3; ++index)
                        Console.WriteLine("!! ERROR !!");
                    Console.WriteLine(".\\RlktDeploy.ini Not found!");
                }
                else
                {
                    int result = 0;
                    StringBuilder RetVal1 = new StringBuilder();
                    Program.GetPrivateProfileString("RLKT_REV", "VersionTextFilePath", "", RetVal1, (int)byte.MaxValue, ".\\RevInc.ini");
                    StringBuilder RetVal2 = new StringBuilder();
                    Program.GetPrivateProfileString("RLKT_REV", "PDBOutputPath", "", RetVal2, (int)byte.MaxValue, ".\\RevInc.ini");
                    if (File.Exists(RetVal1.ToString()))
                    {
                        string s = File.ReadAllText(RetVal1.ToString());
                        Console.WriteLine("Version inside file: " + s);
                        int.TryParse(s, out result);
                    }
                    bool flag = false;
                    if (!Directory.Exists(RetVal2.ToString()))
                    {
                        Console.WriteLine("DIR " + RetVal2.ToString() + " Does not exists");
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(RetVal2.ToString() + result.ToString());
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception: %s", (object)ex.Message);
                        }
                    }
                    Program.GetPrivateProfileString("Deployment", "CMD_TO_RUN", "", new StringBuilder(), (int)byte.MaxValue, ".\\RlktDeploy.ini");
                    StringBuilder RetVal3 = new StringBuilder();
                    Program.GetPrivateProfileString("Deployment", "CMD_PARAMS", "", RetVal3, (int)byte.MaxValue, ".\\RlktDeploy.ini");
                    StringBuilder RetVal4 = new StringBuilder();
                    Program.GetPrivateProfileString("Deployment", "DEPLOY_PATH_CLIENT", "", RetVal4, (int)byte.MaxValue, ".\\RlktDeploy.ini");
                    StringBuilder RetVal5 = new StringBuilder();
                    Program.GetPrivateProfileString("Deployment", "DEPLOY_PATH_SERVER", "", RetVal5, (int)byte.MaxValue, ".\\RlktDeploy.ini");
                    string str1 = args[0] + ".exe";
                    string str2 = RetVal4.ToString() + "Latest\\" + Path.GetFileName(str1);
                    if (RetVal3.ToString().IndexOf("{INPUT}") > -1)
                    {
                        string str3 = RetVal3.ToString();
                        RetVal3.Clear();
                        RetVal3.Append(str3.Replace("{INPUT}", str1));
                    }
                    if (RetVal3.ToString().IndexOf("{OUTPUT}") > -1)
                    {
                        string str3 = RetVal3.ToString();
                        RetVal3.Clear();
                        RetVal3.Append(str3.Replace("{OUTPUT}", str2));
                    }
                    DateTime today = DateTime.Today;
                    string str4 = today.Year.ToString() + "-" + (object)today.Month + "-" + (object)today.Day;
                    if (num == 0)
                    {
                        if (!Directory.Exists(RetVal4.ToString() + result.ToString()))
                        {
                            try
                            {
                                Directory.CreateDirectory(RetVal4.ToString() + result.ToString());
                                flag = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: %s", (object)ex.Message);
                            }
                        }
                        if (!Directory.Exists(RetVal4.ToString() + "Latest\\"))
                        {
                            try
                            {
                                Directory.CreateDirectory(RetVal4.ToString() + "Latest\\");
                                flag = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: %s", (object)ex.Message);
                            }
                        }
                    }
                    else if (!Directory.Exists(RetVal5.ToString() + str4.ToString() + "\\"))
                    {
                        try
                        {
                            Directory.CreateDirectory(RetVal5.ToString() + str4.ToString());
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception: %s", (object)ex.Message);
                        }
                    }
                    Console.WriteLine("!! EXEC CMD !! -- This may take a while ...");
                    if (num == 0)
                    {
                        if (flag && File.Exists(str1))
                        {
                            File.Copy(str1, str2, true);
                            File.Copy(str1, RetVal4.ToString() + result.ToString() + "\\" + Path.GetFileName(str1), true);
                        }
                    }
                    else if (flag)
                    {
                        string destFileName = RetVal5.ToString() + str4.ToString() + "\\" + Path.GetFileName(str2);
                        if (File.Exists(str2))
                            File.Copy(str2, destFileName, true);
                    }
                    Console.WriteLine("Done !");
                }
            }
        }
    }
}