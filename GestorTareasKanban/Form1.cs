using GestorTareasKanban.Controls;

namespace GestorTareasKanban
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var board = new TaskBoard();
            board.Dock = DockStyle.Fill;
            Controls.Add(board);
        }
    }
}
