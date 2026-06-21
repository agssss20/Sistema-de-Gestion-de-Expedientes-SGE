
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

public class AgregarExpedienteUseCase (IExpedienteRepository expedienteRepository,  IAutorizacionService autorizador)
{
public AgregarExpedienteResponse Ejecutar (AgregarExpedienteRequest request)
    {
        if (autorizador.TienePermiso(request.usuarioModificacion, Permiso.ExpedienteAlta))
        {
            var caratula = new Caratula(request.caratula);
            var expediente = new Expediente(caratula);
            expediente.ActualizarUsuarioModificacion(request.usuarioModificacion);
            expedienteRepository.AgregarExpediente(expediente);
            return new AgregarExpedienteResponse(expediente.Id);
        }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para dar de alta el expediente.");
            }
    }
}
