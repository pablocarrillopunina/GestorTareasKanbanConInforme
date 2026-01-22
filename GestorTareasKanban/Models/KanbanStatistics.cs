using System.Collections.Generic;
using System.Linq;

namespace GestorTareasKanban.Models
{
    public class KanbanStatistics
    {
        public int Pendiente { get; private set; }
        public int EnProceso { get; private set; }
        public int Completado { get; private set; }

        public int Total => Pendiente + EnProceso + Completado;

        public KanbanStatistics(IEnumerable<TaskData> tareas)
        {
            Pendiente = tareas.Count(t => t.Estado == EstadoTarea.Pendiente);
            EnProceso = tareas.Count(t => t.Estado == EstadoTarea.EnProceso);
            Completado = tareas.Count(t => t.Estado == EstadoTarea.Completado);
        }
    }
}
