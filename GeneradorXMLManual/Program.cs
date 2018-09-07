using System;
using System.IO;
using Utils;
using Utils.Estructuras;

namespace GeneradorXMLManual
{
    class Program
    {
        private static Configuration _configuration;
        static void Main(string[] args)
        {
            Console.Write("Ingresar invoice number a procesar: ");
            var invoiceNumber = Console.ReadLine();
            _configuration = new ConfigurationReader().Read("RubySalesExporter.config.ini");
            var connDB = new ConnDB($"Dsn=micros;uid=dba;pwd=micros3700");
            var checks = connDB.ReadDb(_configuration, Convert.ToInt32(invoiceNumber), true);

            var index = 0;

            foreach (var check in checks)
            {
                GenerarXML(check, index);
                index++;
            }
            Console.WriteLine($"Procesado el fcrInvNumber {invoiceNumber}");
            Console.ReadLine();
        }
        static private void GenerarXML(MicrosCheck check, int index)
        {
            var filePath = $"{"TRX"}" +
                           $"{_configuration.CodigoTerminal}" +
                           $"{_configuration.CodigoBUPLA}" +
                           $"{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Year.ToString().Substring(2, 2)}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month}" : DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day}" : DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour}" : DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute}" : DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute.ToString())}" +
                           $"{check.Encabezado.IdDoc.Znumd}" +
                           $"{".xml"}";
            try
            {
                var doc = XmlFormatter.OpenFile(filePath);
                XmlFormatter.ImprimirDocumento(filePath, doc, index);
                XmlFormatter.ImprimirElementosEncabezado(doc, filePath, check.Encabezado, index);

                if (check.Detalle != null)
                {
                    var j = 1;

                    foreach (var detalle in check.Detalle)
                    {
                        XmlFormatter.ImprimirDetalle(doc, filePath, index, j);
                        XmlFormatter.ImprimirCdgItem(doc, filePath, index, j);
                        XmlFormatter.ImprimirElementosDetalle(doc, filePath, detalle, index, j);
                        j++;
                    }
                }

                XmlFormatter.ImprimirTotales(doc, filePath, index);
                XmlFormatter.ImprimirElementosTotales(doc, filePath, check.Totales, index);

                XmlFormatter.ImprimirDscrcgGlobal(doc, filePath, index);
                XmlFormatter.ImprimirElementosDscrcgGlobar(doc, filePath, check.DscrgGlobal, index);

                XmlFormatter.ImprimirParametros(doc, filePath, index);
                XmlFormatter.ImprimirElementosParametros(doc, filePath, check.Parametros, index, _configuration.EnviaMontoEscrito == "0");
                XmlFormatter.RenameXmlNodes(doc, filePath);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
