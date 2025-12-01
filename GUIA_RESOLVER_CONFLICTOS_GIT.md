# Guía para Resolver Conflictos de Git en GitHub

## Opción 1: Usar GitHub Desktop (Recomendado - Más Fácil)

Si tienes GitHub Desktop instalado:

1. **Abre GitHub Desktop**
2. **Selecciona tu repositorio** (Aula_Virtual_UNI)
3. **Haz clic en "Fetch origin"** para obtener los cambios remotos
4. **Si hay conflictos**, GitHub Desktop te mostrará qué archivos tienen conflictos
5. **Haz clic en "Merge into current branch"** o **"Rebase current branch"**
6. **Resuelve los conflictos**:
   - Abre los archivos con conflictos
   - Busca las marcas `<<<<<<<`, `=======`, `>>>>>>>`
   - Decide qué código mantener (el tuyo, el remoto, o una combinación)
   - Elimina las marcas de conflicto
7. **Marca los archivos como resueltos** en GitHub Desktop
8. **Haz commit** de los cambios
9. **Push** para subir los cambios

## Opción 2: Usar Git desde la Terminal (Si Git está instalado)

### Paso 1: Verificar el estado
```powershell
git status
```

### Paso 2: Guardar tus cambios locales
```powershell
git add .
git commit -m "Guardar cambios locales antes de merge"
```

### Paso 3: Obtener cambios del remoto
```powershell
git fetch origin
```

### Paso 4: Intentar hacer merge
```powershell
git pull origin main
```
(Reemplaza `main` con el nombre de tu rama si es diferente, podría ser `master`)

### Paso 5: Si hay conflictos, resolverlos

Los archivos con conflictos tendrán marcas como estas:
```
<<<<<<< HEAD
Tu código local
=======
Código del remoto
>>>>>>> branch-name
```

**Para resolver:**
1. Abre cada archivo con conflictos en tu editor
2. Decide qué código mantener:
   - Si quieres **tu versión local**: elimina todo desde `<<<<<<<` hasta `=======` y también elimina `>>>>>>> branch-name`
   - Si quieres **la versión remota**: elimina todo desde `<<<<<<<` hasta `=======` (incluyendo tu código) y también elimina `>>>>>>> branch-name`
   - Si quieres **combinar ambos**: edita manualmente para combinar el código y elimina las marcas

3. Guarda el archivo

### Paso 6: Marcar conflictos como resueltos
```powershell
git add .
git commit -m "Resolver conflictos de merge"
```

### Paso 7: Subir los cambios
```powershell
git push origin main
```

## Opción 3: Forzar tu versión local (⚠️ CUIDADO - Solo si estás seguro)

**ADVERTENCIA**: Esto sobrescribirá todos los cambios remotos con tus cambios locales. Úsalo solo si estás 100% seguro de que quieres descartar los cambios remotos.

```powershell
git push origin main --force
```

## Opción 4: Usar Visual Studio (Si usas VS)

1. Abre Visual Studio
2. Ve a **Team Explorer** o **Git Changes**
3. Haz clic en **Sync** o **Pull**
4. Si hay conflictos, Visual Studio te mostrará una lista
5. Haz clic en cada conflicto y elige:
   - **Keep Current** (tu versión)
   - **Take Incoming** (versión remota)
   - **Merge** (combinar manualmente)
6. Después de resolver todos, haz **Commit** y **Push**

## Consejos para Evitar Conflictos Futuros

1. **Siempre haz `git pull` antes de empezar a trabajar**
2. **Haz commits frecuentes** de tus cambios
3. **Comunícate con tu equipo** si están trabajando en los mismos archivos
4. **Usa ramas separadas** para features grandes

## Si Git no está instalado

1. Descarga Git desde: https://git-scm.com/download/win
2. Instálalo con las opciones por defecto
3. Reinicia PowerShell o tu terminal
4. Verifica la instalación: `git --version`

## Comandos Útiles de Referencia

```powershell
# Ver estado del repositorio
git status

# Ver diferencias
git diff

# Ver historial de commits
git log --oneline

# Ver qué rama estás usando
git branch

# Cambiar de rama
git checkout nombre-rama

# Descartar cambios locales (CUIDADO)
git checkout -- archivo.txt

# Ver configuración remota
git remote -v
```

