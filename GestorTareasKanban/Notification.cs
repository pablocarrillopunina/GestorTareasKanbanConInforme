using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestorTareasKanban
{
    public static class Notification
    {
        public static void Show(string message, NotificationType type)
        {
            Form notify = new Form();
            notify.FormBorderStyle = FormBorderStyle.None;
            notify.StartPosition = FormStartPosition.Manual;
            notify.Size = new Size(300, 70);
            notify.TopMost = true;

            notify.BackColor = Color.White;
            notify.ShowInTaskbar = false;

            // Posición abajo derecha
            notify.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - notify.Width - 20,
                                        Screen.PrimaryScreen.WorkingArea.Height - notify.Height - 20);

            // ICONO
            PictureBox icon = new PictureBox();
            icon.Size = new Size(32, 32);
            icon.Location = new Point(15, 18);

            switch (type)
            {
                case NotificationType.Success:
                    icon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case NotificationType.Warning:
                    icon.Image = SystemIcons.Warning.ToBitmap();
                    break;

                case NotificationType.Error:
                    icon.Image = SystemIcons.Error.ToBitmap();
                    break;
            }

            // TEXTO DEL MENSAJE
            Label lbl = new Label();
            lbl.Text = message;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lbl.AutoSize = false;
            lbl.Size = new Size(230, 50);
            lbl.Location = new Point(60, 10);

            notify.Controls.Add(icon);
            notify.Controls.Add(lbl);

            // Timer de cierre automático
            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
            timer.Interval = 2500;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                notify.Close();
            };

            timer.Start();
            notify.Show();
        }
    }

    public enum NotificationType
    {
        Success,
        Warning,
        Error
    }
}
