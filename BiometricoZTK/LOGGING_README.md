# Sistema de Logging - BiometricoZTK

## Descripción

Este proyecto implementa un sistema completo de logging utilizando **Serilog** con un middleware personalizado para capturar y registrar todos los errores de la aplicación.

## Características

### 1. Middleware de Captura de Errores (`ErrorLoggingMiddleware`)

El middleware captura automáticamente todas las excepciones no controladas y registra información detallada:

- **ID único del error**: Para rastreo y seguimiento
- **Fecha y hora**: Timestamp preciso del error
- **Información de la petición HTTP**:
  - Método HTTP (GET, POST, etc.)
  - Ruta solicitada
  - Query string
  - IP del cliente
  - User Agent
- **Detalles del error**:
  - Tipo de excepción
  - Mensaje de error
  - Stack trace completo
  - Inner exceptions (si existen)

### 2. Configuración de Serilog

El sistema está configurado para escribir logs en múltiples destinos:

#### a) Consola
- Muestra logs en tiempo real durante el desarrollo
- Formato: `[HH:mm:ss LEVEL] Mensaje`

#### b) Archivo de Logs Generales (`logger/log-YYYYMMDD.txt`)
- **Nivel mínimo**: Information
- **Rotación**: Diaria (crea un nuevo archivo cada día)
- **Retención**: 30 días
- **Tamaño máximo**: 10 MB por archivo
- **Formato**: `YYYY-MM-DD HH:mm:ss.fff [LEVEL] Mensaje`

#### c) Archivo de Errores (`logger/error-YYYYMMDD.txt`)
- **Nivel mínimo**: Error
- **Rotación**: Diaria
- **Retención**: 90 días
- **Tamaño máximo**: 50 MB por archivo
- **Incluye**: Stack trace completo y propiedades adicionales

## Estructura de Archivos

```
BiometricoZTK/
├── logger/                          # Carpeta de logs (creada automáticamente)
│   ├── log-20260215.txt            # Logs generales del día
│   ├── error-20260215.txt          # Solo errores del día
│   └── ...
├── Middleware/
│   └── ErrorLoggingMiddleware.cs   # Middleware de captura de errores
└── Program.cs                       # Configuración de Serilog
```

## Ejemplo de Log de Error

```
2026-02-15 14:32:45.123 -05:00 [ERR] Error no controlado capturado por middleware. ErrorId: a1b2c3d4-e5f6-7890-abcd-ef1234567890, Ruta: /api/zk/process, Método: POST, IP: 192.168.1.100
System.InvalidOperationException: Operación no válida
   at BiometricoZTK.Application.Services.ZTKProcessingService.ProcessData() in C:\...\ZTKProcessingService.cs:line 45
   at BiometricoZTK.Controllers.ZKController.ProcessFile() in C:\...\ZKController.cs:line 28

--- Stack Trace ---
Properties: {RequestId="0HMVKQK3QNQ7T", SourceContext="BiometricoZTK.Middleware.ErrorLoggingMiddleware"}
```

## Respuesta HTTP al Cliente

Cuando ocurre un error, el cliente recibe una respuesta JSON estructurada:

```json
{
  "success": false,
  "errorId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "message": "Ha ocurrido un error interno en el servidor. Por favor contacte al administrador.",
  "timestamp": "2026-02-15T14:32:45.123",
  "path": "/api/zk/process"
}
```

El `errorId` permite correlacionar el error reportado al cliente con el log registrado en el servidor.

## Uso en Controladores

Puedes usar ILogger en tus controladores para registrar información adicional:

```csharp
public class MiController : ControllerBase
{
    private readonly ILogger<MiController> _logger;

    public MiController(ILogger<MiController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Procesando solicitud GET");
        
        try
        {
            // Tu código aquí
            _logger.LogDebug("Operación completada exitosamente");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar GET");
            throw; // El middleware capturará esto
        }
    }
}
```

## Niveles de Log

- **Debug**: Información detallada para debugging (solo en Development)
- **Information**: Información general del flujo de la aplicación
- **Warning**: Advertencias que no detienen la ejecución
- **Error**: Errores que requieren atención
- **Fatal**: Errores críticos que detienen la aplicación

## Configuración Personalizada

Puedes modificar la configuración en:

1. **Program.cs**: Configuración programática de Serilog
2. **appsettings.json**: Configuración para producción
3. **appsettings.Development.json**: Configuración para desarrollo

### Cambiar Retención de Archivos

En `Program.cs`, modifica `retainedFileCountLimit`:

```csharp
.WriteTo.File(
    path: Path.Combine(logFolder, "error-.txt"),
    retainedFileCountLimit: 90,  // Cambia este número
    // ...
)
```

### Cambiar Tamaño Máximo de Archivo

Modifica `fileSizeLimitBytes` (en bytes):

```csharp
fileSizeLimitBytes: 52428800  // 50 MB (50 * 1024 * 1024)
```

## Consideraciones de Producción

1. **Espacio en Disco**: Monitorea el espacio disponible, especialmente en aplicaciones con alto tráfico
2. **Rotación**: Los archivos se rotan automáticamente por día y tamaño
3. **Seguridad**: Los logs pueden contener información sensible. Asegúrate de:
   - No registrar contraseñas o tokens
   - Proteger la carpeta `logger` con permisos adecuados
   - Excluir la carpeta del control de versiones (agregada a .gitignore)

## Mantenimiento

- Los archivos antiguos se eliminan automáticamente según la retención configurada
- No es necesario limpiar manualmente los logs
- Para depuración, busca por el `errorId` en los archivos de log

## Troubleshooting

### La carpeta logger no se crea
- La aplicación crea la carpeta automáticamente al iniciar
- Verifica permisos de escritura en el directorio de la aplicación

### Los logs no se escriben
- Verifica que Serilog esté correctamente configurado en Program.cs
- Revisa que el middleware esté registrado antes de otros middlewares
- Comprueba los permisos de escritura en la carpeta logger

### Archivos de log muy grandes
- Reduce el `fileSizeLimitBytes`
- Ajusta el `retainedFileCountLimit`
- Considera aumentar el nivel mínimo de log (por ejemplo, de Information a Warning)
