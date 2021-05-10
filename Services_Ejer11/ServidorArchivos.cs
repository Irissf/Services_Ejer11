using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services_Ejer11
{
    class ServidorArchivos
    {
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
            int lineasLeidas = 0;
            try
            {
                using (StreamReader sr = new StreamReader(nombreArchivo))
                {
                    while (lineasLeidas <= nLineas)
                    {
                        texto += sr.ReadLine();
                        texto += "\n";
                        lineasLeidas++;
                    }
                }

            }
            catch (Exception)
            {
                Console.WriteLine("No se encontró el archivo en: " + nombreArchivo);
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
            return 0;
        }

        /// <summary>
        ///  guardaPuerto(numero): Guarda el parámetro numero en C:\%EXAMEN%\puerto.txt creándolo si no 
        ///  existe o sobreescribiéndolo si ya existe.
        /// </summary>
        /// <param name="numero"></param>
        public void GuardaPuerto(int numero)
        {

        }

        /// <summary>
        /// listaArchivos(): Método que obtiene la lista de archivos de extensión txt del directorio EXAMEN
        /// y la devuelve como una cadena única(los nombres de los archivos separados por retornos de carro. No
        /// se incluye la trayectoria).
        /// </summary>
        public void ListaArchivos()
        {

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

        }

        /*Se debe controlar que si se cierra el telnet de golpe que el servidor no falle.*/
    }
}
