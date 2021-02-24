using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace MeteoStation
{
    public partial class Form1 : Form
    {

            String DefaultCOM = "COM2";
            DataTable dt = new DataTable();
            bool bData_received = true;
            byte[] BufferS;
            List<Trame> LTrame = new List<Trame>();
            List<byte> BufferF = new List<byte>();
            int count;
            int cptRead = 0;
            Timer timer = new Timer { Interval = 200, Enabled = false };


        public Form1()
            {
                InitializeComponent();
                createGrid();
                initialSerialPort();

            }

            private void createGrid()
            {
                datagridMeteo.DataSource = dt;
                ///////////////////////////////
                ///CREATE GRID
                DataColumn dc = new DataColumn("Début", typeof(String));
                dt.Columns.Add(dc);
                dc = new DataColumn("ID", typeof(int));
                dt.Columns.Add(dc);
                dc = new DataColumn("Nbre Octet", typeof(int));
                dt.Columns.Add(dc);
                dc = new DataColumn("Type", typeof(int));
                dt.Columns.Add(dc);
                dc = new DataColumn("SumOctet", typeof(String));
                dt.Columns.Add(dc);
                dc = new DataColumn("CheckSum", typeof(int));
                dt.Columns.Add(dc);
                dc = new DataColumn("Fin", typeof(int));
                dt.Columns.Add(dc);
                


            }

            private void initialSerialPort()
            {
                Serial.PortName = DefaultCOM;
                Serial.ReceivedBytesThreshold = 1;
                Serial.Handshake = Handshake.None;
                Serial.DtrEnable = false;
                Serial.RtsEnable = false;
                // Instatiate this class
                Console.WriteLine("Incoming Data :");
                // Begin communications
                Serial.Open();
                Serial.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);
            }

            private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                if (bData_received == true)
                {
                    count = Serial.BytesToRead;
                    BufferS = new byte[count];
                    Serial.Read(BufferS, 0, count);
                    bData_received = false;
                    cptRead++;
                    AddtoBufferF();
            }
              
            //SysoToTab();
            
                //AddToList();
                //MessageBox.Show("Result tab: "+ Serial.Read(BufferS,0,count));
                //MessageBox.Show("Result Status: "+Serial.IsOpen+" Taille :"+ Serial.BytesToRead);
                // Show all the incoming data in the port's buffer
                //Console.WriteLine("Caractère reçu");
                //Console.WriteLine(Serial.ReadExisting());
            }

        private void SysoToTab()
        {
            for (uint index = 0; index < BufferS.Length; index++)
            {
                Console.WriteLine("Index :" + index + " | Value :" + BufferS[index]);
            }
        }
        private void AddtoBufferF()
        {
            //////////////////////////////////
            ////Insertion to list
            for (int index = 0; index < BufferS.Length; index++)
            {
                BufferF.Add(BufferS[index]);
            }
            AddtoLTrame();
        }
        private void AddtoLTrame()
        {
            String fintrame = "";
            bool VerifTrame = false;
            bool traitementTrame = false;
            for (int index = 0; index < BufferF.Count; index++)
            {
                if (BufferF[index] == 85 && VerifTrame == false)
                {
                    String debuttrame = "";
                    
                    VerifTrame = true;
                    debuttrame = BufferF[index].ToString() + BufferF[index + 1].ToString() + BufferF[index + 2].ToString();
                    Console.WriteLine("Debut de trame");
                    /////////////////////////////////////
                    ////Position sur id de la trame
                    if (debuttrame == "8517085")
                    {
                        ////////////////////////////////////
                        //Verif que c'est bien le debut d'une trame
                        traitementTrame = true;

                    }
                    else
                    {
                        VerifTrame = false;
                    }
                }
                if (traitementTrame == true)
                {
                    /////////////////////////////////
                    ///Creation d'une trame d'information
                    Trame newtrame = new Trame();
                    String newdata = "";
                    ////////////////////////////////////////
                    ///Commencement de la recuperation des informations de la trame
                    /////id position
                    int memoryposition = index + 3;
                    newtrame.id = BufferF[memoryposition];
                    ////////////////
                    ///Nombre de data 
                    newtrame.cptOctet = BufferF[memoryposition + 1];
                    ////////////////
                    ///Type
                    newtrame.Type = BufferF[memoryposition + 2];
                    //////////////////
                    ///Data
                    for (int cpt = 0; cpt <= newtrame.cptOctet; cpt++)
                    {
                        newdata += BufferS[memoryposition + 3 + cpt].ToString();

                    }

                    newtrame.data = newdata;
                    ///////////////
                    ///Checksumm
                    newtrame.checksum = BufferS[memoryposition + 3+ newtrame.cptOctet ];
                    //////////////////////////////////////////
                    ///Fin de trame 
                    index = memoryposition + 3 + newtrame.cptOctet;
                    fintrame = BufferF[index+1].ToString() + BufferF[index + 2].ToString() + BufferF[index + 3].ToString();
                    //////////////////////////////////////////
                    ///reset
                   
                    if (fintrame == "17085170")
                    {
                        Console.WriteLine("New trame id:" + newtrame.id + "| Nbre :" + newtrame.cptOctet + "| Type :" + newtrame.cptOctet + "| " + "| Data :" + newtrame.data + "| " + "| Checksum :" + newtrame.checksum);
                        LTrame.Add(newtrame);
                        Console.WriteLine("taille de la liste:" + LTrame.Count);
                        index = index + 3;
                        VerifTrame = false;
                        traitementTrame = false;

                    }
                    else
                    {
                        Console.WriteLine("probleme" + fintrame);
                    }

                }
            }
            MessageBox.Show("Fin de la lecture du jet d'information n°" + cptRead);
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            foreach(Trame trame in LTrame)
            {
                DataRow dr = new DataRow();
                datagridMeteo.Rows.Add(dr);
            }
        }
    }
}
