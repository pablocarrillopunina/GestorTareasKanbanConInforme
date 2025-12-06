using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GestorTareasKanban.Models;

namespace GestorTareasKanban.Controls
{
    public partial class TaskBoard : UserControl
    {
        private FlowLayoutPanel colPendiente;
        private FlowLayoutPanel colEnProceso;
        private FlowLayoutPanel colCompletado;

        public TaskBoard()
        {
            InitializeComponent();
            BuildUI();
            CargarTareasDesdeJSON();
        }

        // -------------------------------------------------------------
        // PANEL PRINCIPAL DEL TABLERO
        // -------------------------------------------------------------
        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.LightGray;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            colPendiente = CreateColumn("Pendiente", Color.LightPink, EstadoTarea.Pendiente);
            colEnProceso = CreateColumn("En Proceso", Color.LightYellow, EstadoTarea.EnProceso);
            colCompletado = CreateColumn("Completado", Color.LightGreen, EstadoTarea.Completado);

            layout.Controls.Add(colPendiente, 0, 0);
            layout.Controls.Add(colEnProceso, 1, 0);
            layout.Controls.Add(colCompletado, 2, 0);

            this.Controls.Add(layout);
        }

        // -------------------------------------------------------------
        // CREAR COLUMNA CON BOTÓN + TÍTULO
        // -------------------------------------------------------------
        private FlowLayoutPanel CreateColumn(string titulo, Color color, EstadoTarea estado)
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = color,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                AllowDrop = true
            };

            panel.DragEnter += Panel_DragEnter;
            panel.DragDrop += Panel_DragDrop;

            var lbl = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lbl);

            var btnAdd = new Button
            {
                Text = "+ Añadir tarea",
                Height = 30,
                Width = 120,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;

            btnAdd.Click += (s, e) => CrearNuevaTarea(estado);

            panel.Controls.Add(btnAdd);

            return panel;
        }

        // -------------------------------------------------------------
        // CREAR NUEVA TAREA
        // -------------------------------------------------------------
        private void CrearNuevaTarea(EstadoTarea estado)
        {
            var form = new Form
            {
                Text = "Nueva tarea",
                Size = new Size(350, 300),
                BackColor = Color.WhiteSmoke,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var lblTitulo = new Label
            {
                Text = "Título",
                Left = 20,
                Top = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            form.Controls.Add(lblTitulo);

            var txtTitulo = new TextBox
            {
                Left = 20,
                Top = 45,
                Width = 280
            };
            form.Controls.Add(txtTitulo);

            var lblDesc = new Label
            {
                Text = "Descripción",
                Left = 20,
                Top = 85,
                Width = 300,        // <<< AÑADIDO
                AutoSize = false,   // <<< AÑADIDO
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            form.Controls.Add(lblDesc);


            var txtDesc = new TextBox
            {
                Left = 20,
                Top = 110,
                Width = 300,      // <<< más ancho
                Height = 80,
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            form.Controls.Add(txtDesc);


            var btnOk = new Button
            {
                Text = "Crear",
                Left = 20,
                Top = 200,
                Width = 100,
                Height = 32,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Show("El título es obligatorio");
                    return;
                }

                var tarea = new TaskData
                {
                    Titulo = txtTitulo.Text,
                    Descripcion = txtDesc.Text,
                    Estado = estado
                };

                AddTask(tarea);
                GuardarTareasEnJSON();
                MostrarNotificacion("📝 Tarea creada");

                form.Close();
            };

            form.Controls.Add(btnOk);

            form.ShowDialog();
        }

        // -------------------------------------------------------------
        // AÑADIR TAREA AL TABLERO
        // -------------------------------------------------------------
        public void AddTask(TaskData tarea)
        {
            var item = new TaskItem(tarea);
            item.Margin = new Padding(10);
            item.EditRequested += Item_EditRequested;
            item.DeleteRequested += Item_DeleteRequested;

            item.MouseDown += (s, e) => item.DoDragDrop(item, DragDropEffects.Move);

            if (tarea.Estado == EstadoTarea.Pendiente)
                colPendiente.Controls.Add(item);
            else if (tarea.Estado == EstadoTarea.EnProceso)
                colEnProceso.Controls.Add(item);
            else
                colCompletado.Controls.Add(item);
        }

        // -------------------------------------------------------------
        // ELIMINAR TAREA
        // -------------------------------------------------------------
        private void Item_DeleteRequested(object sender, TaskEventArgs e)
        {
            foreach (FlowLayoutPanel col in new[] { colPendiente, colEnProceso, colCompletado })
            {
                foreach (Control c in col.Controls)
                {
                    if (c is TaskItem ti && ti.Tarea.Id == e.Tarea.Id)
                    {
                        col.Controls.Remove(ti);
                        GuardarTareasEnJSON();
                        MostrarNotificacion("🗑️ Tarea eliminada");
                        return;
                    }
                }
            }
        }

        // -------------------------------------------------------------
        // EDITAR TAREA
        // -------------------------------------------------------------
        private void Item_EditRequested(object sender, TaskEventArgs e)
        {
            var taskItem = sender as TaskItem;

            Form form = new Form
            {
                Text = "Editar tarea",
                Size = new Size(350, 300),
                BackColor = Color.WhiteSmoke,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var lblTitulo = new Label
            {
                Text = "Título",
                Left = 20,
                Top = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            form.Controls.Add(lblTitulo);

            var txtTitulo = new TextBox
            {
                Text = e.Tarea.Titulo,
                Left = 20,
                Top = 45,
                Width = 280
            };
            form.Controls.Add(txtTitulo);

            var lblDesc = new Label
            {
                Text = "Descripción",
                Top = 70,
                Left = 10,
                Width = 300,
                AutoSize = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };


            var txtDesc = new TextBox
            {
                Text = e.Tarea.Descripcion,
                Left = 20,
                Top = 110,
                Width = 280,
                Height = 70,
                Multiline = true
            };
            form.Controls.Add(txtDesc);

            var btnOk = new Button
            {
                Text = "Guardar",
                Top = txtDesc.Bottom + 15,
                Width = 140,
                Height = 36,
                Left = (form.ClientSize.Width - 140) / 2, // Centrado
                BackColor = Color.FromArgb(0, 150, 80),   // Verde moderno
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;


            btnOk.Click += (s2, e2) =>
            {
                e.Tarea.Titulo = txtTitulo.Text;
                e.Tarea.Descripcion = txtDesc.Text;

                taskItem.ActualizarVisual();
                GuardarTareasEnJSON();
                MostrarNotificacion("✏️ Tarea editada");

                form.Close();
            };

            form.Controls.Add(btnOk);

            form.ShowDialog();
        }

        // -------------------------------------------------------------
        // NOTIFICACIÓN VISUAL
        // -------------------------------------------------------------
        private void MostrarNotificacion(string mensaje)
        {
            Label notif = new Label();
            notif.Text = mensaje;
            notif.BackColor = Color.FromArgb(50, 180, 80);
            notif.ForeColor = Color.White;
            notif.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            notif.AutoSize = false;
            notif.Size = new Size(260, 40);
            notif.TextAlign = ContentAlignment.MiddleCenter;
            notif.BorderStyle = BorderStyle.FixedSingle;

            notif.Location = new Point(this.Width - notif.Width - 20, 20);

            this.Controls.Add(notif);
            notif.BringToFront();

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000;
            timer.Tick += (s, e) =>
            {
                notif.Dispose();
                timer.Stop();
            };
            timer.Start();
        }

        // -------------------------------------------------------------
        // DRAG & DROP ENTRE COLUMNAS
        // -------------------------------------------------------------
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TaskItem)))
                e.Effect = DragDropEffects.Move;
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            var taskItem = (TaskItem)e.Data.GetData(typeof(TaskItem));
            var target = sender as FlowLayoutPanel;

            (taskItem.Parent as FlowLayoutPanel)?.Controls.Remove(taskItem);
            target.Controls.Add(taskItem);

            if (target == colPendiente)
                taskItem.Tarea.Estado = EstadoTarea.Pendiente;
            else if (target == colEnProceso)
                taskItem.Tarea.Estado = EstadoTarea.EnProceso;
            else
            {
                taskItem.Tarea.Estado = EstadoTarea.Completado;
                MostrarNotificacion("✔ Tarea completada");
            }

            GuardarTareasEnJSON();
        }

        // -------------------------------------------------------------
        // PERSISTENCIA JSON
        // -------------------------------------------------------------
        private void GuardarTareasEnJSON()
        {
            var lista = new List<TaskData>();

            foreach (FlowLayoutPanel col in new[] { colPendiente, colEnProceso, colCompletado })
            {
                foreach (Control c in col.Controls)
                {
                    if (c is TaskItem item)
                        lista.Add(item.Tarea);
                }
            }

            TaskStorage.Save(lista);
        }

        private void CargarTareasDesdeJSON()
        {
            var tareas = TaskStorage.Load();
            foreach (var t in tareas)
                AddTask(t);
        }
    }
}
