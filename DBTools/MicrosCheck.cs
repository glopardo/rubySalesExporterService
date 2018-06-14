using System.Collections.Generic;

namespace Utils.Estructuras
{
    public class MicrosCheck
    {
        public MicrosCheckEncabezado Encabezado { get; set; }
        public List<MicrosCheckDetalle> Detalle { get; set; }
        public MicrosCheckTotales Totales { get; set; }
        public MicrosCheckDscrcgGlobal DscrgGlobal { get; set; }
        public MicrosCheckParametros Parametros { get; set; }
    }
}
