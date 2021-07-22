using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace SeraManagementv2
{
    public partial class Form1 : Form
    {
        asyclient client;

        private const int PORT = 7777;

        public Form1()  //Programın başlangıç ayaralrı
        {
            InitializeComponent();
            client = new asyclient(this);
            CheckForIllegalCrossThreadCalls = false;

            groupBox1.BackColor = Color.FromArgb(120, Color.Gray);
            groupBox2.BackColor = Color.FromArgb(120, Color.Gray);
            groupBox3.BackColor = Color.FromArgb(120, Color.Gray);
            groupBox4.BackColor = Color.FromArgb(0, Color.Gray);
            groupBox5.BackColor = Color.FromArgb(0, Color.Gray);
            groupBox6.BackColor = Color.FromArgb(0, Color.Gray);
            label5.BackColor = Color.FromArgb(0, Color.Black);
            label6.BackColor = Color.FromArgb(0, Color.Black);
            label7.BackColor = Color.FromArgb(0, Color.Black);
            label2.BackColor = Color.FromArgb(0, Color.Black);
            label3.BackColor = Color.FromArgb(0, Color.Black);
            label4.BackColor = Color.FromArgb(0, Color.Black);
            label8.BackColor = Color.FromArgb(0, Color.Black);
            label11.BackColor = Color.FromArgb(0, Color.Black);
            label12.BackColor = Color.FromArgb(0, Color.Black);
            label9.BackColor = Color.FromArgb(0, Color.Black);
            label10.BackColor = Color.FromArgb(0, Color.Black);
            label13.BackColor = Color.FromArgb(0, Color.Black);
            label1.BackColor = Color.FromArgb(0, Color.Black);
            label14.BackColor = Color.FromArgb(0, Color.Black);
            label15.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox1.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox2.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox3.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox4.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox5.BackColor = Color.FromArgb(0, Color.Black);
            pictureBox6.BackColor = Color.FromArgb(0, Color.Black);


        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //Program kapanırken sunucuyu kapatan fonksiyon
        {
         
         client.server.Close();

        }




        private void label7_Click(object sender, EventArgs e)
        {

        }

        public static void close(Form1 form)  //Eğer bağlanmaya çalıştığımız sunucu kapalıysa programı sonlandıran fonksiyon
        {
            MessageBox.Show("Server Kapalı! Uygulama Kapanıyor...");
            Thread.Sleep(3000);
            form.Close();
        }


        public class asyclient //programla sunucu arasındaki veri alışverişi için oluşturulan sınıf
        {
            Form1 form;
            public IPAddress serverIP;
            public int serverPort;
            public Socket server;
            IPEndPoint ip;

            public string readtext;

            public asyclient(Form1 form)
            {
                
                this.form = form;
                serverIP = IPAddress.Parse("25.51.82.88"); //sunucunun ip adresi
                serverPort = 7777;

                try
                {
                    ip = new IPEndPoint(serverIP, serverPort);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
                    server.Connect(ip);  //bağlantının kurulması
                    SendData("yonetim", server); //Sunucuya yönetim programının bağlandığını gösteren mesajın gönderilmesi
                }
                catch(Exception e)
                {                  
                    Form1.close(form);                 
                }

                Thread th = new Thread(() => ReadData(server));//Sunucudan gelen verilerin okunup işlenmesi için iş parçacığı oluşturulması
                th.Start();

            }

            public void ReadData(Socket server) //Sunucudan gelen verileri okuyup gelen veriyi işleyen fonksiyon
            {
                try
                {

                    NetworkStream ns = new NetworkStream(server);
                    StreamReader reader = new StreamReader(ns);
                    

                    char[] buff = new char[20];
                    int readbytecount = 0;
                    string data;

                    while (true)
                    {
                        readbytecount =  reader.Read(buff, 0, buff.Length);//Mesajın okunması

                        if (readbytecount <= 0)
                        {
                            Console.WriteLine("Disconnected from server!");
                    
                            break;
                        }
                        readtext = new string(buff);
                        Console.WriteLine("Received bytes: " + readbytecount + " Message: " + readtext);
                        Array.Clear(buff, 0, buff.Length);


                        if (readtext.Substring(0, 2) == "s1")//Eğer gelen mesajın ilk 2 karakteri "s1" ise sera 1 için gelmiş demektir.
                        {
                            data = readtext.Substring(2, readtext.Length - 2);
                            if (data.Contains("99")) //Eğer sera kapalıysa 99 değeri geliyor demektir.Sera durumu programda gösterilir.
                            {
                                form.label9.Text = "";
                                form.label5.Text = "Sera Durumu : Kapalı";
                                form.label1.Text = "";


                            }
                            else //Sera kapalı değilse çalışma sıcaklığı ve sera durumu programda gösterilir
                            {
                            form.label9.Text = data.Substring(0,5);
                            form.label5.Text = "Sera Durumu : Çalışıyor";
                            form.label1.Text = "°C";


                            }




                        }
                        else if (readtext.Substring(0, 2) == "s2")  //Eğer gelen mesajın ilk 2 karakteri "s2" ise sera 2 için gelmiş demektir.
                        {
                            data = readtext.Substring(2, readtext.Length - 2);
                            if (data.Contains("99")) //Eğer sera kapalıysa 99 değeri geliyor demektir.Sera durumu programda gösterilir.
                            {
                                form.label10.Text = "";
                                form.label6.Text = "Sera Durumu : Kapalı";
                                form.label14.Text = "";

                            }
                            else  //Sera kapalı değilse çalışma sıcaklığı ve sera durumu programda gösterilir
                            {
                            form.label10.Text = data.Substring(0, 5);
                                form.label6.Text = "Sera Durumu : Çalışıyor";
                            form.label14.Text = "°C";

                            }

                        }
                        else if (readtext.Substring(0, 2) == "s3")//Eğer gelen mesajın ilk 2 karakteri "s3" ise sera 2 için gelmiş demektir.
                        {
                            data = readtext.Substring(2, readtext.Length - 2);
                            if (data.Contains("99")) //Eğer sera kapalıysa 99 değeri geliyor demektir.Sera durumu programda gösterilir.
                            {
                                form.label13.Text = "";
                                form.label7.Text = "Sera Durumu : Kapalı";
                                form.label15.Text = "";

                            }

                            else//Sera kapalı değilse çalışma sıcaklığı ve sera durumu programda gösterilir
                            {
                            form.label13.Text = data.Substring(0, 5);
                                form.label7.Text = "Sera Durumu : Çalışıyor";
                            form.label15.Text = "°C";

                            }

                        }        

                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            public void SendData(string input,Socket server) //Giriş olarak verilen stringin sunucuya gönderilmesini sağlayan yardımcı fonksiyon
            {
                byte[] msg = Encoding.UTF8.GetBytes(input);
                byte[] bytes = new byte[256];
                try
                {
                   
                    int i = server.Send(msg);
                    Console.WriteLine("Sent {0} bytes.", i);
                }

                catch (SocketException e)
                {
                    Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);

                }

            }

        }
        private void button4_Click(object sender, EventArgs e)//Sera1 'in kapatma butonuna tıklandığında sunucuya ilgili mesajı("cut1") gönderen fonksiyon
        {
            string send = "cut";
            byte[] buffer = new byte[512];
            send = String.Concat(send, "1");
            client.SendData(send,client.server);
            Array.Clear(buffer, 0, buffer.Length);          

        }

        private void button5_Click(object sender, EventArgs e)//Sera2 'nin kapatma butonuna tıklandığında sunucuya ilgili mesajı("cut2") gönderen fonksiyon
        {

            string send = "cut";
            byte[] buffer = new byte[512];
            send = String.Concat(send, "2");
            client.SendData(send,client.server);
            Array.Clear(buffer, 0, buffer.Length);
            

        }

        private void button6_Click(object sender, EventArgs e)//Sera3 'ün kapatma butonuna tıklandığında sunucuya ilgili mesajı("cut3") gönderen fonksiyon
        {

            string send = "cut";
            byte[] buffer = new byte[512];
            send = String.Concat(send, "3");
            client.SendData(send,client.server);
            Array.Clear(buffer, 0, buffer.Length);        
        }

        private void button1_Click(object sender, EventArgs e) //Sera1'in sıcaklık değişimi butonununa tıklandığında sunucuya ilgili mesajı(örnek: "degree1-45") gönderen fonksiyon
        {
            int value;
            string id = "degree1";
            string send;
            string masktext = textBox4.Text;
            if (Int32.TryParse(masktext,out value))
            {
                if(value>=51 && value < 0)
                {
                    MessageBox.Show("Hatalı sayı girişi!");

                }
                else
                {
                
                send=String.Concat(id, masktext);
                client.SendData(send, client.server);
                
                }


            }
            else
            {
                MessageBox.Show("Hatalı sayı girişi!");

            }
        }

        private void button2_Click(object sender, EventArgs e)//Sera2'nin sıcaklık değişimi butonununa tıklandığında sunucuya ilgili mesajı(örnek: "degree2-45") gönderen fonksiyon
        {
            int value;
            string id = "degree2";
            string send;
            string masktext = textBox5.Text;
            if (Int32.TryParse(masktext, out value))
            {
                if (value >= 51 && value < 0)
                {
                    MessageBox.Show("Hatalı sayı girişi!");

                }
                else
                {
                    send = String.Concat(id, masktext);
                    client.SendData(send, client.server);             
                }

            }
            else
            {
                MessageBox.Show("Hatalı sayı girişi!");

            }

        }

        private void button3_Click(object sender, EventArgs e)//Sera3'ün sıcaklık değişimi butonununa tıklandığında sunucuya ilgili mesajı(örnek: "degree3-45") gönderen fonksiyon
        {
            int value;
            string id = "degree3";
            string send;
            string masktext = textBox6.Text;
            if (Int32.TryParse(masktext, out value))
            {
                if (value >= 51 && value < 0)
                {
                    MessageBox.Show("Hatalı sayı girişi!");

                }
                else
                {
                    send = String.Concat(id, masktext);
                    client.SendData(send, client.server);
                    
                }

            }
            else
            {
                MessageBox.Show("Hatalı sayı girişi!");

            }
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }




}
