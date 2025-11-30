using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace CapaPresentacion
{
    public partial class frmMarca : Form
    {
        public frmMarca()
        {
            InitializeComponent();
        }

        private void frmMarca_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });

            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;

            // Cargar columnas para búsqueda
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar" && columna.Name != "IdMarca")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            // Columna checkbox
            if (!dgvdata.Columns.Contains("btnseleccionar"))
            {
                DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "",
                    Width = 30,
                    Name = "btnseleccionar",
                    ReadOnly = false
                };
                dgvdata.Columns.Insert(0, checkColumn);
            }

            // Cargar MARCAS
            List<Marca> lista = new CN_Marca().Listar();

            foreach (Marca item in lista)
            {
                dgvdata.Rows.Add(new object[]
                {
                    false,
                    item.IdMarca,
                    item.Nombre,
                    item.LugarOrigen,
                    item.Estado == true ? 1 : 0,
                    item.Estado == true ? "Activo" : "No Activo"
                });
            }
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Marca obj = new Marca()
            {
                IdMarca = Convert.ToInt32(txtid.Text),
                Nombre = txtMarca.Text,
                LugarOrigen = txtLugar.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.IdMarca == 0)
            {
                // Registrar
                int idMarcagenerado = new CN_Marca().Registrar(obj, out mensaje);

                if (idMarcagenerado != 0)
                {
                    dgvdata.Rows.Add(new object[]
                    {
                        false,
                        idMarcagenerado,
                        txtMarca.Text,
                        txtLugar.Text,
                        ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()
                    });

                    Limpiar();
                    MessageBox.Show("Marca registrada exitosamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                // Editar
                bool resultado = new CN_Marca().Editar(obj, out mensaje);

                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["IdMarca"].Value = txtid.Text;
                    row.Cells["Nombre"].Value = txtMarca.Text;
                    row.Cells["LugarOrigen"].Value = txtLugar.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();

                    Limpiar();
                    MessageBox.Show("Marca editada exitosamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
        }

        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            txtMarca.Text = "";
            txtLugar.Text = "";
            cboestado.SelectedIndex = 0;
            txtMarca.Select();
        }


        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.check20.Width;
                var h = Properties.Resources.check20.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.check20, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }


        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /*if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["IdMarca"].Value.ToString();
                    txtMarca.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtLugar.Text = dgvdata.Rows[indice].Cells["LugarOrigen"].Value.ToString();

                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = cboestado.Items.IndexOf(oc);
                            break;
                        }
                    }
                }
            }*/

            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtindice.Text = indice.ToString();

                    // VALIDACIÓN SEGURA
                    var valor = dgvdata.Rows[indice].Cells["IdMarca"].Value;

                    if (valor != null && int.TryParse(valor.ToString(), out int idMarca))
                    {
                        txtid.Text = idMarca.ToString();
                    }
                    else
                    {
                        MessageBox.Show("El valor de la marca seleccionada no es válido.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtMarca.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtLugar.Text = dgvdata.Rows[indice].Cells["LugarOrigen"].Value.ToString();

                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            cboestado.SelectedIndex = cboestado.Items.IndexOf(oc);
                            break;
                        }
                    }
                }
            }

        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar la marca?", "Mensaje",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Marca obj = new Marca()
                    {
                        IdMarca = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN_Marca().Eliminar(obj, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    row.Visible =
                        row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper()
                        .Contains(txtbusqueda.Text.Trim().ToUpper());
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnexportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crear tabla dinámica basada en columnas visibles
            var tabla = new DataTable();

            List<int> columnasExportables = new List<int>();

            foreach (DataGridViewColumn col in dgvdata.Columns)
            {
                if (col.Visible && col.Name != "btnseleccionar")
                {
                    tabla.Columns.Add(col.HeaderText);
                    columnasExportables.Add(col.Index);
                }
            }

            // Agregar filas correspondientes
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                var valores = new List<object>();

                foreach (int index in columnasExportables)
                    valores.Add(row.Cells[index].Value);

                tabla.Rows.Add(valores.ToArray());
            }

            using (var sfd = new SaveFileDialog() { Filter = "Archivo CSV (*.csv)|*.csv", FileName = "MarcasExportadas.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    /*using (var sw = new StreamWriter(sfd.FileName))*/
                    using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // Escribir encabezados
                        sw.WriteLine(string.Join(";", tabla.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                        // Escribir filas
                        foreach (DataRow dr in tabla.Rows)
                            sw.WriteLine(string.Join(";", dr.ItemArray));
                    }

                    MessageBox.Show("Exportación completada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
