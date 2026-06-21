
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Tramites;
// si el usuario tiene perrmiso para agregar un tramite, se llama a este use case que se encarga de la creacion del tramite con los datos enviados al request
// para luego llamar al servicio de actualizacion de estado del expediente que define el nuevo estado del expediente basado en el ultimo tramite agregado
public class AgregarTramiteUseCase(ITramiteRepository repository , IAutorizacionService autorizador, ActualizacionEstadoExpedienteService servicioActualizacionEstado)
{
    public AgregarTramiteResponse Ejecutar (AgregarTramiteRequest request) // le manda el request con los datos necesarios para crear el tramite, y devuelve un response con el id del tramite creado
    {
        if (autorizador.TienePermiso(request.UsuarioUltModificacion, Permiso.TramiteAlta))
        {
            var contenido = request.ContenidoTramite;
            var tramite = new Tramite(request.ExpId, contenido);
            tramite.ActualizarUsuarioModificacion(request.UsuarioUltModificacion);
            repository.AgregarTramite(tramite); // mete el tramite al repositorio llamando al metodo agregar tramite del repositorio, que se encarga de guardarlo
            servicioActualizacionEstado.Ejecutar(request.ExpId, request.UsuarioUltModificacion); // agrega al tramite al expediente y llama al servicio para determinar el nuevo estado del expediente
            return new AgregarTramiteResponse(tramite.Id); // devuelve un response con el id del tramite creado, para que el controlador lo pueda usar si es necesario
        }
        else
        {
                throw new AutorizacionException("El usuario no tiene permiso para dar de alta el expediente.");
        }
    }
}
