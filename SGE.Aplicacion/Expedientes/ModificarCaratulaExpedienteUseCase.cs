
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Autorizacion;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase (IExpedienteRepository expedienteRepository, IAutorizacionService autorizador)
{
    public void Ejecutar (Guid IdExpediente, Guid usuario, string nuevaCaratula)
    {
        if (autorizador.TienePermiso(usuario, Permiso.ExpedienteModificacion))
            {
            var expediente = expedienteRepository.obtenerExpedientePorId(IdExpediente);
            if (expediente == null)
            {
                throw new ApplicationException($"No se encontró el expediente con Id {IdExpediente}.");
            }
            expediente.ModificarCaratula(new Caratula(nuevaCaratula), usuario);
            expedienteRepository.ModificarExpediente(expediente);
            }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para modificar la carátula del expediente.");
            }
    }
}
