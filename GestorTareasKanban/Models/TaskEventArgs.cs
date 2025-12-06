using System;

namespace GestorTareasKanban.Models
{
    public class TaskEventArgs : EventArgs
    {
        public TaskData Tarea { get; }

        public TaskEventArgs(TaskData tarea)
        {
            Tarea = tarea;
        }
    }
}
