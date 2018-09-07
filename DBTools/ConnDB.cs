using System;
using System.Collections.Generic;
using System.Data.Odbc;
using Utils.Estructuras;

namespace Utils
{
    public class ConnDB
    {
        private readonly string _queryEncabezado;
        private readonly string _orderByEncabezado;
        private readonly string _queryDetalle;
        private readonly string _logFilePath;
        private readonly string _connString;
        public OdbcConnection Connection { get; set; }
        public OdbcConnection SubConnection { get; set; }
        public OdbcCommand RsHeader { get; set; }
        public OdbcCommand RsDetail { get; set; }

        public ConnDB(string connectionString)
        {
            _connString = connectionString;
            _queryEncabezado = "SELECT FCRINVNUMBER, MICROSCHKNUM, INVOICETYPE, FCRBSNZDATE, SUBTOTAL1 BRUTO, SUBTOTAL2 DESCUENTOS, SUBTOTAL5 EXENTO, SUBTOTAL9 PROPINA, SUBTOTAL6 IVA, SUBTOTAL8 NETO FROM MICROS.FCR_INVOICE_DATA WHERE FCRINVNUMBER ";
            _orderByEncabezado = "ORDER BY FCRINVNUMBER";
            _queryDetalle = "SELECT CONVERT(CHAR(10), FID.FCRBSNZDATE, 112) FECHA, FID.FCRINVNUMBER, FID.MICROSCHKNUM, MD.OBJ_NUM ITEM, D.DTL_NAME, D.RPT_CNT, D.CHK_TTL TOTAL FROM MICROS.FCR_INVOICE_DATA FID, MICROS.CHK_DTL CD, MICROS.TRANS_DTL TD, MICROS.DTL D, MICROS.MI_DTL MD WHERE CD.CHK_NUM = FID.MICROSCHKNUM AND CONVERT(CHAR(10), CD.CHK_CLSD_DATE_TIME,112) = CONVERT(CHAR(10), FID.FCRBSNZDATE,112) AND TD.CHK_SEQ = CD.CHK_SEQ AND D.TRANS_SEQ = TD.TRANS_SEQ AND MD.TRANS_SEQ = D.TRANS_SEQ AND D.DTL_SEQ = MD.DTL_SEQ AND D.CHK_TTL<> 0 AND FID.FCRINVNUMBER = ";
            _logFilePath = @"c:\Netgroup\Ruby\RubySalesExporterService.log";

            //Connection para los headers
            try
            {
                Connection = new OdbcConnection
                {
                    ConnectionString = _connString,
                    ConnectionTimeout = 600000
                };
                Connection.Open();

                SubConnection = new OdbcConnection
                {
                    ConnectionString = _connString,
                    ConnectionTimeout = 600000
                };
                SubConnection.Open();
            }
            catch (Exception exception)
            {
                Logger.WriteLog($"Error en conexión a BD: {exception.Message}", _logFilePath);
                throw;
            }
        }

        public List<MicrosCheck> ReadDb(Configuration configuration, int ultimoCheck, bool manual)
        {
            var list = new List<MicrosCheck>();

            RsHeader = new OdbcCommand
            {
                CommandText = !manual ? $"{_queryEncabezado} > {ultimoCheck} {_orderByEncabezado}" : $"{_queryEncabezado} = {ultimoCheck} {_orderByEncabezado}",
                Connection = Connection
            };

            RsDetail = new OdbcCommand {Connection = SubConnection};

            var reader = RsHeader.ExecuteReader();

            while (reader.Read())
            {
                string str;

                switch (reader[2].ToString())
                {
                    case "1":
                        str = "BO";
                        break;

                    case "2":
                        str = "FA";
                        break;

                    case "3":
                        str = "GD";
                        break;

                    case "4":
                        str = "NC";
                        break;

                    default:
                        str = "BO";
                        break;
                }

                var fechaHora = reader[3].ToString();
                var fechaHoraSeparado = fechaHora.Split(' ');
                var fecha = fechaHoraSeparado[0].Replace('/', '.');                
                var hora = fechaHoraSeparado[1];

                var doc = new IdDoc
                {
                    Blart = str,
                    Znumd = reader[0].ToString(),
                    Zarri = "",
                    Waers = "CLP",
                    Bldat = $"{fecha}",
                    Zhora = hora.Length > 7 ? hora.Substring(0, 8) : $"0{hora.Substring(0, 7)}",
                    Budat = $"{fecha}",
                    Bupla = configuration.CodigoBUPLA,
                    Zsect = "",
                    Tvorg = "",
                    Mwskz = "C1",
                    Vkont = ""
                };

                var emisor = new Emisor
                {
                    RutEmisor = configuration.RutEmisor,
                    RznSocEmisor = configuration.RazonSocialEmisor,
                    GiroEmisor = configuration.GiroEmisor.Length > 30 ? configuration.GiroEmisor.Substring(0, 30) : configuration.GiroEmisor,
                    CdgSiiSucur = configuration.CodigoSiiSucursal.Length > 6 ? configuration.CodigoSiiSucursal.Substring(0,6) : configuration.CodigoSiiSucursal,
                    DirOrigen = configuration.DireccionOrigenEmisor.Length > 15 ? configuration.DireccionOrigenEmisor.Substring(0,15) : configuration.DireccionOrigenEmisor,
                    CmnaOrigen = configuration.ComunaOrigenEmisor,
                    CiudadOrigen = configuration.CiudadOrigenEmisor
                };

                var receptor = new Receptor
                {
                    RutRecep = configuration.RutReceptor,
                    RznSocRecep = configuration.RazonSocialReceptor,
                    Contacto = configuration.ContactoReceptor,
                    DirRecep = configuration.DireccionReceptor,
                    CmnaRecep = configuration.ComunaReceptor,
                    CiudadRecep = configuration.CiudadReceptor
                };

                var encabezado = new MicrosCheckEncabezado
                {
                    IdDoc = doc,
                    Emisor = emisor,
                    Receptor = receptor
                };
                
                var totales = new MicrosCheckTotales();
                try
                {
                    totales = new MicrosCheckTotales
                    {
                        MntNeto = decimal.Parse(reader[4].ToString()) - decimal.Parse(reader[8].ToString()),
                        TasaIva = 19M,
                        Iva = decimal.Parse(reader[8].ToString()),
                        MntTotal = decimal.Parse(reader[9].ToString())
                    };
                }
                catch (Exception ex)
                {
                    Logger.WriteLog($"Error al crear totales: {ex.Message}", _logFilePath);
                }

                var global = new MicrosCheckDscrcgGlobal
                {
                    NroLinDr = 1,
                    TpoMov = "D",
                    GlosaDr = "DESCUENTO",
                    TpoValor = "%",
                    ValorDr = reader[5].ToString()
                };

                var parametros = new MicrosCheckParametros
                {
                    MontoEscrito = configuration.EnviaMontoEscrito == "1" ? NumToText.Convert(reader[9].ToString()) : string.Empty,
                    Refer = string.Empty,
                    Neto = string.Empty
                };

                var listaDetalle = new List<MicrosCheckDetalle>();
                OdbcDataReader subReader;

                try
                {
                    RsDetail.CommandText = $"{_queryDetalle} {reader[0]}";
                    subReader = RsDetail.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(ex.InnerException != null ? $"Error al ejecutar reader de subquery: {ex.InnerException}" : $"Error al ejecutar reader de subquery: {ex.Message}", _logFilePath);
                    throw;
                }

                var i = 1;

                while (subReader.Read())
                {
                    var num3 = int.Parse(subReader[3].ToString());
                    var str6 = "Categoria";
                    var str7 = "Subcategoria";
                    var str8 = "Segmento";
                    var str9 = "Codigo";

                    foreach (var mapeo in configuration.SnackRango)
                    {
                        if ((num3 >= mapeo.Desde) && (num3 <= mapeo.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Alimentacion";
                            str8 = "Snack";
                            str9 = "12474108";
                        }
                    }

                    foreach (var mapeo2 in configuration.BarRango)
                    {
                        if ((num3 >= mapeo2.Desde) && (num3 <= mapeo2.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Bebida";
                            str8 = "Bar";
                            str9 = "161724";
                        }
                    }

                    foreach (var mapeo3 in configuration.BuffetRango)
                    {
                        if ((num3 >= mapeo3.Desde) && (num3 <= mapeo3.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Alimentacion";
                            str8 = "Buffet";
                            str9 = "122131";
                        }
                    }

                    foreach (var mapeo4 in configuration.CafeteriaRango)
                    {
                        if ((num3 >= mapeo4.Desde) && (num3 <= mapeo4.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Bebida";
                            str8 = "Servicios de cafeteria";
                            str9 = "1673106";
                        }
                    }

                    foreach (var mapeo5 in configuration.CervezasRango)
                    {
                        if ((num3 >= mapeo5.Desde) && (num3 <= mapeo5.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Bebida";
                            str8 = "Cervezas";
                            str9 = "162841";
                        }
                    }

                    foreach (var mapeo6 in configuration.GaseosasRango)
                    {
                        if ((num3 >= mapeo6.Desde) && (num3 <= mapeo6.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Bebida";
                            str8 = "Gaseosas";
                            str9 = "164261";
                        }
                    }

                    foreach (var mapeo7 in configuration.JugosRango)
                    {
                        if ((num3 >= mapeo7.Desde) && (num3 <= mapeo7.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Bebida";
                            str8 = "Jugos";
                            str9 = "164971";
                        }
                    }

                    foreach (var mapeo8 in configuration.ReposteriaRango)
                    {
                        if ((num3 >= mapeo8.Desde) && (num3 <= mapeo8.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Alimentacion";
                            str8 = "Reposteria";
                            str9 = "126999";
                        }
                    }

                    foreach (var mapeo9 in configuration.ComidaRapidaRango)
                    {
                        if ((num3 >= mapeo9.Desde) && (num3 <= mapeo9.Hasta))
                        {
                            str6 = "Alimentacion y comida";
                            str7 = "Alimentacion";
                            str8 = "Comida Rapida";
                            str9 = "123147";
                        }
                    }

                    //Logger.WriteLog(string.Format("Codigo terminal: {0}", configuration.CodigoTerminal), _logFilePath);

                    var item = new CdgItem
                    {
                        ZCant = subReader[5].ToString(),
                        ZCodp = str9,
                        ZCate = str6,
                        ZSubc = str7,
                        ZSegm = str8,
                        CodigoItemMicros = subReader[3].ToString(),
                        ZSubs = subReader[4].ToString(),
                        Betrw = Math.Round(double.Parse(subReader[6].ToString()) / 1.19, 0).ToString(),
                        DescuentoMonto = decimal.Zero,
                        Zimad = configuration.CodigoTerminal
                    };

                    var detalle = new MicrosCheckDetalle
                    {
                        NroLinDet = i,
                        CdgItem = item
                    };

                    var itemRepetido = false;
                    //En index almaceno el índice del ítem existente para luego aumentarle ZCANT
                    var index = 0;

                    foreach (var elementoDetalle in listaDetalle)
                    {
                        if (elementoDetalle.CdgItem.CodigoItemMicros == item.CodigoItemMicros)
                        {
                            itemRepetido = true;
                            break;
                        }
                        index++;
                    }

                    if (!itemRepetido)
                        listaDetalle.Add(detalle);
                    else
                    {
                        listaDetalle[index].CdgItem.ZCant = (Convert.ToInt32(listaDetalle[index].CdgItem.ZCant) + 1).ToString();
                    }

                    i++;
                }

                subReader.Close();

                var check = new MicrosCheck
                {
                    Encabezado = encabezado,
                    Detalle = listaDetalle,
                    DscrgGlobal = global,
                    Parametros = parametros,
                    Totales = totales
                };
                list.Add(check);
            }
            reader.Close();
            return list;
        }
    }

}
