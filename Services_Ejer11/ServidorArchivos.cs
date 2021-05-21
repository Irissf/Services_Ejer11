﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services_Ejer11
{
    class ServidorArchivos
    {

        //Vamos a necesitar un lock para gestionar lo de desconectar el servido ............. FUCK
        //Solo para gestionar la booleana del servidor, la del cliente es única de cada cliente
        //ESTRUCTURA WHILE, LOCK, IF

        private bool acabar = false;
        private bool clienteAcabar = false;
        private Socket socketServidor;
        private object llave = new object();


        /// <summary>
        /// leeArchivo(nombreArchivo, nLineas): Función que lee nLineas del archivo de texto indicado en el 
        /// parámetro nombreArchivo y las devuelve como string. Si el archivo es más corto que las líneas 
        /// indicadas simplemente lo devuelve entero. El parámetro nombreArchivo simplemente es el nombre
        /// y la extensión, el directorio donde lo lee es el apuntado por la variable de entorno EXAMEN. Para 
        /// trabajar con la variable de entorno usa la función Environment.GetEnvironmentVariable. Si hay 
        /// algún problema de apertura devuelve el string “<ERROR_IO>”.
        /// </summary>
        /// <param name="nombreArchivo"></param>
        /// <param name="nLineas"></param>
        /// <returns></returns>
        public string LeeArchivo(string nombreArchivo, int nLineas)
        {
            string texto = "";
            string linea = "";
            int lineasLeidas = 0;
            bool salir = false;
            try
            {
                using (StreamReader sr = new StreamReader(Environment.GetEnvironmentVariable("EXAMEN")+"/"+nombreArchivo))
                {
                    while (lineasLeidas < nLineas && !salir) //o que llegue al final del archivo devuelve null o -1, comprobarlo
                    {
                        linea = sr.ReadLine();
                        if (linea != null)
                        {
                            texto += linea;
                            texto += "\n";
                            lineasLeidas++;
                        }
                        else
                        {
                            salir = true;
                        }
                    }
                }

            }
            catch (Exception)
            {
                return("No se encontró el archivo en: " + nombreArchivo);
            }

            return texto;
        }

        /// <summary>
        /// leePuerto(): método que trata de leer el archivo C:\%EXAMEN%\puerto.txt y lee el valor que haya
        /// en su primera línea. Para ello llama a leeArchivo(“puerto.txt”,1). Si es un valor de puerto válido lo
        /// devuelve. Si no lo es o no puede abrir el archivo devuelve 31416.
        /// </summary>
        /// <returns></returns>
        public int LeePuerto()
        {
            int puerto = 31416;
            using (StreamReader sr = new StreamReader(Environment.GetEnvironmentVariable("EXAMEN") + "/puerto.txt"))
            {
                try
                {
                    puerto = Convert.ToInt32(sr.ReadLine());
                    //El puerto minimo es IPEndPoint.MinPort (0) pero yo voy a poner un mínimo de 10000
                    if (puerto < 10000 || puerto > IPEndPoint.MaxPort)
                    {
                        puerto = 31416;
                    }
                }
                catch (Exception)
                {
                }
            }
            return puerto;
        }

        /// <summary>
        ///  guardaPuerto(numero): Guarda el parámetro numero en C:\%EXAMEN%\puerto.txt creándolo si no 
        ///  existe o sobreescribiéndolo si ya existe.
        /// </summary>
        /// <param name="numero"></param>
        public void GuardaPuerto(int numero)
        {
            using (StreamWriter sw = new StreamWriter(Environment.GetEnvironmentVariable("EXAMEN") + "/puerto.txt"))
            {
                sw.WriteLine("" + numero);
            }
        }

        /// <summary>
        /// listaArchivos(): Método que obtiene la lista de archivos de extensión txt del directorio EXAMEN
        /// y la devuelve como una cadena única(los nombres de los archivos separados por retornos de carro. No
        /// se incluye la trayectoria).
        /// </summary>
        public string ListaArchivos()
        {
            String lista = "";

            //Cogemos el directorio
            DirectoryInfo directorio = new DirectoryInfo(Environment.GetEnvironmentVariable("EXAMEN"));
            //Accedemos a los archivos del directorio
            FileInfo[] archivos = directorio.GetFiles();
            for (int i = 0; i < archivos.Length; i++)
            {
                if (archivos[i].Extension == ".txt")
                {
                    lista += archivos[i].Name.Remove(archivos[i].Name.Length - 4) + "\n";
                }
            }
            return lista;
        }

        /// <summary>
        /// iniciaServidorArchivos(): Método principal donde se inicia el servicio. Realiza la programación 
        /// necesaria para el inicio de la comunicación por red.Se obtiene el puerto llamando a leePuerto, si el
        /// puerto está ocupado informa de ese hecho en pantalla y finaliza el programa.Si no debe informar
        /// por pantalla del puerto de conexión y tras esto se queda a la escucha.
        /// Finalmente entra en un bucle en el cual se realiza la conexión con el cliente iniciando un hilo que
        /// ejecuta la función hiloCliente.Del bucle se saldrá cuando reciba el comando correcto de un cliente
        /// como se indica más abajo.
        /// </summary>
        public void IniciaServidorArchivos()
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse("127.0.0.1"), LeePuerto());
            socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //Enlazamos el socket al puerto
                socketServidor.Bind(ie);
                Console.WriteLine("Puerto Conectado al: " + LeePuerto());

                //Esperamos clientes
                socketServidor.Listen(2);

                //Hilo de los clientes

                
                while (!acabar)
                {
                    //mientras no se de la orden de acabar
                    lock (llave)
                    {
                        
                        if (!acabar)
                        {
                            //Aceptamos a un cliente y lo hilamos
                            try
                            {
                                Socket socketCliente = socketServidor.Accept(); //sigue sin funcionar
                                Thread hilo = new Thread(HiloCliente);
                                hilo.Start(socketCliente);
                            }
                            catch (SocketException)
                            {
                                //cuando cerramos el servidor aun sigue quedando en esa linea del Accept, y lanza una excenpción, solo es necesario controlarla
                            }
                            
                        }
                    }
                }

            }
            catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
            {
                Console.WriteLine("Puerto Ocupado");
                socketServidor.Close();
            }
        }

        /// <summary>
        /// hiloCliente(object socket): Función que se ejecuta como hilo según se ha comentado. El parámetro 
        /// es el socket de cliente. Indicará por pantalla la IP y puerto del cliente. Tras conectarse el cliente al
        /// servidor, este último envía el mensaje CONEXION ESTABLECIDA. Luego se queda a la espera de 
        /// alguno de los siguientes comandos (si no se cumple el protocolo de forma precisa no hace nada):
        ///     GET archivo,n: Llama a la función leeArchivo con n líneas y envía al cliente lo que devuelva dicha función.
        ///     PORT numero: guarda numero en el archivo puerto.txt llamando a guardaPuerto. 
        ///     LIST: manda la cadena que devuelve la función listaArchivos
        ///     CLOSE: Desconecta al cliente
        ///     HALT: finaliza el servidor completamente.
        /// </summary>
        /// <param name="socket"></param>
        public void HiloCliente(object socket)
        {

            Console.WriteLine("Entro aqui");
            clienteAcabar = false;
            Socket socketCliente = (Socket)socket;
            //para la info del puerto del cliente
            IPEndPoint infoCLiente = (IPEndPoint)socketCliente.RemoteEndPoint;
            

            string accion = "";
            string[] cadenaPorEspacios;
            string[] cadenaPorComas;

            //NullReferenceException no se capturan alguna variable apunta a null y no se captura

            using (NetworkStream ns = new NetworkStream(socketCliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                sw.WriteLine("Cliente conectado al puerto :"+infoCLiente.Port+" Con dirección: "+infoCLiente.Address);
                //sw.Flush();
                sw.AutoFlush = true; //con esto ya nos libramos de estar poniendo el flush
                
                while (!clienteAcabar)
                {
                    accion = sr.ReadLine();
                    if (accion != null)
                    {
                        cadenaPorEspacios = accion.Split(' '); /*divide la cadena por espacios que tenga,
                                                  * de esa forma sacamos la primera palabra accediendo a la posicion 0
                                                  Separamos la instruccion de los parámetros*/    
                        
                        switch (cadenaPorEspacios[0])
                        {

                            case "GET":
         
                                try
                                {
                                    cadenaPorComas = cadenaPorEspacios[1].Split(',');
                                    int nLineas = Convert.ToInt32(cadenaPorComas[cadenaPorComas.Length - 1]);
                                    string mensaje = LeeArchivo(cadenaPorComas[0], nLineas);
                                    sw.WriteLine(mensaje);
                                }
                                catch (FormatException)
                                {
                                    sw.WriteLine("Formato incorrecto: debe ser GET nombre_archivo,nlineas");
                                }
                                catch (Exception e)
                                {
                                    sw.WriteLine("Error:"+ e.Message);
                                }
                                break;
                            case "PORT":
                                try
                                {
                                    if (cadenaPorEspacios.Length >= 1)
                                    {
                                        GuardaPuerto(Convert.ToInt32(cadenaPorEspacios[1]));
                                        sw.WriteLine("Puerto guardado");
                                    }
                                }
                                catch (FormatException)
                                {
                                    sw.WriteLine("Formato incorrecto");
                                }
                                //avisar si se guardó
                                break;
                            case "LIST":
                                string ContenidoDelArchvio = ListaArchivos();
                                sw.WriteLine(ContenidoDelArchvio);
                                //mostrar la cadena
                                break;
                            case "CLOSE":
                                clienteAcabar = true;
                                socketCliente.Close();
                                break;
                            case "HALT":
                                lock (this)
                                {
                                    clienteAcabar = true;
                                    acabar = true;
                                    socketServidor.Close();
                                }
                                
                                break;
                            default:
                                
                                //acción no entendida
                                break;
                        }
                    }

                    
                }
               
            }

        }

        /*Se debe controlar que si se cierra el telnet de golpe que el servidor no falle.*/
    }
}
