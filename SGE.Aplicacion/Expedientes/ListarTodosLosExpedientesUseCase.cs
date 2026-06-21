
namespace SGE.Aplicacion.Expedientes;

public class ListarTodosLosExpedientesUseCase(IExpedienteRepository repository)
{
    public ListarTodosLosExpedientesResponse Ejecutar()
    {
        var expedientes = repository.ListarTodosLosExpedientes();
        return new ListarTodosLosExpedientesResponse(expedientes);
    }

}
