USE [Aula_Virtual_Proyecto]
GO

-- Eliminar el procedimiento si existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spObtenerProyectos]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[spObtenerProyectos]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Crear el procedimiento actualizado con soporte para estudiantes asignados directamente y a través de proyecto_estudiante
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



