using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services_Ejer11
{
    /*
        Se desea realizar un servidor de archivos de texto sencillo. Los clientes serán consolas telnet. Para ello lo 
        primero que debes hacer en tu equipo es crear manualmente una variable de entorno denominada 
        EXAMEN que apunte a un subdirectorio denominado ArchivosTexto del directorio del usuario actual. Para 
        ello abre una consola y ejecuta el comando: 
        setx EXAMEN %homedrive%%homepath%\ArchivosTexto
        Reinicia el terminal y comprueba mediante el comando set que se ha creado correctamente. A continuación
        se pide:
        Crea un nuevo proyecto de consola denominado ExamenSERVApellidoNombre. A continuación crea una 
        clase denominada ServidorArchivos. En dicha clase habrá al menos los siguientes métodos públicos NO 
        estáticos
    */
    class Program
    {
        
        static void Main(string[] args)
        {
            /*En el programa principal que estará en la clase Program simplemente se crea un objeto del tipo Servidor y 
            se llama a la función iniciaServicioChat.*/
            ServidorArchivos sa = new ServidorArchivos();
            //string ruta = Environment.GetEnvironmentVariable("EXAMEN");
            //Console.WriteLine("la ruta es:"+ruta);
            //Console.ReadLine();

            sa.IniciaServidorArchivos();
        }
    }
}
