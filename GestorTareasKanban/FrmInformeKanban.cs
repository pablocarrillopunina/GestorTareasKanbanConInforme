using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GestorTareasKanban.Models;
using ClosedXML.Excel;

namespace GestorTareasKanban
{
    public partial class FrmInformeKanban : Form
    {
        private Label lblTitulo;
        private DataGridView dgvTareas;
        private Chart chartEstados;

        private Label lblTotal;
        private Label lblPorcentaje;
        private Button btnExportarExcel;
        private ComboBox cboTipoGrafico;
        private ComboBox cboEstado;
        private TextBox txtUsuario;

        public FrmInformeKanban()
        {
            InitializeComponent();
            InicializarInterfaz();
            CargarDatos();
        }

        private void InicializarInterfaz()
        {
            // ----- FORM -----
            Text = "Informe Kanban";
            Width = 1000;
            Height = 780;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 248); // gris claro dashboard

            // ----- TÍTULO -----
            lblTitulo = new Label
            {
                Text = "Estado - Kanban",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // ----- GRID -----
            dgvTareas = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(950, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            dgvTareas.Columns.Add("colTitulo", "Tarea");
            dgvTareas.Columns.Add("colDescripcion", "Descripción");
            dgvTareas.Columns.Add("colEstado", "Estado");

            // ----- GRÁFICO -----
            chartEstados = new Chart
            {
                Location = new Point(20, 400),
                Size = new Size(450, 300),
                BackColor = Color.White
            };

            ChartArea area = new ChartArea();
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartEstados.ChartAreas.Add(area);
            chartEstados.Legends.Add(new Legend());

            // ----- RESUMEN -----
            lblTotal = new Label
            {
                Location = new Point(500, 420),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };

            lblPorcentaje = new Label
            {
                Location = new Point(500, 450),
                Font = new Font("Segoe UI", 11),
                AutoSize = true
            };

            // ----- EXPORTAR -----
            btnExportarExcel = new Button
            {
                Text = "Exportar a Excel",
                Location = new Point(500, 500),
                Width = 170,
                Height = 32
            };
            btnExportarExcel.Click += BtnExportarExcel_Click;

            // ----- SELECTORES -----
            cboTipoGrafico = new ComboBox
            {
                Location = new Point(500, 550),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTipoGrafico.Items.AddRange(new[] { "Circular", "Barras", "Áreas" });
            cboTipoGrafico.SelectedIndex = 0;
            cboTipoGrafico.SelectedIndexChanged += (_, __) => CargarDatos();

            cboEstado = new ComboBox
            {
                Location = new Point(700, 550),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboEstado.Items.AddRange(new[] { "Todos", "Pendiente", "EnProceso", "Completado" });
            cboEstado.SelectedIndex = 0;
            cboEstado.SelectedIndexChanged += (_, __) => CargarDatos();

            txtUsuario = new TextBox
            {
                Location = new Point(700, 500),
                Width = 170,
                PlaceholderText = "Filtrar por usuario"
            };
            txtUsuario.TextChanged += (_, __) => CargarDatos();

            Controls.AddRange(new Control[]
            {
                lblTitulo, dgvTareas, chartEstados,
                lblTotal, lblPorcentaje,
                btnExportarExcel, cboTipoGrafico,
                cboEstado, txtUsuario
            });
        }

        private void CargarDatos()
        {
            var tareas = ObtenerTareas();

            if (cboEstado.SelectedItem.ToString() != "Todos")
            {
                tareas = tareas
                    .Where(t => t.Estado.ToString() == cboEstado.SelectedItem.ToString())
                    .ToList();
            }

            dgvTareas.Rows.Clear();
            foreach (var t in tareas)
                dgvTareas.Rows.Add(t.Titulo, t.Descripcion, t.Estado);

            KanbanStatistics stats = new KanbanStatistics(tareas);

            lblTotal.Text = $"Total de tareas: {stats.Total}";
            lblPorcentaje.Text = $"Completadas: {(stats.Total == 0 ? 0 : (double)stats.Completado / stats.Total * 100):0.##}%";

            DibujarGrafico(stats);
        }

        private void DibujarGrafico(KanbanStatistics stats)
        {
            chartEstados.Series.Clear();

            string tipo = cboTipoGrafico.SelectedItem.ToString();

            if (tipo == "Circular")
            {
                Series s = new Series
                {
                    ChartType = SeriesChartType.Doughnut
                };

                s.Points.AddXY("Pendiente", stats.Pendiente);
                s.Points[0].Color = Color.IndianRed;

                s.Points.AddXY("En Proceso", stats.EnProceso);
                s.Points[1].Color = Color.Goldenrod;

                s.Points.AddXY("Completado", stats.Completado);
                s.Points[2].Color = Color.ForestGreen;

                chartEstados.Series.Add(s);
            }
            else
            {
                SeriesChartType chartType = tipo == "Barras"
                    ? SeriesChartType.Column
                    : SeriesChartType.Area;

                chartEstados.Series.Add(CrearSerie("Pendiente", stats.Pendiente, chartType, Color.IndianRed));
                chartEstados.Series.Add(CrearSerie("En Proceso", stats.EnProceso, chartType, Color.Goldenrod));
                chartEstados.Series.Add(CrearSerie("Completado", stats.Completado, chartType, Color.ForestGreen));
            }
        }

        private Series CrearSerie(string nombre, int valor, SeriesChartType tipo, Color color)
        {
            Series s = new Series(nombre)
            {
                ChartType = tipo,
                Color = color
            };
            s.Points.AddXY("Estados", valor); // MISMA X → clave para Áreas
            return s;
        }

        private void BtnExportarExcel_Click(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = "InformeKanban.xlsx"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var tareas = ObtenerTareas();
            KanbanStatistics stats = new KanbanStatistics(tareas);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Informe");

            ws.Cell(1, 1).Value = "Tarea";
            ws.Cell(1, 2).Value = "Descripción";
            ws.Cell(1, 3).Value = "Estado";

            int row = 2;
            foreach (var t in tareas)
            {
                ws.Cell(row, 1).Value = t.Titulo;
                ws.Cell(row, 2).Value = t.Descripcion;
                ws.Cell(row, 3).Value = t.Estado.ToString();
                row++;
            }

            row += 2;
            ws.Cell(row, 1).Value = "Total";
            ws.Cell(row, 2).Value = stats.Total;
            row++;
            ws.Cell(row, 1).Value = "Completadas (%)";
            ws.Cell(row, 2).Value = stats.Total == 0 ? 0 : (double)stats.Completado / stats.Total * 100;

            ws.Columns().AdjustToContents();
            wb.SaveAs(sfd.FileName);
        }

        private List<TaskData> ObtenerTareas()
        {
            try { return TaskStorage.Load(); }
            catch { return new List<TaskData>(); }
        }
    }
}
