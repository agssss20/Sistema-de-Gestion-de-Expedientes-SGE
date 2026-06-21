
namespace SGE.Aplicacion.Tramites;

public class ListarTramitesPorExpedienteUseCase(ITramiteRepository repository)
{
    public ListarTramitesPorExpedienteResponse Ejecutar(ListarTramitesPorExpedienteRequest request)
    {
        if(request.IdExp == Guid.Empty)
            throw new ApplicationException("El Id del expediente no puede ser vacío.");
        var tramites = repository.ListarTramitesPorExpediente(request.IdExp);
        return new ListarTramitesPorExpedienteResponse(tramites);
    }
}
