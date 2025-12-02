using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmPromociones : Form
    {
        public frmPromociones()
        {
            InitializeComponent();
        }

        private void frmPromociones_Load(object sender, EventArgs e)
        {
            // Configurar estado
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;

            // Configurar fechas por defecto
            dtpfechainicio.Value = DateTime.Now;
            dtpfechafin.Value = DateTime.Now.AddDays(30);

            // Cargar columnas para búsqueda
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar" && columna.Name != "IdPromocion")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            if (cbobusqueda.Items.Count > 0)
            {
                cbobusqueda.SelectedIndex = 0;
            }

            // Cargar datos
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgvdata.Rows.Clear();

            List<Promocion> lista = new CN_Promocion().Listar();

            foreach (Promocion item in lista)
            {
                dgvdata.Rows.Add(new object[] {
                    "",
                    item.IdPromocion,
                    item.Nombre,
                    item.Descripcion,
                    item.PorcentajeDescuento.ToString("0.00") + "%",
                    item.FechaInicio.ToString("dd/MM/yyyy"),
                    item.FechaFin.ToString("dd/MM/yyyy"),
                    item.Estado == true ? 1 : 0,
                    item.Estado == true ? "Activo" : "No Activo"
                });
            }
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtnombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (decimal.TryParse(txtporcentaje.Text, out decimal porcentaje))
            {
                if (porcentaje < 0 || porcentaje > 100)
                {
                    MessageBox.Show("El porcentaje debe estar entre 0 y 100", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("El porcentaje debe ser un número válido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpfechafin.Value <= dtpfechainicio.Value)
            {
                MessageBox.Show("La fecha de fin debe ser mayor a la fecha de inicio", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Promocion obj = new Promocion()
            {
                IdPromocion = Convert.ToInt32(txtid.Text),
                Nombre = txtnombre.Text,
                Descripcion = txtdescripcion.Text,
                PorcentajeDescuento = porcentaje,
                FechaInicio = dtpfechainicio.Value,
                FechaFin = dtpfechafin.Value,
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.IdPromocion == 0)
            {
                // Registrar
                int idGenerado = new CN_Promocion().Registrar(obj, out mensaje);

                if (idGenerado != 0)
                {
                    CargarDatos();
                    Limpiar();
                    MessageBox.Show("Promoción registrada exitosamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Editar
                bool resultado = new CN_Promocion().Editar(obj, out mensaje);

                if (resultado)
                {
                    CargarDatos();
                    Limpiar();
                    MessageBox.Show("Promoción editada exitosamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            txtnombre.Text = "";
            txtdescripcion.Text = "";
            txtporcentaje.Text = "";
            dtpfechainicio.Value = DateTime.Now;
            dtpfechafin.Value = DateTime.Now.AddDays(30);
            cboestado.SelectedIndex = 0;
            txtnombre.Select();
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar la promoción?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Promocion obj = new Promocion()
                    {
                        IdPromocion = Convert.ToInt32(txtid.Text)
                    };

                    bool respuesta = new CN_Promocion().Eliminar(obj, out mensaje);

                    if (respuesta)
                    {
                        CargarDatos();
                        Limpiar();
                        MessageBox.Show("Promoción eliminada exitosamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["IdPromocion"].Value.ToString();
                    txtnombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    
                    if (dgvdata.Rows[indice].Cells["Descripcion"].Value != null)
                        txtdescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
                    else
                        txtdescripcion.Text = "";

                    string porcentajeTexto = dgvdata.Rows[indice].Cells["PorcentajeDescuento"].Value.ToString().Replace("%", "");
                    txtporcentaje.Text = porcentajeTexto;

                    if (DateTime.TryParse(dgvdata.Rows[indice].Cells["FechaInicio"].Value.ToString(), out DateTime fechaInicio))
                        dtpfechainicio.Value = fechaInicio;

                    if (DateTime.TryParse(dgvdata.Rows[indice].Cells["FechaFin"].Value.ToString(), out DateTime fechaFin))
                        dtpfechafin.Value = fechaFin;

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

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Cells[columnaFiltro].Value != null)
                    {
                        row.Visible = row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper()
                            .Contains(txtbusqueda.Text.Trim().ToUpper());
                    }
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
    }
}
