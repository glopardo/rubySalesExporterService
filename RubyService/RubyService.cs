using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using Utils;
using Utils.Estructuras;
using Timer = System.Timers.Timer;

namespace RubyService
{
    public partial class RubyService : ServiceBase
    {
        // Fields
        private Timer _timer;
        private int _ultimoCheck;
        private Configuration _configuration;
        private ConnDB _dbConn;
        private readonly string _logFilePath;
        private readonly string _iniFileName;
        private Thread _thread;

        public RubyService()
        {
            _logFilePath = @"d:\Netgroup\Ruby\RubySalesExporterService.log";
            _iniFileName = "RubySalesExporter.config.ini";
            components = null;
            InitializeComponent();
        }

        private void ActualizarIni(int ultimoCheck)
        {
            if (File.Exists(_iniFileName))
            {
                var list2 = File.ReadLines(_iniFileName).ToList().Select(s => s.Replace(_ultimoCheck.ToString(), ultimoCheck.ToString())).ToList();
                using (var writer = new StreamWriter(_iniFileName, false))
                {
                    foreach (var str in list2)
                    {
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
            }
        }

        private void GenerarXML(MicrosCheck check, int index)
        {
            var filePath = $"{@"D:\archivos-xml\"}" +
                           $"{"TRX"}" +
                           $"{_configuration.CodigoTerminal}" +
                           $"{_configuration.CodigoBUPLA}" +
                           $"{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Year.ToString().Substring(2, 2)}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month}" : DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day}" : DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour}" : DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour.ToString())}" +
                           $"{(DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute.ToString().Length == 1 ? $"0{DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute}" : DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute.ToString())}" +
                           $"{check.Encabezado.IdDoc.Znumd}" +
                           $"{".xml"}";

            var copyPath = $"{@"D:\Netgroup\Ruby\xml\"}" +
                           $"{"TRX"}" +
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
                //Logger.WriteLog($"Grabo xml en {filePath}", _logFilePath);
                var doc = XmlFormatter.OpenFile(filePath);
                XmlFormatter.ImprimirDocumento(filePath, doc, index);
                XmlFormatter.ImprimirElementosEncabezado(doc, filePath, check.Encabezado, index);
                
                //Logger.WriteLog($"Detalles qty: {check.Encabezado.IdDoc.Znumd} - {check.Detalle.Count}", _logFilePath);
                    
                if (check.Detalle.Count != 0)
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
                else
                {
                    Logger.WriteLog("El documento aún no posee detalles grabados en base de datos.", _logFilePath);
                }

                XmlFormatter.ImprimirTotales(doc, filePath, index);
                XmlFormatter.ImprimirElementosTotales(doc, filePath, check.Totales, index);

                XmlFormatter.ImprimirDscrcgGlobal(doc, filePath, index);
                XmlFormatter.ImprimirElementosDscrcgGlobar(doc, filePath, check.DscrgGlobal, index);

                XmlFormatter.ImprimirParametros(doc, filePath, index);
                XmlFormatter.ImprimirElementosParametros(doc, filePath, check.Parametros, index, _configuration.EnviaMontoEscrito == "0");
                XmlFormatter.RenameXmlNodes(doc, filePath);

                try
                {
                    File.Copy(filePath, copyPath, true);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog($"Error al copiar xml: {ex.Message}", _logFilePath);
                    throw;
                }
                
                Logger.WriteLog($"Se generó XML para fcrInvNumber: {check.Encabezado.IdDoc.Znumd} en la ruta {filePath}", _logFilePath);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error al generar XML: {ex.Message}", _logFilePath);
                throw;
            }
        }

        private void LeerIni()
        {
            try
            {
                //Directory.CreateDirectory(@"c:\Netgroup\Ruby\");
                Directory.CreateDirectory(@"D:\Netgroup\Ruby\xml\");

                _configuration = new ConfigurationReader().Read(_iniFileName);
                if (int.Parse(_configuration.UltimoCheck) != _ultimoCheck)
                {
                    _ultimoCheck = int.Parse(_configuration.UltimoCheck);
                    Logger.WriteLog($"Último check procesado: {_ultimoCheck}", _logFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error al leer archivo de configuración: {ex.Message}", _logFilePath);
            }
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(Read);
            LeerIni();
            _thread.Start();
            Logger.WriteLog("Servicio iniciado.", _logFilePath);
        }

        protected override void OnStop()
        {
            Logger.WriteLog("Servicio detenido.", _logFilePath);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            LeerIni();
            List<MicrosCheck> list;
            try
            {
                list = _dbConn.ReadDb(_configuration, _ultimoCheck, false);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error en TimedEvent: {ex.Message}", _logFilePath);
                throw;
            }
            var index = 0;

            foreach (var check in list)
            {
                if (check.Detalle.Count == 0)
                {
                    //Logger.WriteLog($"Check {check.Encabezado.IdDoc.Znumd} no tiene detalles.", _logFilePath);
                    break;
                }

                GenerarXML(check, index);
                ActualizarIni(int.Parse(check.Encabezado.IdDoc.Znumd));
                _ultimoCheck = int.Parse(check.Encabezado.IdDoc.Znumd);
                index++;
            }
        }

        private void Read()
        {
            _dbConn = new ConnDB($"Dsn=micros;uid=dba;pwd={_configuration.DbaPassword}");
            _timer = new Timer(30000);
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;
        }
    }
}
