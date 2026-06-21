
namespace SGE.Dominio.Comun;

public class DominioException : Exception
{
    public DominioException()
{
}
public DominioException(string? message) : base(message)
{
}
public DominioException(string? message, Exception? innerException)
: base(message, innerException)
{
}
}
