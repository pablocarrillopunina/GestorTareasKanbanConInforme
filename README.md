# GestorTareasKanban – Proyecto WinForms DAM

Este proyecto implementa un tablero Kanban desarrollado en .NET WinForms, cumpliendo con los requisitos de la práctica “Crear un Control de Usuario Personalizado para Gestión de Tareas” del módulo de Componentes Visuales. Permite gestionar tareas mediante controles reutilizables con funcionalidades de crear, editar, eliminar y mover tareas entre columnas.

---

## 🧩 Características Principales

### 1. Control `TaskItem` (Componente Tarea)
Representa una tarjeta individual con:
- Título  
- Descripción  
- Botón **Editar**  
- Botón **Eliminar**  

Incluye eventos personalizados para notificar al tablero de las acciones.

### 2. Control `TaskBoard` (Tablero Kanban)
El tablero contiene tres columnas:
- **Pendiente**
- **En Proceso**
- **Completado**

Cada columna permite agregar tareas y aceptar tareas arrastradas desde otras columnas.

### 3. Interactividad – Drag & Drop
Las tareas pueden arrastrarse y soltarse entre columnas, actualizando su estado automáticamente mediante los eventos DragEnter, DoDragDrop y DragDrop.

### 4. Lógica de Negocio (CRUD Completo)
El proyecto permite:
- Crear nuevas tareas  
- Editarlas  
- Eliminarlas  
- Moverlas de columna manteniendo coherencia en el estado

### 5. Diseño y Estilo
Cada columna usa colores diferenciados.  
El diseño es claro, visual y organizado, siguiendo las recomendaciones de la práctica.

---

## ⭐ Extensiones Opcionales Implementadas
- ✔ Notificaciones visuales  
- ✔ Organización de código clara  
- (Opcional) Persistencia en JSON  
- (Opcional) Personalización de columnas  

---

## 📂 Estructura del Proyecto

<img width="247" height="487" alt="image" src="https://github.com/user-attachments/assets/864bb0cc-7871-4cf2-9436-6be100c60710" />

## 🚀 Cómo Ejecutarlo
1. Abrir el proyecto en Visual Studio  
2. Compilar con **Ctrl + Shift + B**  
3. Ejecutar con **F5**

---

## 👨‍💻 Autor
**Pablo Carrillo** – 2º DAM  
Proyecto académico del módulo Diseño de interfaces: Componentes Visuales Reutilizables.


