# Esquema de Base de Datos - Aula Virtual Proyecto

## 📊 Información General

- **Nombre de la Base de Datos:** `Aula_Virtual_Proyecto`
- **Motor:** SQL Server
- **Nivel de Compatibilidad:** 160 (SQL Server 2022)
- **Usuario de Base de Datos:** `PolloLag_SQLLogin_1` (db_owner)

---

## 📋 Estructura de Tablas

### 1. **roles**
Catálogo de roles del sistema.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id_rol` | TINYINT | Clave primaria |
| `nombre` | NVARCHAR(20) | Nombre del rol |

**Valores en la base de datos:**
- `1` = Estudiante
- `2` = Profesor

**⚠️ IMPORTANTE:** El stored procedure `ObtenerEstudiantes` en la BD filtra incorrectamente por `id_rol = 2` (Profesor). El código usa una consulta directa con `id_rol = 1` (Estudiante) que es el valor correcto según la tabla `roles`.

---

### 2. **usuarios**
Tabla central que almacena todos los usuarios del sistema.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `id_usuario` | INT (IDENTITY) | Clave primaria | NOT NULL |
| `nombre` | NVARCHAR(20) | Nombre del usuario | NOT NULL |
| `apellido1` | NVARCHAR(50) | Primer apellido | NOT NULL |
| `apellido2` | NVARCHAR(50) | Segundo apellido | NULL |
| `cedula` | NVARCHAR(20) | Cédula/Matrícula | NOT NULL, UNIQUE |
| `telefono` | NVARCHAR(20) | Teléfono | NULL |
| `correo` | NVARCHAR(100) | Email | NOT NULL, UNIQUE |
| `sede` | NVARCHAR(20) | Sede universitaria | NOT NULL |
| `contrasena` | NVARCHAR(15) | Contraseña | NOT NULL |
| `id_rol` | TINYINT | FK a `roles.id_rol` | NOT NULL |

**Índices Únicos:**
- `correo` (UNIQUE)
- `cedula` (UNIQUE)

**Foreign Keys:**
- `id_rol` → `roles.id_rol`

---

### 3. **proyectos**
Proyectos creados por profesores.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `id_proyecto` | INT (IDENTITY) | Clave primaria | NOT NULL |
| `nombre` | NVARCHAR(50) | Nombre del proyecto | NOT NULL |
| `descripcion` | NVARCHAR(250) | Descripción | NULL |
| `fecha_inicio` | DATE | Fecha de inicio | NOT NULL |
| `fecha_finalizacion` | DATE | Fecha de finalización | NOT NULL |
| `id_profesor` | INT | FK a `usuarios.id_usuario` | NOT NULL |
| `curso` | NVARCHAR(50) | Curso asociado | NULL |
| `estado` | NVARCHAR(50) | Estado del proyecto | NULL |

**Foreign Keys:**
- `id_profesor` → `usuarios.id_usuario`

---

### 4. **tareas**
Tareas asignadas a estudiantes dentro de proyectos.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `id_tarea` | INT (IDENTITY) | Clave primaria | NOT NULL |
| `id_proyecto` | INT | FK a `proyectos.id_proyecto` | NOT NULL |
| `titulo` | NVARCHAR(50) | Título de la tarea | NOT NULL |
| `descripcion` | NVARCHAR(250) | Descripción | NULL |
| `id_asignado` | INT | FK a `usuarios.id_usuario` (estudiante) | NOT NULL |
| `fecha_inicio` | DATE | Fecha de inicio | NOT NULL |
| `fecha_limite` | DATE | Fecha límite de entrega | NOT NULL |
| `estado` | NVARCHAR(20) | Estado | NULL (default: 'pendiente') |

**Valores por defecto:**
- `estado` = 'pendiente'

**Foreign Keys:**
- `id_proyecto` → `proyectos.id_proyecto` (ON DELETE CASCADE)
- `id_asignado` → `usuarios.id_usuario`

**Estados posibles:**
- `pendiente`
- `en progreso`
- `completada`
- `cancelada`

---

### 5. **proyecto_estudiante**
Tabla de relación muchos a muchos entre proyectos y estudiantes.

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `id_proyecto` | INT | FK a `proyectos.id_proyecto` | NOT NULL |
| `id_estudiante` | INT | FK a `usuarios.id_usuario` | NOT NULL |

**Clave primaria compuesta:** (`id_proyecto`, `id_estudiante`)

**Foreign Keys:**
- `id_proyecto` → `proyectos.id_proyecto` (ON DELETE CASCADE)
- `id_estudiante` → `usuarios.id_usuario` (ON DELETE CASCADE)

---

### 6. **tarea_estudiante**
Tabla de relación muchos a muchos entre tareas y estudiantes (asignaciones adicionales).

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `id_tarea` | INT | FK a `tareas.id_tarea` | NOT NULL |
| `id_estudiante` | INT | FK a `usuarios.id_usuario` | NOT NULL |
| `fecha_asignacion` | DATETIME | Fecha de asignación | NULL (default: GETDATE()) |

**Clave primaria compuesta:** (`id_tarea`, `id_estudiante`)

**Foreign Keys:**
- `id_tarea` → `tareas.id_tarea` (ON DELETE CASCADE)
- `id_estudiante` → `usuarios.id_usuario` (ON DELETE CASCADE)

**Valores por defecto:**
- `fecha_asignacion` = GETDATE()

---

## 📦 Procedimientos Almacenados

### Autenticación y Usuarios

#### `spIniciarSesion`
**Parámetros:**
- `@pNombreUsuario` NVARCHAR(80)
- `@pContrasena` NVARCHAR(50)

**Retorna:**
- `Exito` (INT) - 1 si éxito, 0 si falla
- `NombreCompleto` (NVARCHAR) - Nombre completo del usuario
- `id_rol` (TINYINT) - ID del rol
- `id_usuario` (INT) - ID del usuario

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
- `Exito` (INT) - 1 si éxito, 0 si falla
- `Mensaje` (NVARCHAR)
- `IdUsuario` (INT) - Si éxito
- `NombreCompleto` (NVARCHAR) - Si éxito
- `RolId` (TINYINT) - Si éxito

**Nota:** El código actual en `RegistroDAL.cs` llama a `spRegistrarUsuario`, pero el procedimiento almacenado se llama `RegistrarUsuario` y tiene parámetros diferentes.

---

#### `ObtenerEstudiantes`
**Parámetros:** Ninguno

**Retorna:**
- `Id` (INT) - id_usuario
- `NombreCompleto` (NVARCHAR) - Nombre completo concatenado
- `Matricula` (NVARCHAR) - Cédula
- `Email` (NVARCHAR) - Correo

**Filtro:** `WHERE u.id_rol = 2` (⚠️ INCORRECTO - filtra Profesores, no Estudiantes)

**⚠️ PROBLEMA:** Este stored procedure está mal configurado. Filtra por `id_rol = 2` que es Profesor, no Estudiante.

**Uso en código:** `AulaVirtualDAL/UsuariosDal.cs` usa una consulta directa con `id_rol = 1` (correcto) en lugar de este SP.

---

#### `ObtenerEstudiantesProyecto`
**Parámetros:**
- `@ProyectoId` INT

**Retorna:**
- `Id` (INT)
- `NombreCompleto` (NVARCHAR)
- `Matricula` (NVARCHAR)
- `Email` (NVARCHAR)

**Filtro:** Estudiantes asignados al proyecto con `id_rol = 2` (⚠️ Verificar si este SP también tiene el mismo problema)

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

**Nota:** El procedimiento usa `GETDATE()` para `fecha_inicio` si `@pFechainicio` es NULL.

---

#### `spObtenerProyectos`
**Parámetros:** Ninguno

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

**Nota:** No se usa actualmente en el código.

---

#### `EliminarProyecto`
**Parámetros:**
- `@Id` INT

**Retorna:**
- `Exito` (INT) - 0 si hay tareas asociadas, 1 si se elimina
- `Mensaje` (NVARCHAR)

**Validación:** No permite eliminar si hay tareas asociadas.

**Uso en código:** `AulaVirtualDAL/ProyectosDal.cs`

---

#### `AsignarEstudianteProyecto`
**Parámetros:**
- `@ProyectoId` INT
- `@EstudianteId` INT

**Retorna:**
- `Exito` (INT) - 0 si ya está asignado, 1 si se asigna
- `Mensaje` (NVARCHAR)

**Validación:** Verifica si el estudiante ya está asignado.

**Uso en código:** `AulaVirtualDAL/ProyectosDal.cs`

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

**Retorna:**
- `Id` (INT) - SCOPE_IDENTITY() del nuevo registro

**Uso en código:** `AulaVirtualDAL/TareasDal.cs`

---

#### `ObtenerTareas`
**Parámetros:** Ninguno

**Retorna:**
- `Id` (INT) - id_tarea
- `Titulo` (NVARCHAR)
- `Descripcion` (NVARCHAR)
- `ProyectoId` (INT)
- `EstudianteAsignadoId` (INT)
- `FechaInicio` (DATE)
- `FechaLimite` (DATE)
- `Estado` (NVARCHAR)
- `NombreAsignado` (NVARCHAR) - Nombre del estudiante asignado
- `Curso` (NVARCHAR) - Curso del proyecto

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

#### `ActualizarEstadoTarea`
**Parámetros:**
- `@Id` INT
- `@Estado` NVARCHAR(20)

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

#### `AsignarEstudianteATarea`
**Parámetros:**
- `@TareaId` INT
- `@EstudianteId` INT

**Funcionalidad:** Inserta en `tarea_estudiante` si no existe la asignación.

**Nota:** No se usa actualmente en el código.

---

#### `ObtenerEstudiantesPorTarea`
**Parámetros:**
- `@TareaId` INT

**Retorna:**
- `Id` (INT)
- `NombreCompleto` (NVARCHAR)
- `Matricula` (NVARCHAR)
- `Email` (NVARCHAR)

**Nota:** No se usa actualmente en el código.

---

#### `EliminarAsignacionesEstudiantes`
**Parámetros:**
- `@TareaId` INT

**Funcionalidad:** Elimina todas las asignaciones de estudiantes a una tarea en `tarea_estudiante`.

**Nota:** No se usa actualmente en el código.

---

### Reportes

#### `ReporteEstudiantesProyecto`
**Parámetros:**
- `@ProyectoId` INT

**Retorna:** Información detallada de estudiantes con estadísticas de tareas.

**Nota:** No se usa actualmente en el código.

---

#### `ReportePersonalEstudiante`
**Parámetros:**
- `@EstudianteId` INT

**Retorna:** Información de proyectos y tareas del estudiante con alertas de fechas.

**Nota:** No se usa actualmente en el código.

---

#### `ReporteProyectosProfesor`
**Parámetros:**
- `@ProfesorId` INT

**Retorna:** Información de proyectos del profesor con estadísticas.

**Nota:** No se usa actualmente en el código.

---

## 🔄 Mapeo de Entidades C# a Base de Datos

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

### Proyectos (Entities/Proyectos.cs)
```csharp
id_proyecto         → proyectos.id_proyecto
nombre              → proyectos.nombre
descripcion         → proyectos.descripcion
fecha_inicio        → proyectos.fecha_inicio
fecha_finalizacion  → proyectos.fecha_finalizacion (CORREGIDO)
id_profesor         → proyectos.id_profesor
curso               → proyectos.curso
estado              → proyectos.estado
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

## ⚠️ Correcciones Realizadas

### 1. UsuariosDAL.cs
- **Antes:** Usaba el stored procedure `ObtenerEstudiantes` que filtra incorrectamente por `id_rol = 2` (Profesor)
- **Después:** Usa consulta directa con `id_rol = 1` (Estudiante) que es el valor correcto según la tabla `roles`

### 2. Proyectos.cs
- **Antes:** Propiedad `fecha` 
- **Después:** Propiedad `fecha_finalizacion` para coincidir con la BD

### 3. ProyectosDal.cs
- **Actualizado:** Referencias a `Proyecto.fecha` cambiadas a `Proyecto.fecha_finalizacion`

### 4. V_Proyectos.cshtml
- **Actualizado:** Referencia a `Proyecto.fecha` cambiada a `Proyecto.fecha_finalizacion`

---

## 📝 Notas Importantes

1. **Roles:** 
   - `id_rol = 1` = Estudiante
   - `id_rol = 2` = Profesor
   - ⚠️ El stored procedure `ObtenerEstudiantes` está mal configurado (filtra por `id_rol = 2` que es Profesor). El código usa `id_rol = 1` directamente.

2. **RegistroDAL.cs:** Actualmente llama a `spRegistrarUsuario`, pero el procedimiento almacenado se llama `RegistrarUsuario` y tiene parámetros diferentes. Se requiere corrección.

3. **Seguridad:** Las contraseñas se almacenan en texto plano (`NVARCHAR(15)`) sin hash. Se recomienda implementar hash de contraseñas.

4. **Cascadas de Eliminación:**
   - Eliminar un proyecto elimina automáticamente las relaciones en `proyecto_estudiante` y `tareas`
   - Eliminar una tarea elimina automáticamente las relaciones en `tarea_estudiante`

---

## 🔗 Cadena de Conexión

La cadena de conexión se encuentra en `appsettings.json`:
```json
"ConexionDB": "workstation id=Aula_Virtual_Proyecto.mssql.somee.com;packet size=4096;user id=PolloLag_SQLLogin_1;pwd=zh68rbd1i1;data source=Aula_Virtual_Proyecto.mssql.somee.com;persist security info=False;initial catalog=Aula_Virtual_Proyecto;TrustServerCertificate=True"
```

---

**Última actualización:** Basado en el script SQL proporcionado el 4/12/2025

