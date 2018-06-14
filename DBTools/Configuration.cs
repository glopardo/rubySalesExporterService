using System.Collections.Generic;

namespace Utils
{
    public class Configuration
    {
        public string UltimoCheck { get; set; }
        public string RutEmisor { get; set; }
        public string RazonSocialEmisor { get; set; }
        public string GiroEmisor { get; set; }
        public string CodigoSiiSucursal { get; set; }
        public string DireccionOrigenEmisor { get; set; }
        public string ComunaOrigenEmisor { get; set; }
        public string CiudadOrigenEmisor { get; set; }
        public string RutReceptor { get; set; }
        public string RazonSocialReceptor { get; set; }
        public string ContactoReceptor { get; set; }
        public string DireccionReceptor { get; set; }
        public string ComunaReceptor { get; set; }
        public string CiudadReceptor { get; set; }
        public string CodigoLocal { get; set; }
        public string CodigoTerminal { get; set; }
        public string CodigoBUPLA { get; set; }
        public string DbaPassword { get; set; }
        public List<RangoMapeo> SnackRango { get; set; }
        public List<RangoMapeo> BuffetRango { get; set; }
        public List<RangoMapeo> ComidaRapidaRango { get; set; }
        public List<RangoMapeo> ReposteriaRango { get; set; }
        public List<RangoMapeo> BarRango { get; set; }
        public List<RangoMapeo> CervezasRango { get; set; }
        public List<RangoMapeo> GaseosasRango { get; set; }
        public List<RangoMapeo> JugosRango { get; set; }
        public List<RangoMapeo> CafeteriaRango { get; set; }
    }
}