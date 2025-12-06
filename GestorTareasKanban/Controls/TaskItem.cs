using System;
using System.Drawing;
using System.Windows.Forms;
using GestorTareasKanban.Models;

namespace GestorTareasKanban.Controls
{
    public partial class TaskItem : UserControl
    {
        private TextBox txtTitulo;
        private TextBox txtDescripcion;
        private Button btnEditar;
        private Button btnEliminar;

        public TaskData Tarea { get; private set; }

        public event EventHandler<TaskEventArgs> EditRequested;
        public event EventHandler<TaskEventArgs> DeleteRequested;

        public TaskItem()
        {
            InitializeComponent();
            BuildUI();
        }

        public TaskItem(TaskData tarea) : this()
        {
            SetTarea(tarea);
        }

        private void BuildUI()
        {
            this.Width = 260;
            this.Height = 120;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.White;

            // ---------------------------
            // TÍTULO
            // ---------------------------
            txtTitulo = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 25,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White,
                TextAlign = HorizontalAlignment.Left
            };

            // ---------------------------
            // DESCRIPCIÓN
            // ---------------------------
            txtDescripcion = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 45,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            txtDescripcion.TextChanged += (s, e) =>
            {
                txtDescripcion.Height = txtDescripcion.PreferredHeight;
            };

            // ---------------------------
            // BOTÓN EDITAR
            // ---------------------------
            btnEditar = new Button
            {
                Text = "Editar",
                Width = 70,
                Height = 25,
                Location = new Point(10, 80),
                BackColor = Color.FromArgb(255, 213, 0), // Amarillo
                FlatStyle = FlatStyle.Flat
            };
            btnEditar.FlatAppearance.BorderSize = 0;

            // ¡¡¡FALTABA ESTO!!!
            btnEditar.Click += BtnEditar_Click;

            // ---------------------------
            // BOTÓN ELIMINAR
            // ---------------------------
            btnEliminar = new Button
            {
                Text = "Eliminar",
                Width = 80,
                Height = 25,
                Location = new Point(90, 80),
                BackColor = Color.FromArgb(232, 17, 35), // Rojo
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEliminar.FlatAppearance.BorderSize = 0;

            // ¡¡¡FALTABA ESTO!!!
            btnEliminar.Click += BtnEliminar_Click;

            // Agregar al control
            this.Controls.Add(btnEditar);
            this.Controls.Add(btnEliminar);
            this.Controls.Add(txtDescripcion);
            this.Controls.Add(txtTitulo);
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            EditRequested?.Invoke(this, new TaskEventArgs(Tarea));
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Seguro que deseas eliminar esta tarea?",
                "Eliminar tarea",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2); // “No” por defecto

            if (result == DialogResult.Yes)
            {
                DeleteRequested?.Invoke(this, new TaskEventArgs(Tarea));
            }
        }


        public void SetTarea(TaskData tarea)
        {
            Tarea = tarea;
            ActualizarVisual();
        }

        public void ActualizarVisual()
        {
            if (Tarea == null) return;

            txtTitulo.Text = Tarea.Titulo;
            txtDescripcion.Text = Tarea.Descripcion;
        }
    }
}
