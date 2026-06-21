
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Tramites;
// si el usuario tiene perrmiso para eliminar un tramite, se llama a este use case que se encarga de la eliminacion a partir de los datos enviados al request
// para luego llamar al servicio de actualizacion de estado del expediente que define el nuevo estado del expediente basado en el ultimo tramite agregado
public class BajaTramiteUseCase(ITramiteRepository tramiteRepository, IAutorizacionService autorizador, ActualizacionEstadoExpedienteService actualizacionEstadoService)
{
    public BajaTramiteResponse Ejecutar (BajaTramiteRequest request)
    {
        if (autorizador.TienePermiso(request.UsuarioUltModificacion, Permiso.TramiteBaja))
            {
                tramiteRepository.TramiteBaja(request.IdTramite);
                actualizacionEstadoService.Ejecutar(request.IdExp, request.UsuarioUltModificacion);
                return new BajaTramiteResponse(true);
            }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para eliminar el trámite.");
            }
    }
}