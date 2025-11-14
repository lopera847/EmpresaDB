using GestionEmpleados.clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GestionEmpleados
{
    public partial class Form1 : Form
    {
        ConexionBD conexion = new ConexionBD();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ListarDatos();
        }
        private void ListarDatos()
        {
            try
            {
                using (SqlConnection con = conexion.Conectar())
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM empleados", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEmpleados.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar empleados: " + ex.Message);
            }
        }

        private bool ValidarCamposInsertar()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios para agregar un empleado.",
                                "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtId.Text, out _))
            {
                MessageBox.Show("El campo ID debe contener solo números.",
                                "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txtEmail.Text, patronCorreo))
            {
                MessageBox.Show("Ingrese un correo electrónico válido (debe incluir '@' y un dominio).",
                                "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Validación para actualizar (solo ID obligatorio)
        private bool ValidarCamposActualizar()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Debe ingresar el ID del empleado a actualizar.",
                                "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtId.Text, out _))
            {
                MessageBox.Show("El campo ID debe contener solo números.",
                                "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(txtEmail.Text, patronCorreo))
                {
                    MessageBox.Show("El correo ingresado no tiene un formato válido.",
                                    "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtApellido.Clear();
            txtNombre.Clear();
            txtDireccion.Clear();
            txtEmail.Clear();
            txtId.Focus();
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposInsertar()) return;

            using (SqlConnection con = conexion.Conectar())
            {
                string query = "INSERT INTO empleados VALUES (@id, @apellido, @nombre, @direccion, @correo)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", txtId.Text);
                cmd.Parameters.AddWithValue("@apellido", txtApellido.Text);
                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                cmd.Parameters.AddWithValue("@correo", txtEmail.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Empleado agregado correctamente.");
                ListarDatos();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposActualizar()) return;

            using (SqlConnection con = conexion.Conectar())
            {
               
                string query = "UPDATE empleados SET ";
                var campos = new List<string>();

                if (!string.IsNullOrWhiteSpace(txtApellido.Text)) campos.Add("apellido=@apellido");
                if (!string.IsNullOrWhiteSpace(txtNombre.Text)) campos.Add("nombre=@nombre");
                if (!string.IsNullOrWhiteSpace(txtDireccion.Text)) campos.Add("direccion=@direccion");
                if (!string.IsNullOrWhiteSpace(txtEmail.Text)) campos.Add("email=@correo");

                if (campos.Count == 0)
                {
                    MessageBox.Show("Debe ingresar al menos un campo para actualizar.",
                                    "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                query += string.Join(", ", campos) + " WHERE idempleado=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", txtId.Text);
                if (!string.IsNullOrWhiteSpace(txtApellido.Text)) cmd.Parameters.AddWithValue("@apellido", txtApellido.Text);
                if (!string.IsNullOrWhiteSpace(txtNombre.Text)) cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                if (!string.IsNullOrWhiteSpace(txtDireccion.Text)) cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                if (!string.IsNullOrWhiteSpace(txtEmail.Text)) cmd.Parameters.AddWithValue("@correo", txtEmail.Text);

                int filas = cmd.ExecuteNonQuery();

                if (filas > 0)
                    MessageBox.Show("Empleado actualizado correctamente.");
                else
                    MessageBox.Show("No se encontró un empleado con ese ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ListarDatos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Debe ingresar el ID del empleado a eliminar.", "Error de validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = conexion.Conectar())
                {
                    string query = "DELETE FROM empleados WHERE idempleado=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", txtId.Text);
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        MessageBox.Show("Empleado eliminado correctamente.");
                    else
                        MessageBox.Show("No se encontró un empleado con ese ID.");

                    ListarDatos();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar empleado: " + ex.Message);
            }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            ListarDatos();
        }
    }
}
