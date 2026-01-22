using System;
using System.Drawing;
using System.Windows.Forms;
using GestorTareasKanban.Controls;

namespace GestorTareasKanban
{
    public partial class Form1 : Form
    {
        private Button btnInforme;
        private Panel panelSuperior;

        public Form1()
        {
            InitializeComponent();
            InicializarLayout();
        }

        private void InicializarLayout()
        {
            // ----- PANEL SUPERIOR -----
            panelSuperior = new Panel();
            panelSuperior.Height = 45;
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.BackColor = Color.LightGray;

            btnInforme = new Button();
            btnInforme.Text = "Ver informe";
            btnInforme.Width = 120;
            btnInforme.Height = 30;
            btnInforme.Location = new Point(10, 7);
            btnInforme.Click += BtnInforme_Click;

            panelSuperior.Controls.Add(btnInforme);

            // ----- TABLERO KANBAN -----
            var board = new TaskBoard();
            board.Dock = DockStyle.Fill;

            // ----- AÑADIR CONTROLES EN ORDEN -----
            Controls.Add(board);
            Controls.Add(panelSuperior);
        }

        private void BtnInforme_Click(object sender, EventArgs e)
        {
            try
            {
                FrmInformeKanban informe = new FrmInformeKanban();
                informe.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo abrir el informe.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
