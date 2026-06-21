
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Tramites; 
// si el usuario tiene perrmiso para modificar un tramite, se llama a este use case que se encarga de la modificacion con los datos enviados al request
// para luego llamar al servicio de actualizacion de estado del expediente que define el nuevo estado del expediente basado en el ultimo tramite agregado
public class ModificarTramiteUseCase(ITramiteRepository tramiteRepository , IAutorizacionService autorizador, ActualizacionEstadoExpedienteService actualizacionEstadoService)
{
    public ModificarTramiteResponse Ejecutar (Guid IdTramite, ModificarTramiteRequest request)
    {
        if (autorizador.TienePermiso(request.UsuarioUltModificacion, Permiso.TramiteModificacion))
            {
                var tramite = tramiteRepository.obtenerTramitePorId(IdTramite);
                tramite.ModificarTramite(request.Etiqueta, request.ContenidoTramite, request.UsuarioUltModificacion);
                tramiteRepository.ModificarTramite(tramite);
                actualizacionEstadoService.Ejecutar(request.ExpId, request.UsuarioUltModificacion); 
                return new ModificarTramiteResponse(tramite.Id);
            }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para modificar la carátula del expediente.");
            }
    }
}