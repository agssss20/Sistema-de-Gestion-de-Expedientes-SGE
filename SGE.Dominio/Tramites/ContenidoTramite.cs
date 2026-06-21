
using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

//genero la record class para garantizar que el contenido de mi tramite no sea null
public record class ContenidoTramite
{
    public string Valor{get;}
    public ContenidoTramite(string valor)
    {
        if(string.IsNullOrWhiteSpace(valor))
            throw new DominioException("El contenido del tramite no puede estar vacio.");

        Valor = valor.Trim();
    }

    public override string? ToString() => Valor;
}
