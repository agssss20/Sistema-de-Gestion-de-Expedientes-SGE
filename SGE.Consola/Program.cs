using SGE.Dominio.Comun;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Infraestructura;

// Limpiar archivos de persistencia al iniciar , esto en realidad no se debe hacer 
// se hizo principalmente para facilitar la demostración, ya que al ejecutar varias veces el programa se acumulan los datos y puede resultar confuso. 
string pathExp  = "Expediente.txt";
string pathTram = "Tramite.txt";
if (File.Exists(pathExp))  File.Delete(pathExp);
if (File.Exists(pathTram)) File.Delete(pathTram);

// Repositorios y servicios
var expedienteRepo = new ExpedienteTxtRepository();
var tramiteRepo    = new TramiteTxtRepository();
var autorizador    = new AutorizacionProvisionalService();
var servicioEstado = new ActualizacionEstadoExpedienteService(tramiteRepo, expedienteRepo);

// use cases de expediente
var agregarExpedienteUC = new AgregarExpedienteUseCase(expedienteRepo, autorizador);
var bajaExpedienteUC    = new BajaExpedienteUseCase(expedienteRepo, autorizador, tramiteRepo);
var modificarCaratulaUC = new ModificarCaratulaExpedienteUseCase(expedienteRepo, autorizador);
var modificarEstadoUC   = new ModificarEstadoExpedienteUseCase(expedienteRepo, autorizador);
var listarExpedientesUC = new ListarTodosLosExpedientesUseCase(expedienteRepo);

// use cases de tramite
var agregarTramiteUC   = new AgregarTramiteUseCase(tramiteRepo, autorizador, servicioEstado);
var bajaTramiteUC      = new BajaTramiteUseCase(tramiteRepo, autorizador, servicioEstado);
var modificarTramiteUC = new ModificarTramiteUseCase(tramiteRepo, autorizador, servicioEstado);
var listarTramitesUC   = new ListarTramitesPorExpedienteUseCase(tramiteRepo);

// Usuario simulado 
Guid usuarioAdmin = Guid.NewGuid();

// camino feliz :D

// Escenario de prueba: Agregar un expediente válido
Console.WriteLine("--- 1. Agregar expediente válido: 'Solicitud de habilitación comercial' ---");
Guid idExp1 = Guid.Empty;
try
{
    var request  = new AgregarExpedienteRequest("Solicitud de habilitación comercial", usuarioAdmin);
    var response = agregarExpedienteUC.Ejecutar(request);
    idExp1 = response.Id;
    Console.WriteLine($"[Éxito] Expediente agregado. ID: {response.Id}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Agregar otro expediente válido
Console.WriteLine("--- 2. Agregar expediente válido: 'Reclamo vecinal zona sur' ---");
Guid idExp2 = Guid.Empty;
try
{
    var request  = new AgregarExpedienteRequest("Reclamo vecinal zona sur", usuarioAdmin);
    var response = agregarExpedienteUC.Ejecutar(request);
    idExp2 = response.Id;
    Console.WriteLine($"[Éxito] Expediente agregado. ID: {response.Id}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Listar todos los expedientes
Console.WriteLine("--- 3. Listado de todos los expedientes ---");
try
{
    var response = listarExpedientesUC.Ejecutar();
    foreach (var exp in response.Expedientes)
        Console.WriteLine($"  > {exp}");
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[Error al listar]: {ex.Message}\n"); }

// Escenario de prueba: Agregar un trámite al expediente 1, no cambia el Estado
Console.WriteLine("--- 4. Agregar trámite 'EscritoPresentado' al expediente 1 ---");
Guid idTram1 = Guid.Empty;
try
{
    var request  = new AgregarTramiteRequest(idExp1, usuarioAdmin, new ContenidoTramite("Se presenta el escrito inicial del solicitante."));
    var response = agregarTramiteUC.Ejecutar(request);
    idTram1 = response.Id;
    Console.WriteLine($"[Éxito] Trámite agregado. ID: {response.Id}");
    Console.WriteLine($"        Etiqueta por defecto: EscritoPresentado");
    Console.WriteLine($"        Estado del expediente: {expedienteRepo.obtenerExpedientePorId(idExp1)?.Estado}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// modificar tramite a PaseAEstudio, lo que debería cambiar el estado del expediente a ParaResolver
Console.WriteLine("--- 5. Modificar trámite a 'PaseAEstudio' → estado esperado: ParaResolver ---");
try
{
    var reqMod = new ModificarTramiteRequest(idExp1, usuarioAdmin, new ContenidoTramite("El expediente pasa a estudio técnico."), EtiquetaTramite.PaseAEstudio);
    modificarTramiteUC.Ejecutar(idTram1, reqMod);
    Console.WriteLine("[Éxito] Trámite modificado a PaseAEstudio.");
    Console.WriteLine($"       Estado del expediente: {expedienteRepo.obtenerExpedientePorId(idExp1)?.Estado}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Agregar un nuevo trámite con etiqueta Resolucion, lo que debería cambiar el estado del expediente a ConResolucion
Console.WriteLine("--- 6. Agregar nuevo trámite 'Resolucion' → estado esperado: ConResolucion ---");
Guid idTram2 = Guid.Empty;
try
{
    // Agregamos con contenido; la etiqueta inicial es EscritoPresentado
    var reqAg = new AgregarTramiteRequest(idExp1, usuarioAdmin, new ContenidoTramite("Resolución favorable emitida por la autoridad competente."));
    var resAg = agregarTramiteUC.Ejecutar(reqAg);
    idTram2 = resAg.Id;
 
    // Lo modificamos a Resolucion
    var reqMod = new ModificarTramiteRequest(idExp1, usuarioAdmin, new ContenidoTramite("Resolución favorable emitida por la autoridad competente."), EtiquetaTramite.Resolucion);
    modificarTramiteUC.Ejecutar(idTram2, reqMod);

    Console.WriteLine("[Éxito] Trámite modificado a Resolucion.");
    Console.WriteLine($"       Estado del expediente: {expedienteRepo.obtenerExpedientePorId(idExp1)?.Estado}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Intentar cambiar el estado manualmente a EnNotificacion, lo que debería ser permitido 
Console.WriteLine("--- 7. Cambio de estado manual a 'EnNotificacion' (sin agregar trámite) ---");
try
{
    modificarEstadoUC.Ejecutar(idExp1, usuarioAdmin, EstadoExpediente.EnNotificacion);
    Console.WriteLine("[Éxito] Estado cambiado manualmente.");
    Console.WriteLine($"       Estado del expediente: {expedienteRepo.obtenerExpedientePorId(idExp1)?.Estado}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Listar trámites del expediente 1
Console.WriteLine("--- 8. Listado de trámites del expediente 1 ---");
try
{
    var response = listarTramitesUC.Ejecutar(new ListarTramitesPorExpedienteRequest(idExp1));
    foreach (var t in response.Tramites)
        Console.WriteLine($"  > {t}");
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[Error al listar]: {ex.Message}\n"); }

// Modificar la carátula del expediente 2
Console.WriteLine("--- 9. Modificar carátula del expediente 2 ---");
try
{
    modificarCaratulaUC.Ejecutar(idExp2, usuarioAdmin, "Reclamo vecinal zona norte (corregido)");
    Console.WriteLine("[Éxito] Carátula modificada.");
    Console.WriteLine($"       {expedienteRepo.obtenerExpedientePorId(idExp2)}\n");
}
catch (DominioException ex)      { Console.WriteLine($"[Error de Dominio]: {ex.Message}\n"); }
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Dar de baja el trámite de Resolución, lo que debería cambiar el estado del expediente a ParaResolver
Console.WriteLine("--- 10. Dar de baja el trámite 'Resolucion' → estado esperado: ParaResolver ---");
try
{
    bajaTramiteUC.Ejecutar(new BajaTramiteRequest(idExp1, usuarioAdmin, idTram2));
    Console.WriteLine("[Éxito] Trámite dado de baja.");
    Console.WriteLine($"       Estado del expediente: {expedienteRepo.obtenerExpedientePorId(idExp1)?.Estado}\n");
}
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Dar de baja el expediente 2, lo que debería eliminarlo junto con sus trámites
Console.WriteLine("--- 11. Dar de baja el expediente 2 (elimina sus trámites en cascada) ---");
try
{
    bajaExpedienteUC.Ejecutar(idExp2, usuarioAdmin);
    Console.WriteLine("[Éxito] Expediente y todos sus trámites dados de baja.\n");
}
catch (AutorizacionException ex) { Console.WriteLine($"[Error de Autorización]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Listar nuevamente todos los expedientes para verificar que solo queda el expediente 1
Console.WriteLine("--- 12. Listado final de expedientes (solo debe quedar el 1) ---");
try
{
    var response = listarExpedientesUC.Ejecutar();
    foreach (var exp in response.Expedientes)
        Console.WriteLine($"  > {exp}");
    Console.WriteLine();
}
catch (Exception ex) { Console.WriteLine($"[Error al listar]: {ex.Message}\n"); }


// caminos de Error >:(

// Escenario de prueba: Agregar expediente con carátula vacía, lo que debería lanzar una DominioException
Console.WriteLine("--- E1. Agregar expediente con carátula vacía → DominioException esperada ---");
try
{
    var request = new AgregarExpedienteRequest("", usuarioAdmin);
    agregarExpedienteUC.Ejecutar(request);
    Console.WriteLine("[Inesperado] No se lanzó excepción.\n");
}
catch (DominioException ex) { Console.WriteLine($"[Se atajó el error correctamente]: {ex.Message}\n"); }
catch (Exception ex)        { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Agregar expediente con carátula de solo espacios, lo que debería lanzar una DominioException
Console.WriteLine("--- E2. Agregar expediente con carátula de solo espacios → DominioException esperada ---");
try
{
    var request = new AgregarExpedienteRequest("     ", usuarioAdmin);
    agregarExpedienteUC.Ejecutar(request);
    Console.WriteLine("[Inesperado] No se lanzó excepción.\n");
}
catch (DominioException ex) { Console.WriteLine($"[Se atajó el error correctamente]: {ex.Message}\n"); }
catch (Exception ex)        { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Agregar trámite con contenido vacío, lo que debería lanzar una DominioException
Console.WriteLine("--- E3. Agregar trámite con contenido vacío → DominioException esperada ---");
try
{
    var request = new AgregarTramiteRequest(idExp1, usuarioAdmin, new ContenidoTramite(""));
    agregarTramiteUC.Ejecutar(request);
    Console.WriteLine("[Inesperado] No se lanzó excepción.\n");
}
catch (DominioException ex) { Console.WriteLine($"[Se atajó el error correctamente]: {ex.Message}\n"); }
catch (Exception ex)        { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Intentar ejecutar un caso de uso sin permisos, lo que debería lanzar una AutorizacionException
Console.WriteLine("--- E4. Sin permisos → AutorizacionException esperada ---");
Console.WriteLine("  (Para probarlo en código real: cambiar 'return true' a 'return false'");//se deberia comentar todo el codigo anterior a esta linea, para que no se acumulen los datos en la persistencia y se pueda llegar a este escenario sin problemas, luego de modificar el servicio de autorizacion se debe descomentar el codigo y ejecutar nuevamente)
Console.WriteLine("   en AutorizacionProvisionalService.TienePermiso y volver a ejecutar)");
Console.WriteLine();
// Demostración con mock inline:
var autorizadorSinPermisos = new AutorizacionProvisionalService(); // deberia modificarse para que devuelva false en TienePermiso
var ucSinPermisos = new AgregarExpedienteUseCase(expedienteRepo, autorizadorSinPermisos);
try
{
    var request = new AgregarExpedienteRequest("Expediente de prueba sin permisos", usuarioAdmin);
    ucSinPermisos.Ejecutar(request);
    Console.WriteLine("[Inesperado] No se lanzó excepción.\n");
}
catch (AutorizacionException ex) { Console.WriteLine($"[Se atajó el error correctamente]: {ex.Message}\n"); }
catch (Exception ex)             { Console.WriteLine($"[Error]: {ex.Message}\n"); }

// Escenario de prueba: Modificar la carátula de un expediente que no existe, lo que debería lanzar una DominioException
Console.WriteLine("--- E5. Modificar carátula de expediente inexistente ---");
try
{
    modificarCaratulaUC.Ejecutar(Guid.NewGuid(), usuarioAdmin, "Carátula que no va a ningún lado");
    Console.WriteLine("[Inesperado] No se lanzó excepción.\n");
}
catch (DominioException ex) { Console.WriteLine($"[Se atajó el error correctamente]: {ex.Message}\n"); }
catch (Exception ex)            { Console.WriteLine($"[Error]: {ex.Message}\n"); }