using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmRecursoInterno : Form
    {
        private CN_RecursoInterno _cnRecurso = new CN_RecursoInterno();

        public frmRecursoInterno()
        {
            InitializeComponent();
        }

        private void frmRecursoInterno_Load(object sender, EventArgs e)
        {
            CargarRecursos();
            LimpiarCampos();
        }

        private void CargarRecursos()
        {
            dgvRecursos.Rows.Clear();

            foreach (RecursoInterno r in _cnRecurso.Listar())
            {
                dgvRecursos.Rows.Add(
                    r.IdRecurso,
                    r.NombreRecurso,
                    r.TipoRecurso,
                    r.Cantidad,
                    r.Ubicacion,
                    r.Estado ? "Activo" : "Inactivo"
                );
            }
        }

        private void LimpiarCampos()
        {
            txtIdRecurso.Text = string.Empty;
            txtNombreRecurso.Text = string.Empty;
            txtTipoRecurso.Text = string.Empty;
            txtCantidad.Text = string.Empty;
            txtUbicacion.Text = string.Empty;
            chkEstado.Checked = true;

            txtNombreRecurso.Focus();
        }

        private RecursoInterno ObtenerDesdeFormulario()
        {
            int id = 0;
            int.TryParse(txtIdRecurso.Text, out id);

            int cantidad = 0;
            int.TryParse(txtCantidad.Text, out cantidad);

            return new RecursoInterno
            {
                IdRecurso = id,
                NombreRecurso = txtNombreRecurso.Text.Trim(),
                TipoRecurso = txtTipoRecurso.Text.Trim(),
                Cantidad = cantidad,
                Ubicacion = txtUbicacion.Text.Trim(),
                Estado = chkEstado.Checked
            };
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            // Validación simple de cantidad numérica
            int cantidadTemp;
            if (!int.TryParse(txtCantidad.Text, out cantidadTemp))
            {
                MessageBox.Show("La cantidad debe ser un número entero.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RecursoInterno obj = ObtenerDesdeFormulario();

            int idGenerado = _cnRecurso.Registrar(obj, out mensaje);

            if (idGenerado != 0)
            {
                MessageBox.Show("Recurso registrado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarRecursos();
                LimpiarCampos();
            }
            else
            {
                if (!string.IsNullOrEmpty(mensaje))
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("No se pudo registrar el recurso.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdRecurso.Text))
            {
                MessageBox.Show("Seleccione un recurso de la lista para editar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje = string.Empty;

            int cantidadTemp;
            if (!int.TryParse(txtCantidad.Text, out cantidadTemp))
            {
                MessageBox.Show("La cantidad debe ser un número entero.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RecursoInterno obj = ObtenerDesdeFormulario();

            bool resultado = _cnRecurso.Editar(obj, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Recurso actualizado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarRecursos();
                LimpiarCampos();
            }
            else
            {
                if (!string.IsNullOrEmpty(mensaje))
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("No se pudo actualizar el recurso.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdRecurso.Text))
            {
                MessageBox.Show("Seleccione un recurso de la lista para eliminar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Desea eliminar el recurso seleccionado?", "Mensaje",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            string mensaje = string.Empty;
            int id = int.Parse(txtIdRecurso.Text);

            bool resultado = _cnRecurso.Eliminar(id, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Recurso eliminado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarRecursos();
                LimpiarCampos();
            }
            else
            {
                if (!string.IsNullOrEmpty(mensaje))
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("No se pudo eliminar el recurso.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvRecursos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvRecursos.Rows.Count > 0)
            {
                DataGridViewRow row = dgvRecursos.Rows[e.RowIndex];

                txtIdRecurso.Text = row.Cells["IdRecurso"].Value.ToString();
                txtNombreRecurso.Text = row.Cells["NombreRecurso"].Value.ToString();
                txtTipoRecurso.Text = row.Cells["TipoRecurso"].Value.ToString();
                txtCantidad.Text = row.Cells["Cantidad"].Value.ToString();
                txtUbicacion.Text = row.Cells["Ubicacion"].Value.ToString();

                string estadoTexto = row.Cells["Estado"].Value.ToString();
                chkEstado.Checked = (estadoTexto == "Activo");
            }
        }
    }
}
