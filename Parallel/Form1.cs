using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Parallel
{
    public partial class Form1 : Form
    {
        string fileName;
        string copyfileName;
        string fileNamecrypt;
        Thread myThread;
        public Form1()
        {
            InitializeComponent();
            button3.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            progressBar1.Visible = false;

            radioButton1.Visible = false;
            radioButton2.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            progressBar2.Visible = false;
            label3.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;


            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            progressBar1.Visible = true;
            this.Width = 348;
            this.Height = 193;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                textBox1.Text = fileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                copyfileName = openFileDialog1.FileName;
                textBox2.Text = copyfileName;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Threading.Tasks.Parallel.Invoke(Copy);
        }

        private void Copy()
        {
            try
            {
                using (FileStream stream = File.Open(fileName, FileMode.Open))
                {
                    progressBar1.Maximum = (int)stream.Length;
                    byte[] array = new byte[0];
                    if (stream.Length > 4096)
                        array = new byte[4096];
                    else
                        array = new byte[stream.Length];

                    using (FileStream write = File.Open(copyfileName, FileMode.Create))
                    {
                        while (stream.Position < stream.Length)
                        {
                            progressBar1.Value = (int)stream.Position;
                            stream.Read(array, 0, array.Length);
                            write.Write(array, 0, array.Length);
                        }
                        progressBar1.Value = progressBar1.Maximum;
                        MessageBox.Show("Виконано успішно");
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox temp = (TextBox)sender;
            if(temp.Name == textBox1.Name)
            {
                fileName = temp.Text;
            }
            else if (temp.Name == textBox2.Name)
            {
                copyfileName = temp.Text;
            }
            else if (temp.Name == textBox4.Name)
            {
                fileNamecrypt = temp.Text;
            }
        }
        //генератор повторюваної послідовності
        private string GetRepeatKey(string s, int n)
        {
            var r = s;
            while (r.Length < n)
            {
                r += r;
            }

            return r.Substring(0, n);
        }

        //метод шифрування/дешифрування
        private string Cipher(string text, string secretKey)
        {
            var currentKey = GetRepeatKey(secretKey, text.Length);
            var res = string.Empty;
            for (var i = 0; i < text.Length; i++)
            {
                res += ((char)(text[i] ^ currentKey[i])).ToString();
            }

            return res;
        }

        //шифрування тексту
        public string Encrypt(string plainText, string password)
            => Cipher(plainText, password);

        //розшифрування тексту
        public string Decrypt(string encryptedText, string password)
            => Cipher(encryptedText, password);

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;


            radioButton1.Visible = true;
            radioButton2.Visible = true;
            button6.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            progressBar2.Visible = true;
            label3.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            this.Width = 429;
            this.Height = 190;  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileNamecrypt = openFileDialog1.FileName;
                textBox4.Text = fileNamecrypt;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            myThread = new Thread(new ThreadStart(Second));
            myThread.Start();
        }

        private void Second()
        {
            try
            {
                StreamReader stream_t = new StreamReader(fileNamecrypt);
                progressBar2.Invoke((Action)delegate () { progressBar2.Maximum = stream_t.ReadToEnd().Length; });
                stream_t.Dispose();
                List<string> list = new List<string>();
                using (StreamReader stream = new StreamReader(fileNamecrypt))
                {
                    string tepm;
                    while (!stream.EndOfStream)
                    {
                        tepm = stream.ReadLine();
                        progressBar2.Invoke((Action)delegate () { progressBar2.Value += tepm.Length; });
                        if (radioButton1.Checked)
                            list.Add(Encrypt(tepm, textBox3.Text));
                        else if (radioButton2.Checked)
                            list.Add(Decrypt(tepm, textBox3.Text));
                        Thread.Sleep(1000);
                    }
                    progressBar2.Invoke((Action)delegate () { progressBar2.Value = (int)stream.BaseStream.Length; });
                    MessageBox.Show("Виконано успішно");
                }
                using (StreamWriter write = new StreamWriter(fileNamecrypt, false, System.Text.Encoding.Default))
                {
                    foreach (string tem in list)
                    {
                        write.Write(tem);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            myThread.Interrupt();
            myThread.Join();
            myThread = null;
            progressBar2.Value = 0;
        }
    }
}
