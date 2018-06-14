using System;
using System.Collections.Generic;
using System.IO;

namespace Utils
{
    public class ConfigurationReader
    {
        private readonly string _logFilePath;

        public ConfigurationReader()
        {
            _logFilePath = @"c:\Netgroup\Ruby\RubySalesExporterService.log";
        }

        public Configuration Read(string filePath)
        {
            string str;
            var dictionary = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                return null;
            }

            var reader = new StreamReader(filePath);

            while ((str = reader.ReadLine()) != null)
            {
                if (str[0] != '#')
                {
                    var separator = new char[] { '=' };
                    var strArray = str.Split(separator);
                    dictionary.Add(strArray[0], strArray[1]);
                }
            }

            reader.Close();

            var configuration = new Configuration
            {
                UltimoCheck = dictionary["UltimoCheck"],
                RutEmisor = dictionary["RutEmisor"],
                RazonSocialEmisor = dictionary["RazonSocialEmisor"],
                GiroEmisor = dictionary["GiroEmisor"],
                CodigoSiiSucursal = dictionary["CodigoSiiSucursal"],
                DireccionOrigenEmisor = dictionary["DireccionOrigenEmisor"],
                CiudadOrigenEmisor = dictionary["CiudadOrigenEmisor"],
                ComunaOrigenEmisor = dictionary["ComunaOrigenEmisor"],
                RutReceptor = dictionary["RutReceptor"],
                RazonSocialReceptor = dictionary["RazonSocialReceptor"],
                ContactoReceptor = dictionary["ContactoReceptor"],
                DireccionReceptor = dictionary["DireccionReceptor"],
                ComunaReceptor = dictionary["ComunaReceptor"],
                CiudadReceptor = dictionary["CiudadReceptor"],
                CodigoTerminal = dictionary["codigoTerminal"],
                CodigoBUPLA = dictionary["codigoBUPLA"],
                DbaPassword = dictionary["DbaPassword"]
            };

            try
            {
                RangoMapeo mapeo2;
                configuration.SnackRango = new List<RangoMapeo>();
                var separator = new char[] { ';' };

                foreach (var str2 in dictionary["SnackRango"].Split(separator))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray3 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str2.Split(chArray3)[0]);
                    var chArray4 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str2.Split(chArray4)[1]);
                    var item = mapeo2;
                    configuration.SnackRango.Add(item);
                }

                configuration.BuffetRango = new List<RangoMapeo>();
                var chArray5 = new char[] { ';' };

                foreach (var str3 in dictionary["BuffetRango"].Split(chArray5))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray6 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str3.Split(chArray6)[0]);
                    var chArray7 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str3.Split(chArray7)[1]);
                    var item = mapeo2;
                    configuration.BuffetRango.Add(item);
                }

                configuration.ComidaRapidaRango = new List<RangoMapeo>();
                var chArray8 = new char[] { ';' };

                foreach (var str4 in dictionary["ComidaRapidaRango"].Split(chArray8))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray9 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str4.Split(chArray9)[0]);
                    var chArray10 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str4.Split(chArray10)[1]);
                    var item = mapeo2;
                    configuration.ComidaRapidaRango.Add(item);
                }

                configuration.ReposteriaRango = new List<RangoMapeo>();
                var chArray11 = new char[] { ';' };

                foreach (var str5 in dictionary["ReposteriaRango"].Split(chArray11))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray12 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str5.Split(chArray12)[0]);
                    var chArray13 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str5.Split(chArray13)[1]);
                    var item = mapeo2;
                    configuration.ReposteriaRango.Add(item);
                }

                configuration.BarRango = new List<RangoMapeo>();
                var chArray14 = new char[] { ';' };

                foreach (var str6 in dictionary["BarRango"].Split(chArray14))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray15 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str6.Split(chArray15)[0]);
                    var chArray16 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str6.Split(chArray16)[1]);
                    var item = mapeo2;
                    configuration.BarRango.Add(item);
                }

                configuration.CervezasRango = new List<RangoMapeo>();
                var chArray17 = new char[] { ';' };

                foreach (var str7 in dictionary["CervezasRango"].Split(chArray17))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray18 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str7.Split(chArray18)[0]);
                    var chArray19 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str7.Split(chArray19)[1]);
                    var item = mapeo2;
                    configuration.CervezasRango.Add(item);
                }

                configuration.GaseosasRango = new List<RangoMapeo>();
                var chArray20 = new char[] { ';' };

                foreach (var str8 in dictionary["GaseosasRango"].Split(chArray20))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray21 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str8.Split(chArray21)[0]);
                    var chArray22 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str8.Split(chArray22)[1]);
                    var item = mapeo2;
                    configuration.GaseosasRango.Add(item);
                }

                configuration.JugosRango = new List<RangoMapeo>();
                var chArray23 = new char[] { ';' };

                foreach (var str9 in dictionary["JugosRango"].Split(chArray23))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray24 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str9.Split(chArray24)[0]);
                    var chArray25 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str9.Split(chArray25)[1]);
                    var item = mapeo2;
                    configuration.JugosRango.Add(item);
                }

                configuration.CafeteriaRango = new List<RangoMapeo>();
                var chArray26 = new char[] { ';' };

                foreach (var str10 in dictionary["CafeteriaRango"].Split(chArray26))
                {
                    mapeo2 = new RangoMapeo();
                    var chArray27 = new char[] { '-' };
                    mapeo2.Desde = int.Parse(str10.Split(chArray27)[0]);
                    var chArray28 = new char[] { '-' };
                    mapeo2.Hasta = int.Parse(str10.Split(chArray28)[1]);
                    var item = mapeo2;
                    configuration.CafeteriaRango.Add(item);
                }
            }
            catch (Exception)
            {
                Logger.WriteLog("Error al leer rangos del archivo de configuración.", _logFilePath);
                throw;
            }
            return configuration;
        }
    }
}
