# Documentación de Base de Datos - Aula Virtual UNI

## 📊 Estructura de la Base de Datos

### Base de Datos
- **Nombre**: `Aula_Virtual_Proyecto`
- **Motor**: SQL Server
- **Compatibilidad**: SQL Server 2022 (nivel 160)

---

## 📋 Tablas Principales

### 1. **usuarios**
Tabla central que almacena todos los usuarios del sistema (estudiantes, profesores, administradores).

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_usuario` | INT (IDENTITY) | Clave primaria |
| `nombre` | NVARCHAR(20) | Nombre del usuario |
| `apellido1` | NVARCHAR(50) | Primer apellido |
| `apellido2` | NVARCHAR(50) | Segundo apellido (nullable) |
| `cedula` | NVARCHAR(20) | Cédula/Matrícula (UNIQUE) |
| `telefono` | NVARCHAR(20) | Teléfono (nullable) |
| `correo` | NVARCHAR(100) | Email (UNIQUE) |
| `sede` | NVARCHAR(20) | Sede universitaria |
| `contrasena` | NVARCHAR(15) | Contraseña (sin hash) |
| `id_rol` | TINYINT | FK a `roles.id_rol` |

**Índices Únicos:**
- `correo` (UNIQUE)
- `cedula` (UNIQUE)

---

### 2. **roles**
Catálogo de roles del sistema.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_rol` | TINYINT | Clave primaria |
| `nombre` | NVARCHAR(20) | Nombre del rol |

**Valores en la base de datos:**
- `1` = Estudiante
- `2` = Profesor  
- `3` = Administrador

---

### 3. **proyectos**
Proyectos creados por profesores.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_proyecto` | INT (IDENTITY) | Clave primaria |
| `nombre` | NVARCHAR(50) | Nombre del proyecto |
| `descripcion` | NVARCHAR(250) | Descripción (nullable) |
| `fecha_inicio` | DATE | Fecha de inicio |
| `fecha_finalizacion` | DATE | Fecha de finalización |
| `id_profesor` | INT | FK a `usuarios.id_usuario` |
| `curso` | NVARCHAR(50) | Curso asociado (nullable) |
| `estado` | NVARCHAR(50) | Estado del proyecto (nullable) |

---

### 4. **tareas**
Tareas asignadas a estudiantes dentro de proyectos.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_tarea` | INT (IDENTITY) | Clave primaria |
| `id_proyecto` | INT | FK a `proyectos.id_proyecto` |
| `titulo` | NVARCHAR(50) | Título de la tarea |
| `descripcion` | NVARCHAR(250) | Descripción (nullable) |
| `id_asignado` | INT | FK a `usuarios.id_usuario` (estudiante) |
| `fecha_inicio` | DATE | Fecha de inicio |
| `fecha_limite` | DATE | Fecha límite de entrega |
| `estado` | NVARCHAR(20) | Estado (default: 'pendiente') |

**Estados posibles:**
- `pendiente`
- `en progreso`
- `completada`
- `cancelada`

---

### 5. **proyecto_estudiante**
Tabla de relación muchos a muchos entre proyectos y estudiantes.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_proyecto` | INT | FK a `proyectos.id_proyecto` |
| `id_estudiante` | INT | FK a `usuarios.id_usuario` |

**Clave primaria compuesta:** (`id_proyecto`, `id_estudiante`)

---

## 🔗 Relaciones (Foreign Keys)

1. **usuarios.id_rol** → **roles.id_rol**
2. **proyectos.id_profesor** → **usuarios.id_usuario**
3. **tareas.id_proyecto** → **proyectos.id_proyecto** (ON DELETE CASCADE)
4. **tareas.id_asignado** → **usuarios.id_usuario**
5. **proyecto_estudiante.id_proyecto** → **proyectos.id_proyecto** (ON DELETE CASCADE)
6. **proyecto_estudiante.id_estudiante** → **usuarios.id_usuario** (ON DELETE CASCADE)

---

## 📦 Procedimientos Almacenados

### Autenticación y Usuarios

#### `spIniciarSesion`
**Parámetros:**
- `@pNombreUsuario` NVARCHAR(80)
- `@pContrasena` NVARCHAR(50)

**Retorna:**
- `Exito` (BIT/INT)
- `NombreCompleto` (NVARCHAR)
- `id_rol` (TINYINT)
- `id_usuario` (INT)

**Uso en código:** `AulaVirtualDAL/Login.cs`

---

#### `RegistrarUsuario`
**Parámetros:**
- `@Nombre` NVARCHAR(20)
- `@Apellido1` NVARCHAR(50)
- `@Apellido2` NVARCHAR(50) = NULL
- `@Cedula` NVARCHAR(20)
- `@Correo` NVARCHAR(100)
- `@Sede` NVARCHAR(20)
- `@Contrasena` NVARCHAR(15)
- `@RolId` TINYINT

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)
- `IdUsuario` (INT)
- `NombreCompleto` (NVARCHAR)
- `RolId` (TINYINT)

**Uso en código:** `AulaVirtualDAL/RegistroDAL.cs` (nota: el código llama a `spRegistrarUsuario`, pero el SP se llama `RegistrarUsuario`)

---

#### `ObtenerEstudiantes`
Obtiene todos los estudiantes (rol = 1).

**Retorna:**
- `Id` (INT)
- `NombreCompleto` (NVARCHAR)
- `Matricula` (NVARCHAR)
- `Email` (NVARCHAR)

**Uso en código:** `AulaVirtualDAL/UsuariosDal.cs`

---

### Proyectos

#### `spInsertarProyecto`
**Parámetros:**
- `@pNombre` NVARCHAR(50)
- `@pDescripcion` NVARCHAR(250)
- `@pFechainicio` DATE = NULL (usa GETDATE() si es NULL)
- `@pFechaFinalizacion` DATE = NULL
- `@pIdProfesor` INT
- `@pCurso` NVARCHAR(50)
- `@pEstado` NVARCHAR(50)

**Uso en código:** `AulaVirtualDAL/ProyectosDal.cs`

---

#### `spObtenerProyectos`
Obtiene todos los proyectos.

**Retorna:** Todos los campos de la tabla `proyectos`

**Uso en código:** `AulaVirtualDAL/ProyectosDal.cs`

---

#### `ActualizarProyecto`
**Parámetros:**
- `@Id` INT
- `@Nombre` NVARCHAR(50)
- `@Descripcion` NVARCHAR(250)
- `@FechaInicio` DATE
- `@FechaFin` DATE
- `@Curso` NVARCHAR(50)
- `@Estado` NVARCHAR(50)

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Nota:** No se usa actualmente en el código DAL.

---

#### `EliminarProyecto`
**Parámetros:**
- `@Id` INT

**Validaciones:**
- No permite eliminar si hay tareas asociadas

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Uso en código:** `AulaVirtualDAL/ProyectosDal.cs` (nota: el código llama a `spEliminarProyecto`, pero el SP se llama `EliminarProyecto`)

---

#### `AsignarEstudianteProyecto`
Asigna un estudiante a un proyecto.

**Parámetros:**
- `@ProyectoId` INT
- `@EstudianteId` INT

**Validaciones:**
- Verifica si el estudiante ya está asignado

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Nota:** No se usa actualmente en el código DAL.

---

### Tareas

#### `InsertarTarea`
**Parámetros:**
- `@Titulo` NVARCHAR(50)
- `@Descripcion` NVARCHAR(250)
- `@ProyectoId` INT
- `@EstudianteAsignadoId` INT
- `@FechaInicio` DATE = NULL (usa GETDATE() si es NULL)
- `@FechaLimite` DATE
- `@Estado` NVARCHAR(20) = 'pendiente'

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

#### `ObtenerTareas`
Obtiene todas las tareas con información relacionada.

**Retorna:**
- `Id` (INT)
- `Titulo` (NVARCHAR)
- `Descripcion` (NVARCHAR)
- `ProyectoId` (INT)
- `EstudianteAsignadoId` (INT)
- `FechaInicio` (DATE)
- `FechaLimite` (DATE)
- `Estado` (NVARCHAR)
- `NombreAsignado` (NVARCHAR) - JOIN con usuarios
- `Curso` (NVARCHAR) - JOIN con proyectos

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

#### `ActualizarTarea`
**Parámetros:**
- `@Id` INT
- `@Titulo` NVARCHAR(50)
- `@Descripcion` NVARCHAR(250)
- `@FechaLimite` DATE
- `@Estado` NVARCHAR(20)
- `@EstudianteAsignadoId` INT = NULL

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

#### `EliminarTarea`
**Parámetros:**
- `@Id` INT

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

#### `ActualizarEstadoTarea`
Actualiza solo el estado de una tarea.

**Parámetros:**
- `@Id` INT
- `@Estado` NVARCHAR(20)

**Retorna:**
- `Exito` (INT)
- `Mensaje` (NVARCHAR)

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

### Consultas y Reportes

#### `ObtenerEstudiantesProyecto`
Obtiene los estudiantes asignados a un proyecto específico.

**Parámetros:**
- `@ProyectoId` INT

**Retorna:**
- `Id` (INT)
- `NombreCompleto` (NVARCHAR)
- `Matricula` (NVARCHAR)
- `Email` (NVARCHAR)

**Nota:** No se usa actualmente en el código DAL.

---

#### `ReporteEstudiantesProyecto`
Reporte detallado de estudiantes en un proyecto con estadísticas de tareas.

**Parámetros:**
- `@ProyectoId` INT

**Retorna:**
- `Id` (INT)
- `NombreCompleto` (NVARCHAR)
- `Matricula` (NVARCHAR)
- `Email` (NVARCHAR)
- `TotalTareas` (INT)
- `TareasCompletadas` (INT)
- `TareasEnProgreso` (INT)
- `TareasPendientes` (INT)
- `PorcentajeCompletitud` (DECIMAL)

**Nota:** No se usa actualmente en el código DAL.

---

#### `ReportePersonalEstudiante`
Reporte personal de un estudiante con todas sus tareas y proyectos.

**Parámetros:**
- `@EstudianteId` INT

**Retorna:**
- `ProyectoId` (INT)
- `NombreProyecto` (NVARCHAR)
- `Curso` (NVARCHAR)
- `TareaId` (INT)
- `NombreTarea` (NVARCHAR)
- `Descripcion` (NVARCHAR)
- `FechaEntrega` (DATE)
- `Estado` (NVARCHAR)
- `DiasRestantes` (INT)
- `AlertaFecha` (INT) - 0: normal, 1: vencida, 2: por vencer (≤3 días)

**Nota:** No se usa actualmente en el código DAL.

---

#### `ReporteProyectosProfesor`
Reporte de todos los proyectos de un profesor con estadísticas.

**Parámetros:**
- `@ProfesorId` INT

**Retorna:**
- `Id` (INT)
- `Nombre` (NVARCHAR)
- `Descripcion` (NVARCHAR)
- `Curso` (NVARCHAR)
- `FechaInicio` (DATE)
- `FechaFin` (DATE)
- `Estado` (NVARCHAR)
- `TotalEstudiantes` (INT)
- `TotalTareas` (INT)
- `TareasCompletadas` (INT)
- `PorcentajeCompletitud` (DECIMAL)

**Nota:** No se usa actualmente en el código DAL.

---

## 🔄 Mapeo Entidades C# ↔ Base de Datos

### Usuario (Entities/Usuario.cs)
```csharp
Id              → usuarios.id_usuario
Nombre          → usuarios.nombre
Apellido1       → usuarios.apellido1
Apellido2       → usuarios.apellido2
Cedula          → usuarios.cedula
Telefono        → usuarios.telefono
Email           → usuarios.correo
Sede            → usuarios.sede
PasswordHash    → usuarios.contrasena
RolId           → usuarios.id_rol
```

### Proyecto (Entities/Proyecto.cs)
```csharp
Id              → proyectos.id_proyecto
Nombre          → proyectos.nombre
Curso           → proyectos.curso
FechaEntrega    → proyectos.fecha_finalizacion
Descripcion     → proyectos.descripcion
```

### Proyectos (Entities/Proyectos.cs)
```csharp
id_proyecto     → proyectos.id_proyecto
nombre          → proyectos.nombre
descripcion     → proyectos.descripcion
fecha_inicio    → proyectos.fecha_inicio
fecha           → proyectos.fecha_finalizacion
id_profesor     → proyectos.id_profesor
curso           → proyectos.curso
estado          → proyectos.estado
```

### Tarea (Entities/Tarea.cs)
```csharp
Id                      → tareas.id_tarea
ProyectoId              → tareas.id_proyecto
Titulo                  → tareas.titulo
Descripcion             → tareas.descripcion
EstudianteAsignadoId    → tareas.id_asignado
FechaInicio             → tareas.fecha_inicio
FechaLimite             → tareas.fecha_limite
Estado                  → tareas.estado
```

---

## ⚠️ Inconsistencias Detectadas

### 1. Nombres de Procedimientos Almacenados
- **Código llama:** `spRegistrarUsuario` → **BD tiene:** `RegistrarUsuario`
- **Código llama:** `spEliminarProyecto` → **BD tiene:** `EliminarProyecto`
- **Código llama:** `spVerificarMatricula` → **BD no tiene este procedimiento**

### 2. Procedimientos No Utilizados
Los siguientes procedimientos almacenados existen en la BD pero no se usan en el código:
- `ActualizarProyecto`
- `AsignarEstudianteProyecto`
- `ObtenerEstudiantesProyecto`
- `ReporteEstudiantesProyecto`
- `ReportePersonalEstudiante`
- `ReporteProyectosProfesor`

### 3. Seguridad
- Las contraseñas se almacenan en texto plano (`NVARCHAR(15)`) sin hash
- Se recomienda implementar hash de contraseñas (bcrypt, PBKDF2, etc.)

---

## 📝 Notas Adicionales

1. **Cascadas de Eliminación:**
   - Al eliminar un proyecto, se eliminan automáticamente las tareas asociadas (ON DELETE CASCADE)
   - Al eliminar un proyecto, se eliminan las relaciones en `proyecto_estudiante` (ON DELETE CASCADE)

2. **Valores por Defecto:**
   - `tareas.estado` tiene default `'pendiente'`

3. **Validaciones en BD:**
   - Email único en `usuarios`
   - Cédula única en `usuarios`
   - `EliminarProyecto` valida que no haya tareas antes de eliminar

4. **Índices:**
   - Claves primarias tienen índices clustered automáticos
   - Índices únicos en `correo` y `cedula` de `usuarios`

---

## 🔧 Recomendaciones

1. **Corregir nombres de procedimientos** en el código para que coincidan con la BD
2. **Implementar hash de contraseñas** antes de almacenarlas
3. **Utilizar los procedimientos de reportes** para funcionalidades futuras
4. **Agregar procedimiento `spVerificarMatricula`** o modificar el código para usar una consulta directa
5. **Considerar agregar índices** en campos frecuentemente consultados como `tareas.estado`, `proyectos.id_profesor`

