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
        private string _logFilePath;
        private string _iniFileName;
        private Thread _thread;

        public RubyService()
        {
            _logFilePath = @"c:\Netgroup\Ruby\RubySalesExporterService.log";
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
            var str = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", @"Z:\", "TRX", _configuration.CodigoTerminal, _configuration.CodigoBUPLA,
                DateTime.Parse(check.Encabezado.IdDoc.Bldat).Year, DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month,
                DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day, DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour,
                DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute, check.Encabezado.IdDoc.Znumd, ".xml");
            var filePath = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", @"C:\Netgroup\Ruby\", "TRX",
                _configuration.CodigoTerminal, _configuration.CodigoBUPLA,
                DateTime.Parse(check.Encabezado.IdDoc.Bldat).Year, DateTime.Parse(check.Encabezado.IdDoc.Bldat).Month,
                DateTime.Parse(check.Encabezado.IdDoc.Bldat).Day, DateTime.Parse(check.Encabezado.IdDoc.Zhora).Hour,
                DateTime.Parse(check.Encabezado.IdDoc.Zhora).Minute, check.Encabezado.IdDoc.Znumd, ".xml");
            try
            {
                Logger.WriteLog(string.Format("Grabo xml en {0}", filePath), _logFilePath);
                var doc = XmlFormatter.OpenFile(filePath);
                XmlFormatter.ImprimirDocumento(filePath, doc, index);
                XmlFormatter.ImprimirElementosEncabezado(doc, filePath, check.Encabezado, index);
                XmlFormatter.ImprimirDetalle(doc, filePath, index);

                if (check.Detalle != null)
                {
                    var j = 1;

                    foreach (var detalle in check.Detalle)
                    {
                        XmlFormatter.ImprimirCdgItem(doc, filePath, index, j);
                        XmlFormatter.ImprimirElementosDetalle(doc, filePath, detalle, index, j);
                        j++;
                    }
                }

                XmlFormatter.ImprimirTotales(doc, filePath, index);
                XmlFormatter.ImprimirElementosTotales(doc, filePath, check.Totales, index);

                if (check.DscrgGlobal.ValorDr.Substring(0, 1) != "0")
                {
                    XmlFormatter.ImprimirDscrcgGlobal(doc, filePath, index);
                    XmlFormatter.ImprimirElementosDscrcgGlobar(doc, filePath, check.DscrgGlobal, index);
                }

                XmlFormatter.ImprimirParametros(doc, filePath, index);
                XmlFormatter.ImprimirElementosParametros(doc, filePath, check.Parametros, index);
                XmlFormatter.RenameXmlNodes(doc, filePath);
                Logger.WriteLog($"Se generó XML para fcrInvNumber: {check.Encabezado.IdDoc.Znumd}", _logFilePath);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error al generar XML: {ex.Message}", _logFilePath);
                throw;
            }

            str = @"\\192.168.1.32\RecibidosXml\";
            try
            {
                Logger.WriteLog(string.Format("Intento copiar xml a {0}", str), _logFilePath);
                var arguments = string.Format("/C copy /b {0} {1}", filePath, str);
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    Verb = "runas"
                };

                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error al copiar archivo a {str}: {ex.Message}", _logFilePath);
            }
        }

        private void LeerIni()
        {
            try
            {
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
            var list = new List<MicrosCheck>();
            try
            {
                list = _dbConn.ReadDb(_configuration, _ultimoCheck);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Error en TimedEvent: {ex.Message}", _logFilePath);
                throw;
            }
            var index = 0;

            foreach (var check in list)
            {
                GenerarXML(check, index);
                Logger.WriteLog($"Procesado el fcrInvNumber {check.Encabezado.IdDoc.Znumd}", _logFilePath);
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
