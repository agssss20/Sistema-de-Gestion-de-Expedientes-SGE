
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

public class ModificarEstadoExpedienteUseCase (IExpedienteRepository expedienteRepository,  IAutorizacionService autorizador)
{
    public void Ejecutar (Guid IdExpediente, Guid usuario, EstadoExpediente nuevoestado)
    {
        if (autorizador.TienePermiso(usuario, Permiso.ExpedienteModificacion))
            {
            var expediente = expedienteRepository.obtenerExpedientePorId(IdExpediente);
            if (expediente == null){
                throw new ApplicationException($"No se encontró el expediente con Id {IdExpediente}.");
            }
            expediente.CambiarEstado(nuevoestado, usuario);
            expedienteRepository.ModificarExpediente(expediente);
            }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para modificar el estado del expediente.");
            }
    }
}
