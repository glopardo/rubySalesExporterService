namespace Utils.Estructuras
{
    public class MicrosCheckEncabezado
    {
        public IdDoc IdDoc { get; set; }
        public Emisor Emisor { get; set; }
        public Receptor Receptor { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal TaxTotal { get; set; }
    }
}
