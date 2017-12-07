using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace RFID_Control_Acceso
{
    public partial class RFID : Form
    {   //variable para abrir el puerto serial
        private SerialPort serialPort1;

        //inicializamos el puerto serial
        public RFID()
        {
            InitializeComponent();
            serialPort1 = new SerialPort();
            //utilizamos el mismo puerto que con el arduino
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 250000;
            serialPort1.DtrEnable = true;
            serialPort1.Open();
            
        }
        //variable para conectar con base de datos
        MySqlConnection conectar;

        //metodo que realiza la conexión a Mysql
        public void Mysqlconect()
        {
            try
            {   //cadena de conexion
                string cadenaConexion = @"Server=127.0.0.1; Database=rfidbd; Uid=root; Password=;";
                conectar = new MySqlConnection(cadenaConexion);
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {   //iniciamos el timer
            timer1.Start();
            //conectamos a la bd
            Mysqlconect();
            //abrimos la conexion a la bd
            conectar.Open();
            
        }
        //variable para leer los datos
        MySqlDataReader reader;
        //variable para ejecutar query  
        MySqlCommand mycmd;
       //variables temporales para cada columna de la bd
        string a1, a2, a3;
        private void timer1_Tick(object sender, EventArgs e)
        {   //tomamos la fecha del sistema
            string fecha = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"); 
                      
                    //cadena de consulta a la BD de la tabla de usuarios, comparando el numero RFID que mando el arduino con el que esta cargado en la tabla
                    string query = "SELECT N_Control, Nombre, T_Usuario From usuarios WHERE RFID = " + serialPort1.ReadLine();
                    //ejecutamos la consulta
                    using ( mycmd = new MySqlCommand(query, conectar));
                    //leemos los datos
                    reader = mycmd.ExecuteReader();
                    //si encontro el registro en la tabla de usuarios
                    if (reader.Read())
                    {
                        //imprime los datos en el Form
                        label1.Text = "Num. Control= " + Convert.ToString(reader["N_Control"]) + " | Nombre= " + Convert.ToString(reader["Nombre"]) + " | Tipo Usuario= " + Convert.ToString(reader["T_Usuario"]);
                            
                        //guardamos los datos en variables
                            a1 = Convert.ToString(reader["N_Control"]);
                            a2 = Convert.ToString(reader["Nombre"]);
                            a3 = Convert.ToString(reader["T_Usuario"]);
                        //mandamos valor de "1" 
                        serialPort1.Write("1");
                        //finaliza la conexión
                        conectar.Close();
                        //iniciamos otra conexión para ingresar datos guardados en las variables
                        conectar.Open();
                        //cadena para insertar los datos en la tabla de acceso, contiene Nombre, Puesto y Fecha
                        string query2 = "INSERT INTO accesos (N_Control, Nombre, Puesto, Fecha) VALUES ('" + a1 + "', '" + a2 + "', '" + a3 + "', '" + fecha + "')";
                        //ejecutamos el query
                        using (MySqlCommand cmd = new MySqlCommand(query2, conectar))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        //finaliza la conexión
                        conectar.Close();
                    }
                    else { //si no se encontro el numero RFID en la tabla, limpiar el label1
                        label1.Text = ""; 
                    }
                    //si el label1 quedo vacio, imprimir "Acceso Denegado" en el label2
                    if (label1.Text == "")
                    {
                        label2.Text = "Acceso Denegado ";
                        //se manda "0" por el puerto serial
                        serialPort1.Write("0");
                        //finaliza la conexión
                        conectar.Close();
                    }
                    else //si el label1 tiene informacion, limpiar el label2 
                        label2.Text = "";

                    //abrir la conexión
                conectar.Open();
        
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

       

       
    }
}
