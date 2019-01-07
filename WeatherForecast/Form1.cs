using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using WeatherForecast.MyDBTableAdapters;

namespace WeatherForecast
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        protected void Logger(string log)
        {
            try
            {
                if (richTextBox1.Lines.Length > 400)
                {
                    richTextBox1.Text = "";
                }
                richTextBox1.Text += "[" + DateTime.Now.ToString() + "] " + (log) + " \n ======================= \n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
            }
            catch { }
        }
        public Cities getData(Cities ct)
        {

            try
            {
                Logger(ct.Name);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://artgroup.feeds.meteonews.net/forecasts/id/" + ct.Code + ".xml?lang=en&cumulation=24h&end=2d");
                request.Credentials = new System.Net.NetworkCredential("artgroup", "Ar+HIspGr0p");
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader readerResult = new StreamReader(stream);

                var result = readerResult.ReadToEnd();
                stream.Dispose();
                readerResult.Dispose();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);




                XmlNodeList elemListMin = xmlDoc.GetElementsByTagName("temp_min");
                if (elemListMin.Count == 2)
                {
                    ct.Min = elemListMin[0].InnerText;
                    ct.Min2 = elemListMin[1].InnerText;
                    Logger("Min1:" + ct.Min);
                    Logger("Min2:" + ct.Min2);
                }


                XmlNodeList elemListMax = xmlDoc.GetElementsByTagName("temp_max");
                if (elemListMax.Count == 2)
                {
                    ct.Max = elemListMax[0].InnerText;
                    ct.Max2 = elemListMax[1].InnerText;
                    Logger("Max1:" + ct.Max);
                    Logger("Max2:" + ct.Max2);

                }

                XmlNodeList elemListState = xmlDoc.GetElementsByTagName("txt");
                if (elemListState.Count == 2)
                {
                    ct.State = elemListState[0].InnerText;
                    ct.State2 = elemListState[1].InnerText;
                    Logger("State:" + ct.State);
                    Logger("State2:" + ct.State2);

                }
            }
            catch (Exception Exp)
            {
                ct.Max = "0";
                ct.Max2 = "0";
                ct.Min = "0";
                ct.Min2 = "0";
                ct.State = "0";
                ct.State2 = "0";
                richTextBox1.Text += Exp.Message + "  \n";
            }


            return ct;
        }
        public void Convert()
        {
            string DateTimeStr = string.Format("{0:0000}", DateTime.Now.Year) + "-" + string.Format("{0:00}", DateTime.Now.Month) + "-" + string.Format("{0:00}", DateTime.Now.Day) + "_" + string.Format("{0:00}", DateTime.Now.Hour) + "-" + string.Format("{0:00}", DateTime.Now.Minute) + "-" + string.Format("{0:00}", DateTime.Now.Second);
            DirectoryInfo Dir = new DirectoryInfo(ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim());
            Dir.Create();
            string DestFile = ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim() + ConfigurationSettings.AppSettings["OutPutFileName"].ToString().Trim() + "_" + DateTimeStr + ".mp4";
            string SourceFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + ConfigurationSettings.AppSettings["OutPutFileName"].ToString().Trim() + ".avi";

            Process proc = new Process();
            proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg";
            proc.StartInfo.Arguments = "-y -i "+SourceFile+"  -preset veryfast -c:a copy   -b 2200k \""+DestFile+"\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.Start();
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Logger(line);
            }
            try
            {
                string StaticDestFileName = ConfigurationSettings.AppSettings["ScheduleDestFileName"].ToString().Trim();
                // File.Delete(StaticDestFileName);
                File.Copy(ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim() + ConfigurationSettings.AppSettings["OutPutFileName"].ToString().Trim() + "_" + DateTimeStr + ".mp4", StaticDestFileName, true);
                Logger("COPY FINAL:" + StaticDestFileName);

            }
            catch (Exception Ex)
            {
                Logger(Ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;


            try
            {

                button1.ForeColor = Color.White;
                button1.Text = "Started";
                button1.BackColor = Color.Red;
                richTextBox1.Text = "";


                string[] FilesList = Directory.GetFiles(ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim());
                foreach (string item in FilesList)
                {
                    try
                    {
                        if (File.GetLastAccessTime(item) < DateTime.Now.AddHours(-48))
                        {
                            File.Delete(item);
                            Logger(item + " Deleted");
                        }
                    }
                    catch (Exception Exp)
                    {
                        Logger("Error Delete" + Exp.Message);
                    }
                }

                List<Cities> Cts = new List<Cities>();
                Cts.Add(new Cities { Name = "ABUJA", Code = "G2352778" });
                Cts.Add(new Cities { Name = "Baghdad", Code = "G98182" });
                Cts.Add(new Cities { Name = "beijing", Code = "G1816670" });
                Cts.Add(new Cities { Name = "Beirut", Code = "G276781" });
                Cts.Add(new Cities { Name = "Berlin", Code = "G2950159" });
                Cts.Add(new Cities { Name = "Brasilia", Code = "G3469058" });
                Cts.Add(new Cities { Name = "BUENOS AIRES", Code = "G3435910" });
                Cts.Add(new Cities { Name = "Cairo", Code = "G360630" });
                Cts.Add(new Cities { Name = "Caracas", Code = "G3646738" });
                Cts.Add(new Cities { Name = "Casablanca", Code = "G2553604" });
                Cts.Add(new Cities { Name = "Damascus", Code = "G170654" });
                Cts.Add(new Cities { Name = "Havana", Code = "G3553478" });
                Cts.Add(new Cities { Name = "Jerusalem (AL QUDS)", Code = "G7870654" });
                Cts.Add(new Cities { Name = "Karachi", Code = "G1174872" });
                Cts.Add(new Cities { Name = "Kuwait City", Code = "G285787" });
                Cts.Add(new Cities { Name = "Lisbon", Code = "G2267057" });
                Cts.Add(new Cities { Name = "London", Code = "G2643743" });
                Cts.Add(new Cities { Name = "Madrid", Code = "G3117735" });
                Cts.Add(new Cities { Name = "Mashhad", Code = "G124665" });
                Cts.Add(new Cities { Name = "Mecca", Code = "G104515" });
                Cts.Add(new Cities { Name = "Melbourne", Code = "G4075766" });
                Cts.Add(new Cities { Name = "Mexico City", Code = "G3530597" });
                Cts.Add(new Cities { Name = "Moscow", Code = "G524901" });
                Cts.Add(new Cities { Name = "Muscat", Code = "G287286" });
                Cts.Add(new Cities { Name = "New Delhi", Code = "G1261481" });
                Cts.Add(new Cities { Name = "Paris", Code = "G2988507" });
                Cts.Add(new Cities { Name = "Pretoria", Code = "G964137" });
                Cts.Add(new Cities { Name = "Sana'a", Code = "G7789599" });
                Cts.Add(new Cities { Name = "Tehran", Code = "G112931" });
                Cts.Add(new Cities { Name = "Tokyo", Code = "G1850147" });
                Cts.Add(new Cities { Name = "Tunis", Code = "G2464470" });
                Cts.Add(new Cities { Name = "Washington", Code = "G4140963" });

                List<Cities> CtsFinal = new List<Cities>();
                foreach (var item in Cts)
                {
                    CtsFinal.Add(getData(item));
                }

                Logger("Start Job");


                //Collect Weather Data
                //CitiesTableAdapter Ta = new CitiesTableAdapter();
                //MyDB.CitiesDataTable Dt = Ta.SelectAllCities();

                StringBuilder Data = new StringBuilder();
                Data.AppendLine("FirstDay = \"" + DateTime.Now.AddDays(1).DayOfWeek.ToString() + "\"");
                Data.AppendLine("SecondDay = \"" + DateTime.Now.AddDays(2).DayOfWeek.ToString() + "\"");
                //Generate XML file:
                for (int i = 0; i < CtsFinal.Count; i++)
                {
                    Logger(CtsFinal[i].Name + " XML");
                    //Weather Format: ["City","1st Day Min","1st Day Max","2nd Day Min","2nd Day Max"]	
                    Data.AppendLine("City" + (i + 1).ToString() + "=[ \"" + CtsFinal[i].Name.ToString().Trim().Replace("\n", "") + "\",\"" + CtsFinal[i].Min.ToString() + "\",\"" + CtsFinal[i].Max.ToString() + "\",\"" + CtsFinal[i].Min2.ToString() + "\",\"" + CtsFinal[i].Max2 + "\"]");

                    //Copy Status Video:
                    string MainStatusDir = ConfigurationSettings.AppSettings["StatusSource"].ToString().Trim();
                    string MainStatusDirDest = ConfigurationSettings.AppSettings["StatusDest"].ToString().Trim();


                    if (!Directory.Exists(MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\"))
                        Directory.CreateDirectory(MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\");

                    if (CtsFinal[i].State.ToLower().Contains("thunderstorm"))
                        File.Copy(MainStatusDir + "Lightening\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("drizzle"))
                        File.Copy(MainStatusDir + "Rain\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("rain"))
                        File.Copy(MainStatusDir + "Rain\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("snow"))
                        File.Copy(MainStatusDir + "Snow\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("atmosphere"))
                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("clouds"))
                        File.Copy(MainStatusDir + "Cloudy\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else
                    if (CtsFinal[i].State.ToLower().Contains("clear"))
                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);
                    else

                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day1\\WP.mov", true);


                    if (!Directory.Exists(MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\"))
                        Directory.CreateDirectory(MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\");

                    if (CtsFinal[i].State2.ToLower().Contains("thunderstorm"))
                        File.Copy(MainStatusDir + "Lightening\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                    if (CtsFinal[i].State2.ToLower().Contains("drizzle"))
                        File.Copy(MainStatusDir + "Rain\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                    if (CtsFinal[i].State2.ToLower().Contains("rain"))
                        File.Copy(MainStatusDir + "Rain\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                   if (CtsFinal[i].State2.ToLower().Contains("snow"))
                        File.Copy(MainStatusDir + "Snow\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                   if (CtsFinal[i].State2.ToLower().Contains("atmosphere"))
                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                   if (CtsFinal[i].State2.ToLower().Contains("clouds"))
                        File.Copy(MainStatusDir + "Cloudy\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else
                   if (CtsFinal[i].State2.ToLower().Contains("clear"))
                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);
                    else

                        File.Copy(MainStatusDir + "Sunny\\WP.mov", MainStatusDirDest + "City (" + (i + 1).ToString() + ")" + "\\Day2\\WP.mov", true);

                }
                //Save Xml File:
                try
                {
                    StreamWriter s = new StreamWriter(ConfigurationSettings.AppSettings["DataPath"].ToString().Trim());
                    s.Write(Data);
                    s.Dispose();
                }
                catch (Exception Exp)
                {
                    Logger("Save XML" + Exp.Message);
                }

                //Render Video:
                render();
                //Convert();
                button1.ForeColor = Color.White;
                button1.Text = "START";
                button1.BackColor = Color.Navy;
            }
            catch (Exception exp)
            {

                Logger(exp.Message);
            }

            timer1.Enabled = true;
        }
        protected void render()
        {
            string DateTimeStr = string.Format("{0:0000}", DateTime.Now.Year) + "-" + string.Format("{0:00}", DateTime.Now.Month) + "-" + string.Format("{0:00}", DateTime.Now.Day) + "_" + string.Format("{0:00}", DateTime.Now.Hour) + "-" + string.Format("{0:00}", DateTime.Now.Minute) + "-" + string.Format("{0:00}", DateTime.Now.Second);

            DirectoryInfo Dir = new DirectoryInfo(ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim());

            if (!Dir.Exists)
            {
                Dir.Create();
            }
            Process proc = new Process();
            proc.StartInfo.FileName = "\"" + ConfigurationSettings.AppSettings["AeRenderPath"].ToString().Trim() + "\"";
            proc.StartInfo.Arguments = " -project " + "\"" + ConfigurationSettings.AppSettings["AeProjectFile"].ToString().Trim() + "\"" + "   -comp   \"" + ConfigurationSettings.AppSettings["Composition"].ToString().Trim() + "\" -output " + "\"" + ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim() + ConfigurationSettings.AppSettings["OutPutFileName"].ToString().Trim() + "_" + DateTimeStr + ".mp4" + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            if (!proc.Start())
            {
                return;
            }

            proc.PriorityClass = ProcessPriorityClass.Normal;
            StreamReader reader = proc.StandardOutput;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Logger(line);
            }
            proc.Close();

           
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            timer1_Tick(null, null);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }
    }
}
