
using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion.Expedientes;

public class BajaExpedienteUseCase (IExpedienteRepository expedienteRepository, IAutorizacionService autorizador, ITramiteRepository tramiteRepository)
{
    public void Ejecutar (Guid IdExp, Guid usuario)
    {
        if (autorizador.TienePermiso(usuario, Permiso.ExpedienteBaja))
            {
                tramiteRepository.EliminarPorExpedienteId(IdExp);
                expedienteRepository.ExpedienteBaja(IdExp);
            }
        else
            {
                throw new AutorizacionException("El usuario no tiene permiso para dar de baja el expediente.");
            }
    }
}
