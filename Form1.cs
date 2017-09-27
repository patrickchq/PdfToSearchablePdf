using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using asprise_ocr_api;
using Tesseract;

namespace PdftoSearchablePdf
{
    public partial class Form1 : Form
    {
        public bool stopButtonClicked = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (!stopButtonClicked)
            {
                string root = @"C:\Documents\RPA\AbbyFinderServer";
                string Processing_root = root + @"\" + "Processing";
                string Processed_root = root + @"\" + "Processed";
                //after this step all the json file should be in processing folder
                checkNewConfigFile(root);
                //get all config files in processing folder
                List<string> configfilelist = GetTobeProcessedConfigFiles(Processing_root);
                foreach (string configfile in configfilelist) {
                    string[] folders = checkFolders(configfile);
                    ChangetoSearchablePdf(folders);
                }
                movetoProcessed(Processing_root, Processed_root);
            }

        }
        //get all config file in processing folder
        private List<string> GetTobeProcessedConfigFiles(string rootfolder) {
            List<string> tobeprocessedstring = new List<string>();
            tobeprocessedstring = System.IO.Directory.GetFiles(rootfolder, "*.json").ToList<string>();
            return tobeprocessedstring;
        }

        private void ChangetoSearchablePdf2(string[] folders) {
            string incomefolder = folders[0];
            string outcomefolder = folders[1];
            TesseractEngine eng = new TesseractEngine("@C:/Documents/123.pdf","eng");
            




        }
        private void ChangetoSearchablePdf(string[] folders) {
            string incomefolder = folders[0];
            string outcomefolder = folders[1];
            AspriseOCR.SetUp();
            AspriseOCR ocr = new AspriseOCR();
            ocr.StartEngine("eng", AspriseOCR.SPEED_FASTEST);
            string[] files = System.IO.Directory.GetFiles(incomefolder, "*.pdf").Select(Path.GetFileName).ToArray();
            foreach (string tmpfile in files) {
                try { 
                string fullinputfilename = incomefolder + @"/" + tmpfile;
                string fulloutputfilename = outcomefolder + @"/" + tmpfile;
                ocr.Recognize(fullinputfilename, -1, -1, -1, -1, -1,
                AspriseOCR.RECOGNIZE_TYPE_ALL, AspriseOCR.OUTPUT_FORMAT_PDF,
                AspriseOCR.PROP_PDF_OUTPUT_FILE,fulloutputfilename,
                AspriseOCR.PROP_PDF_OUTPUT_TEXT_VISIBLE, false);
                Console.WriteLine("File " + fulloutputfilename + "has done transformation yeah" );
                File.Delete(fullinputfilename);
                } catch (Exception ex) {
                    Console.WriteLine(ex + "3333");
                }
                }
            ocr.StopEngine();
        }

        //read the one single json file and output its income and output folder
        private string[] checkFolders(string jasonfile) {
            configfile config = new configfile();
            string[] result = new string[2];
            using (StreamReader r = new StreamReader(jasonfile))
            {
                string json = r.ReadToEnd();
                List<configfile> configlist = JsonConvert.DeserializeObject<List<configfile>>(json);
                Console.WriteLine(configlist);
                //MessageBox.Show(configlist);
                foreach (configfile fff in configlist) {
                    //Console.WriteLine(fff.incomingfolder.GetType());
                    result[0] = fff.incomingfolder;
                    result[1] = fff.outputfolder;
                }
            }
            return result;
        }

        //check for new configuration file and move to processing folder
        private void checkNewConfigFile(string root)
        {
            root = @"C:\Documents\RPA\AbbyFinderServer";
            string[] files = System.IO.Directory.GetFiles(root, "*.json").Select(Path.GetFileName).ToArray();
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Move(root + @"\" + file, root + @"\Processing\" + file);
                }
                catch (Exception ex)
                {
                 //   File.Delete(root + @"\Processing\" + file);
                   // System.IO.File.Move(root + @"\" + file, root + @"\Processing\" + file);
                    Console.WriteLine(ex + "11111");
                }
            }
        }
        //move the processed configuration file into the processed folder
        private void movetoProcessed(string Processing_root, string Processed_root)
        {
            //string Prcessing_root = root + "Processing";
            string[] files = System.IO.Directory.GetFiles(Processing_root, "*.json").Select(Path.GetFileName).ToArray();
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Move(Processing_root + @"\" + file, Processed_root + @"\"+ file);
                }
                catch (Exception ex)
                {
                    //overwrite the original file if existing 
                    File.Delete(Processed_root + @"\" + file);
                    System.IO.File.Move(Processing_root + @"\" + file, Processed_root + @"\" + file);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopButtonClicked = true;
            Console.WriteLine("Button is clicked");
        }
    }
}
