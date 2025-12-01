# Resolver Conflictos con GitHub Desktop

## Pasos para Resolver Conflictos

### 1. Abre GitHub Desktop
- Abre la aplicación GitHub Desktop
- Asegúrate de que tu repositorio **Aula_Virtual_UNI** esté seleccionado

### 2. Sincroniza con el Remoto
- En la parte superior, haz clic en **"Fetch origin"** o **"Pull origin"**
- Esto traerá los cambios del repositorio remoto en GitHub

### 3. Si Aparecen Conflictos
GitHub Desktop te mostrará una notificación indicando que hay conflictos que resolver.

### 4. Resolver los Conflictos

**Opción A: Usar el Editor Integrado de GitHub Desktop**
1. Haz clic en el botón **"Resolve conflicts"** o **"Open in Visual Studio Code"** (si tienes VS Code)
2. GitHub Desktop te mostrará una lista de archivos con conflictos
3. Para cada archivo:
   - Haz clic en el archivo para abrirlo
   - Busca las marcas de conflicto:
     ```
     <<<<<<< HEAD
     Tu código local
     =======
     Código del remoto
     >>>>>>> branch-name
     ```
   - Decide qué mantener:
     - **"Accept Current Change"** = Mantener tu versión local
     - **"Accept Incoming Change"** = Mantener la versión remota
     - **"Accept Both Changes"** = Mantener ambas (combinar)
     - O edita manualmente para combinar como quieras
   - Elimina las marcas `<<<<<<<`, `=======`, `>>>>>>>`
   - Guarda el archivo

**Opción B: Resolver Manualmente**
1. Haz clic derecho en el archivo con conflicto
2. Selecciona **"Open in External Editor"** (tu editor de código)
3. Edita el archivo manualmente eliminando las marcas de conflicto
4. Guarda el archivo
5. Vuelve a GitHub Desktop

### 5. Marcar como Resuelto
- Después de editar cada archivo, vuelve a GitHub Desktop
- El archivo debería aparecer como resuelto automáticamente
- Si no, haz clic en **"Mark as resolved"** o marca la casilla junto al archivo

### 6. Hacer Commit
- En la parte inferior izquierda, escribe un mensaje de commit, por ejemplo:
  ```
  Resolver conflictos de merge
  ```
- Haz clic en **"Commit merge"** o **"Commit to main"**

### 7. Subir los Cambios
- Haz clic en **"Push origin"** en la parte superior
- Esto subirá tus cambios resueltos a GitHub

## Si Prefieres Mantener Solo Tus Cambios Locales

Si estás seguro de que quieres descartar todos los cambios remotos y mantener solo los tuyos:

1. En GitHub Desktop, ve a **Branch** → **Update from [rama]**
2. O usa la opción **"Discard all changes from [rama]"**
3. Luego haz **Push** con la opción **"Force push"** (aparece en el menú de opciones del botón Push)

⚠️ **ADVERTENCIA**: Esto eliminará permanentemente los cambios remotos.

## Consejos

- **Antes de resolver**: Revisa qué cambios hay en el remoto para no perder trabajo importante
- **Comunícate**: Si trabajas en equipo, coordina quién resuelve qué conflictos
- **Haz backup**: Si no estás seguro, puedes crear una rama de respaldo antes de resolver

## Ver el Historial de Cambios

- En GitHub Desktop, puedes ver el historial de commits en la pestaña **"History"**
- Esto te ayuda a entender qué cambios se hicieron y cuándo

