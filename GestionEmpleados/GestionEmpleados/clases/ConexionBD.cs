using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GestionEmpleados.clases
{
    internal class ConexionBD
    {
        private string servidor = "localhost";
        private string bd = "EmpresaDB";
        private string usuario = "sa";         // usuario SQL Server
        private string contrasena = "1234";  // contraseña

        public SqlConnection Conectar()
        {
            string cadena = $"Server={servidor};Database={bd};User Id={usuario};Password={contrasena};";
            SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            return conexion;
        }
    }
}