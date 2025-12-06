using System;

namespace GestorTareasKanban.Models
{
    public class TaskData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

      
        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    }
}
