USE [master]
GO
/****** Object:  Database [Aula_Virtual_Proyecto]    Script Date: 6/12/2025 12:47:06 ******/
CREATE DATABASE [Aula_Virtual_Proyecto]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Aula_Virtual_Proyecto_Data', FILENAME = N'c:\dzsqls\Aula_Virtual_Proyecto.mdf' , SIZE = 8192KB , MAXSIZE = 30720KB , FILEGROWTH = 22528KB )
 LOG ON 
( NAME = N'Aula_Virtual_Proyecto_Logs', FILENAME = N'c:\dzsqls\Aula_Virtual_Proyecto.ldf' , SIZE = 8192KB , MAXSIZE = 30720KB , FILEGROWTH = 22528KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Aula_Virtual_Proyecto].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ARITHABORT OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET  MULTI_USER 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET QUERY_STORE = ON
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Aula_Virtual_Proyecto]
GO
/****** Object:  User [PolloLag_SQLLogin_1]    Script Date: 6/12/2025 12:47:07 ******/
CREATE USER [PolloLag_SQLLogin_1] FOR LOGIN [PolloLag_SQLLogin_1] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [PolloLag_SQLLogin_1]
GO
/****** Object:  Schema [PolloLag_SQLLogin_1]    Script Date: 6/12/2025 12:47:08 ******/
CREATE SCHEMA [PolloLag_SQLLogin_1]
GO
/****** Object:  Table [dbo].[proyecto_estudiante]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[proyecto_estudiante](
	[id_proyecto] [int] NOT NULL,
	[id_estudiante] [int] NOT NULL,
	[fecha_asignacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_proyecto] ASC,
	[id_estudiante] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[proyectos]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[proyectos](
	[id_proyecto] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](50) NOT NULL,
	[descripcion] [nvarchar](250) NULL,
	[fecha_inicio] [date] NOT NULL,
	[fecha_finalizacion] [date] NOT NULL,
	[id_profesor] [int] NOT NULL,
	[curso] [nvarchar](50) NULL,
	[estado] [nvarchar](50) NULL,
	[id_asignado] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_proyecto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id_rol] [tinyint] NOT NULL,
	[nombre] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tarea_estudiante]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tarea_estudiante](
	[id_tarea] [int] NOT NULL,
	[id_estudiante] [int] NOT NULL,
	[fecha_asignacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tarea] ASC,
	[id_estudiante] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tareas]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tareas](
	[id_tarea] [int] IDENTITY(1,1) NOT NULL,
	[id_proyecto] [int] NOT NULL,
	[titulo] [nvarchar](50) NOT NULL,
	[descripcion] [nvarchar](250) NULL,
	[id_asignado] [int] NOT NULL,
	[fecha_inicio] [date] NOT NULL,
	[fecha_limite] [date] NOT NULL,
	[estado] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tarea] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usuarios]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usuarios](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](20) NOT NULL,
	[apellido1] [nvarchar](50) NOT NULL,
	[apellido2] [nvarchar](50) NULL,
	[cedula] [nvarchar](20) NOT NULL,
	[telefono] [nvarchar](20) NULL,
	[correo] [nvarchar](100) NOT NULL,
	[sede] [nvarchar](20) NOT NULL,
	[contrasena] [nvarchar](15) NOT NULL,
	[id_rol] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[correo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tarea_estudiante] ADD  DEFAULT (getdate()) FOR [fecha_asignacion]
GO
ALTER TABLE [dbo].[tareas] ADD  DEFAULT ('pendiente') FOR [estado]
GO
ALTER TABLE [dbo].[proyecto_estudiante]  WITH CHECK ADD FOREIGN KEY([id_estudiante])
REFERENCES [dbo].[usuarios] ([id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[proyecto_estudiante]  WITH CHECK ADD FOREIGN KEY([id_proyecto])
REFERENCES [dbo].[proyectos] ([id_proyecto])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[proyectos]  WITH CHECK ADD FOREIGN KEY([id_profesor])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[tarea_estudiante]  WITH CHECK ADD FOREIGN KEY([id_estudiante])
REFERENCES [dbo].[usuarios] ([id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tarea_estudiante]  WITH CHECK ADD FOREIGN KEY([id_tarea])
REFERENCES [dbo].[tareas] ([id_tarea])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tareas]  WITH CHECK ADD FOREIGN KEY([id_asignado])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[tareas]  WITH CHECK ADD FOREIGN KEY([id_proyecto])
REFERENCES [dbo].[proyectos] ([id_proyecto])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[usuarios]  WITH CHECK ADD FOREIGN KEY([id_rol])
REFERENCES [dbo].[roles] ([id_rol])
GO
/****** Object:  StoredProcedure [dbo].[ActualizarEstadoTarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ActualizarEstadoTarea]
    @Id INT,
    @Estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tareas
    SET estado = @Estado
    WHERE id_tarea = @Id;

    SELECT 1 AS Exito, 'Estado actualizado exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[ActualizarProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ActualizarProyecto]
    @Id INT,
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(250),
    @FechaInicio DATE,
    @FechaFin DATE,
    @Curso NVARCHAR(50),
    @Estado NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.proyectos
    SET nombre = @Nombre,
        descripcion = @Descripcion,
        fecha_inicio = @FechaInicio,
        fecha_finalizacion = @FechaFin,
        curso = @Curso,
        estado = @Estado
    WHERE id_proyecto = @Id;

    SELECT 1 AS Exito, 'Proyecto actualizado exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[ActualizarTarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ActualizarTarea]
    @Id INT,
    @Titulo NVARCHAR(50),
    @Descripcion NVARCHAR(250),
    @FechaLimite DATE,
    @Estado NVARCHAR(20),
    @EstudianteAsignadoId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tareas
    SET titulo       = @Titulo,
        descripcion  = @Descripcion,
        fecha_limite = @FechaLimite,
        estado       = @Estado,
        id_asignado  = ISNULL(@EstudianteAsignadoId, id_asignado)
    WHERE id_tarea = @Id;

    SELECT 1 AS Exito, 'Tarea actualizada exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[AsignarEstudianteATarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 2. PROCEDIMIENTOS ALMACENADOS

-- Procedimiento para asignar un estudiante a una tarea
CREATE   PROCEDURE [dbo].[AsignarEstudianteATarea]
    @TareaId INT,
    @EstudianteId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si la asignación ya existe para evitar duplicados
    IF NOT EXISTS (SELECT 1 FROM tarea_estudiante WHERE id_tarea = @TareaId AND id_estudiante = @EstudianteId)
    BEGIN
        INSERT INTO tarea_estudiante (id_tarea, id_estudiante)
        VALUES (@TareaId, @EstudianteId);
    END
END
GO
/****** Object:  StoredProcedure [dbo].[AsignarEstudianteProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AsignarEstudianteProyecto]
    @ProyectoId INT,
    @EstudianteId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM dbo.proyecto_estudiante 
        WHERE id_proyecto = @ProyectoId AND id_estudiante = @EstudianteId
    )
    BEGIN
        SELECT 0 AS Exito, 'El estudiante ya está asignado a este proyecto' AS Mensaje;
        RETURN;
    END

    INSERT INTO dbo.proyecto_estudiante (id_proyecto, id_estudiante)
    VALUES (@ProyectoId, @EstudianteId);

    SELECT 1 AS Exito, 'Estudiante asignado exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[EliminarAsignacionesEstudiantes]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para eliminar todas las asignaciones de una tarea
CREATE   PROCEDURE [dbo].[EliminarAsignacionesEstudiantes]
    @TareaId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM tarea_estudiante
    WHERE id_tarea = @TareaId;
END
GO
/****** Object:  StoredProcedure [dbo].[EliminarProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EliminarProyecto]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar si hay tareas asociadas
    IF EXISTS (SELECT 1 FROM dbo.tareas WHERE id_proyecto = @Id)
    BEGIN
        SELECT 0 AS Exito,
               'No se puede eliminar el proyecto porque tiene tareas asociadas' AS Mensaje;
        RETURN;
    END

    -- Por FKs ya tienes ON DELETE CASCADE, pero lo dejo explícito por claridad
    DELETE FROM dbo.proyecto_estudiante WHERE id_proyecto = @Id;
    DELETE FROM dbo.proyectos WHERE id_proyecto = @Id;

    SELECT 1 AS Exito, 'Proyecto eliminado exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[EliminarTarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EliminarTarea]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.tareas WHERE id_tarea = @Id;

    SELECT 1 AS Exito, 'Tarea eliminada exitosamente' AS Mensaje;
END
GO
/****** Object:  StoredProcedure [dbo].[InsertarTarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertarTarea]
    @Titulo NVARCHAR(50),
    @Descripcion NVARCHAR(250),
    @ProyectoId INT,
    @EstudianteAsignadoId INT,
    @FechaInicio DATE = NULL,
    @FechaLimite DATE,
    @Estado NVARCHAR(20) = 'pendiente'
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tareas
        (id_proyecto, titulo, descripcion, id_asignado, fecha_inicio, fecha_limite, estado)
    VALUES
        (@ProyectoId,
         @Titulo,
         @Descripcion,
         @EstudianteAsignadoId,
         ISNULL(@FechaInicio, CAST(GETDATE() AS DATE)),
         @FechaLimite,
         @Estado);
         
    SELECT SCOPE_IDENTITY() AS Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[ObtenerEstudiantes]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerEstudiantes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id_usuario AS Id,
        CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
        u.cedula     AS Matricula,
        u.correo     AS Email
    FROM dbo.usuarios u
    WHERE u.id_rol = 2
    ORDER BY u.nombre, u.apellido1;
END
GO
/****** Object:  StoredProcedure [dbo].[ObtenerEstudiantesPorTarea]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para obtener estudiantes asignados a una tarea
CREATE   PROCEDURE [dbo].[ObtenerEstudiantesPorTarea]
    @TareaId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id_usuario AS Id,
        CONCAT(u.nombre, ' ', u.apellido1, COALESCE(' ' + u.apellido2, '')) AS NombreCompleto,
        u.cedula AS Matricula,
        u.correo AS Email
    FROM tarea_estudiante te
    INNER JOIN usuarios u ON te.id_estudiante = u.id_usuario
    WHERE te.id_tarea = @TareaId
    ORDER BY u.nombre, u.apellido1;
END
GO
/****** Object:  StoredProcedure [dbo].[ObtenerEstudiantesProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerEstudiantesProyecto]
    @ProyectoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id_usuario AS Id,
        CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
        u.cedula     AS Matricula,
        u.correo     AS Email
    FROM dbo.proyecto_estudiante pe
    INNER JOIN dbo.usuarios u ON pe.id_estudiante = u.id_usuario
    WHERE pe.id_proyecto = @ProyectoId
      AND u.id_rol = 2   -- 2 = Estudiante (ajusta si tu catálogo de roles es distinto)
    ORDER BY u.apellido1, u.nombre;
END
GO
/****** Object:  StoredProcedure [dbo].[ObtenerTareas]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerTareas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.id_tarea                  AS Id,
        t.titulo                    AS Titulo,
        t.descripcion               AS Descripcion,
        t.id_proyecto               AS ProyectoId,
        t.id_asignado               AS EstudianteAsignadoId,
        t.fecha_inicio              AS FechaInicio,
        t.fecha_limite              AS FechaLimite,
        t.estado                    AS Estado,
        CONCAT(u.nombre, ' ', u.apellido1) AS NombreAsignado,
        p.curso                     AS Curso
    FROM dbo.tareas t
    INNER JOIN dbo.usuarios u ON t.id_asignado = u.id_usuario
    INNER JOIN dbo.proyectos p ON t.id_proyecto = p.id_proyecto
    ORDER BY t.fecha_limite ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[RegistrarUsuario]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RegistrarUsuario]
    @Nombre NVARCHAR(20),
    @Apellido1 NVARCHAR(50),
    @Apellido2 NVARCHAR(50) = NULL,
    @Cedula NVARCHAR(20),
    @Correo NVARCHAR(100),
    @Sede NVARCHAR(20),
    @Contrasena NVARCHAR(15),
    @RolId TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.usuarios WHERE correo = @Correo OR cedula = @Cedula)
    BEGIN
        SELECT 0 AS Exito, 'El usuario ya existe' AS Mensaje;
        RETURN;
    END

    INSERT INTO dbo.usuarios
        (nombre, apellido1, apellido2, cedula, telefono, correo, sede, contrasena, id_rol)
    VALUES
        (@Nombre, @Apellido1, @Apellido2, @Cedula, NULL, @Correo, @Sede, @Contrasena, @RolId);

    DECLARE @Id INT = SCOPE_IDENTITY();

    SELECT 1 AS Exito,
           'Usuario registrado exitosamente' AS Mensaje,
           @Id AS IdUsuario,
           CONCAT(@Nombre, ' ', @Apellido1) AS NombreCompleto,
           @RolId AS RolId;
END
GO
/****** Object:  StoredProcedure [dbo].[ReporteEstudiantesProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ReporteEstudiantesProyecto]
    @ProyectoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id_usuario AS Id,
        CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto,
        u.cedula AS Matricula,
        u.correo AS Email,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = @ProyectoId 
           AND id_asignado = u.id_usuario) AS TotalTareas,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = @ProyectoId 
           AND id_asignado = u.id_usuario 
           AND estado = 'completada') AS TareasCompletadas,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = @ProyectoId 
           AND id_asignado = u.id_usuario 
           AND estado = 'en progreso') AS TareasEnProgreso,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = @ProyectoId 
           AND id_asignado = u.id_usuario 
           AND estado = 'pendiente') AS TareasPendientes,
        CASE 
            WHEN (SELECT COUNT(*) 
                  FROM dbo.tareas 
                  WHERE id_proyecto = @ProyectoId 
                    AND id_asignado = u.id_usuario) = 0 THEN 0
            ELSE CAST(
                    (SELECT COUNT(*) 
                     FROM dbo.tareas 
                     WHERE id_proyecto = @ProyectoId 
                       AND id_asignado = u.id_usuario 
                       AND estado = 'completada') 
                 AS DECIMAL(5,2))
                 /
                 CAST(
                    (SELECT COUNT(*) 
                     FROM dbo.tareas 
                     WHERE id_proyecto = @ProyectoId 
                       AND id_asignado = u.id_usuario) 
                 AS DECIMAL(5,2)) * 100
        END AS PorcentajeCompletitud
    FROM dbo.proyecto_estudiante pe
    INNER JOIN dbo.usuarios u ON pe.id_estudiante = u.id_usuario
    WHERE pe.id_proyecto = @ProyectoId
    ORDER BY u.apellido1, u.nombre;
END
GO
/****** Object:  StoredProcedure [dbo].[ReportePersonalEstudiante]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ReportePersonalEstudiante]
    @EstudianteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.id_proyecto AS ProyectoId,
        p.nombre      AS NombreProyecto,
        p.curso       AS Curso,
        t.id_tarea    AS TareaId,
        t.titulo      AS NombreTarea,
        t.descripcion AS Descripcion,
        t.fecha_limite AS FechaEntrega,
        t.estado      AS Estado,
        DATEDIFF(DAY, GETDATE(), t.fecha_limite) AS DiasRestantes,
        CASE 
            WHEN t.estado = 'completada' THEN 0
            WHEN DATEDIFF(DAY, GETDATE(), t.fecha_limite) < 0 THEN 1
            WHEN DATEDIFF(DAY, GETDATE(), t.fecha_limite) <= 3 THEN 2
            ELSE 0
        END AS AlertaFecha
    FROM dbo.proyecto_estudiante pe
    INNER JOIN dbo.proyectos p ON pe.id_proyecto = p.id_proyecto
    LEFT JOIN dbo.tareas t 
        ON p.id_proyecto = t.id_proyecto 
       AND t.id_asignado = @EstudianteId
    WHERE pe.id_estudiante = @EstudianteId
    ORDER BY p.fecha_inicio DESC, t.fecha_limite ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[ReporteProyectosProfesor]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ReporteProyectosProfesor]
    @ProfesorId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.id_proyecto AS Id,
        p.nombre      AS Nombre,
        p.descripcion AS Descripcion,
        p.curso       AS Curso,
        p.fecha_inicio AS FechaInicio,
        p.fecha_finalizacion AS FechaFin,
        p.estado      AS Estado,
        (SELECT COUNT(*) 
         FROM dbo.proyecto_estudiante 
         WHERE id_proyecto = p.id_proyecto) AS TotalEstudiantes,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = p.id_proyecto) AS TotalTareas,
        (SELECT COUNT(*) 
         FROM dbo.tareas 
         WHERE id_proyecto = p.id_proyecto AND estado = 'completada') AS TareasCompletadas,
        CASE 
            WHEN (SELECT COUNT(*) FROM dbo.tareas WHERE id_proyecto = p.id_proyecto) = 0 THEN 0
            ELSE CAST(
                    (SELECT COUNT(*) 
                     FROM dbo.tareas 
                     WHERE id_proyecto = p.id_proyecto AND estado = 'completada') 
                 AS DECIMAL(5,2)) 
                 / 
                 CAST(
                    (SELECT COUNT(*) 
                     FROM dbo.tareas 
                     WHERE id_proyecto = p.id_proyecto) 
                 AS DECIMAL(5,2)) * 100
        END AS PorcentajeCompletitud
    FROM dbo.proyectos p
    WHERE p.id_profesor = @ProfesorId
    ORDER BY p.fecha_inicio DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[spIniciarSesion]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ==========================================================
-- PROCEDIMIENTO ALMACENADO: spIniciarSesion
-- ==========================================================
CREATE PROCEDURE [dbo].[spIniciarSesion]
    @pNombreUsuario NVARCHAR(80),
    @pContrasena NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE 
            WHEN nombre IS NOT NULL THEN 1 
            ELSE 0 
        END AS Exito,
        CONCAT(nombre, ' ', apellido1, ' ', apellido2) AS NombreCompleto,
        id_rol,
        id_usuario
    FROM Usuarios
    WHERE nombre = @pNombreUsuario COLLATE LATIN1_GENERAL_CS_AS
      AND contrasena = @pContrasena COLLATE LATIN1_GENERAL_CS_AS;
END
GO
/****** Object:  StoredProcedure [dbo].[spInsertarProyecto]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spInsertarProyecto]
    @pNombre NVARCHAR(50),
    @pDescripcion NVARCHAR(250),
    @pFechainicio DATE = NULL,
    @pFechaFinalizacion DATE = NULL,
    @pIdProfesor INT,
    @pCurso NVARCHAR(50),
    @pEstado NVARCHAR(50),
    @EstudianteAsignadoId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO proyectos (
        nombre, 
        descripcion, 
        fecha_inicio, 
        fecha_finalizacion, 
        id_profesor, 
        curso, 
        estado,
        id_asignado  
    )
    VALUES (
        @pNombre,
        @pDescripcion, 
        ISNULL( @pFechainicio, CAST(GETDATE() AS DATE)), 
        @pFechaFinalizacion, 
        @pIdProfesor, 
        @pCurso, 
        @pEstado,
        @EstudianteAsignadoId  
    );

    SELECT SCOPE_IDENTITY() AS Id;
END;
GO
/****** Object:  StoredProcedure [dbo].[spObtenerProyectos]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Crear el procedimiento con el nombre del estudiante
CREATE PROCEDURE [dbo].[spObtenerProyectos]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- CTE para combinar estudiantes asignados directamente (id_asignado) y a través de proyecto_estudiante
    WITH EstudiantesAsignados AS (
        -- Estudiantes asignados directamente en proyectos.id_asignado
        SELECT 
            p.id_proyecto,
            u.id_usuario AS id_estudiante,
            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto
        FROM dbo.proyectos p
        INNER JOIN dbo.usuarios u ON p.id_asignado = u.id_usuario
        WHERE p.id_asignado IS NOT NULL
        
        UNION
        
        -- Estudiantes asignados a través de proyecto_estudiante
        SELECT 
            pe.id_proyecto,
            u.id_usuario AS id_estudiante,
            CONCAT(u.nombre, ' ', u.apellido1, ' ', ISNULL(u.apellido2, '')) AS NombreCompleto
        FROM dbo.proyecto_estudiante pe
        INNER JOIN dbo.usuarios u ON pe.id_estudiante = u.id_usuario
    )
    SELECT 
        p.id_proyecto,
        p.nombre,
        p.descripcion,
        p.fecha_inicio,
        p.fecha_finalizacion,
        p.id_profesor,
        p.curso,
        p.estado,
        LTRIM(RTRIM(STRING_AGG(
            ea.NombreCompleto, 
            ', '
        ) WITHIN GROUP (ORDER BY ea.NombreCompleto))) AS NombreAsignado
    FROM dbo.proyectos p
    LEFT JOIN EstudiantesAsignados ea ON p.id_proyecto = ea.id_proyecto
    GROUP BY 
        p.id_proyecto,
        p.nombre,
        p.descripcion,
        p.fecha_inicio,
        p.fecha_finalizacion,
        p.id_profesor,
        p.curso,
        p.estado
    ORDER BY p.fecha_finalizacion DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[spRegistrarUsuario]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Stored Procedure: spRegistrarUsuario
-- Descripción: Registra un nuevo usuario en el sistema
-- =============================================
CREATE   PROCEDURE [dbo].[spRegistrarUsuario]
    @pNombre NVARCHAR(100),
    @pApellido1 NVARCHAR(100),
    @pApellido2 NVARCHAR(100),
    @pCedula NVARCHAR(20),
    @pTelefono NVARCHAR(20),
    @pCorreo NVARCHAR(150),
    @pSede NVARCHAR(150),
    @pContrasena NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Exito INT = 0;
    DECLARE @Mensaje NVARCHAR(255) = '';
    DECLARE @IdUsuario INT;
    DECLARE @NombreCompleto NVARCHAR(255);
    DECLARE @IdRol TINYINT = 1; -- Por defecto, rol de Estudiante
    
    BEGIN TRY
        -- Verificar si la cédula ya existe
        IF EXISTS (SELECT 1 FROM usuarios WHERE cedula = @pCedula)
        BEGIN
            SET @Exito = 0;
            SET @Mensaje = 'La cédula ya está registrada.';
            SELECT @Exito AS Exito, @Mensaje AS Mensaje, NULL AS id_usuario, NULL AS NombreCompleto, NULL AS id_rol;
            RETURN;
        END
        
        -- Verificar si el correo ya existe
        IF EXISTS (SELECT 1 FROM usuarios WHERE correo = @pCorreo)
        BEGIN
            SET @Exito = 0;
            SET @Mensaje = 'El correo electrónico ya está registrado.';
            SELECT @Exito AS Exito, @Mensaje AS Mensaje, NULL AS id_usuario, NULL AS NombreCompleto, NULL AS id_rol;
            RETURN;
        END
        
        -- Insertar el nuevo usuario
        INSERT INTO usuarios (nombre, apellido1, apellido2, cedula, telefono, correo, sede, contrasena, id_rol)
        VALUES (@pNombre, @pApellido1, @pApellido2, @pCedula, @pTelefono, @pCorreo, @pSede, @pContrasena, @IdRol);
        
        -- Obtener el ID del usuario recién insertado
        SET @IdUsuario = SCOPE_IDENTITY();
        
        -- Construir el nombre completo
        SET @NombreCompleto = @pNombre + ' ' + @pApellido1 + ' ' + @pApellido2;
        
        SET @Exito = 1;
        SET @Mensaje = 'Usuario registrado exitosamente.';
        
        -- Devolver los resultados
        SELECT @Exito AS Exito, @Mensaje AS Mensaje, @IdUsuario AS id_usuario, @NombreCompleto AS NombreCompleto, @IdRol AS id_rol;
        
    END TRY
    BEGIN CATCH
        SET @Exito = 0;
        SET @Mensaje = 'Error al registrar el usuario: ' + ERROR_MESSAGE();
        SELECT @Exito AS Exito, @Mensaje AS Mensaje, NULL AS id_usuario, NULL AS NombreCompleto, NULL AS id_rol;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[spVerificarCedula]    Script Date: 6/12/2025 12:47:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Stored Procedure: spVerificarCedula
-- Descripción: Verifica si una cédula ya existe en el sistema
-- =============================================
CREATE   PROCEDURE [dbo].[spVerificarCedula]
    @pCedula NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Existe INT = 0;
    
    IF EXISTS (SELECT 1 FROM usuarios WHERE cedula = @pCedula)
    BEGIN
        SET @Existe = 1;
    END
    
    SELECT @Existe AS Existe;
END
GO
USE [master]
GO
ALTER DATABASE [Aula_Virtual_Proyecto] SET  READ_WRITE 
GO
