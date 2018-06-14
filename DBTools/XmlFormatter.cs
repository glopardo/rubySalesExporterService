using System;
using System.IO;
using System.Xml.Linq;
using Utils.Estructuras;

namespace Utils
{
    public class XmlFormatter
    {
        #region Constants
        private const string _transaccion = "TRANSACCION";
        private const string _documento = "DOCUMENTO";
        private const string _encabezado = "ENCABEZADO";
        private const string _idDoc = "IDDOC";
        private const string _emisor = "EMISOR";
        private const string _receptor = "RECEPTOR";
        private const string _detalle = "DETALLE";
        private const string _cdgItem = "CDGITEM";
        private const string _totales = "TOTALES";
        private const string _dscrcgglobal = "DSCRCGGLOBAL";
        private const string _parametros = "PARAMETROS";
        private const string _logFilePath = @"c:\Netgroup\Ruby\RubySalesExporterService.log";
        #endregion

        #region Document
        /// <summary>
        /// Imprime estructura del documento sin datos con tags de encabezado
        /// </summary>
        /// <param name="path">Ruta del documento a generar</param>
        /// <param name="doc">Objeto del XML</param>
        /// <param name="index">Número de documento a imprimir</param>
        public static void ImprimirDocumento(string path, XDocument doc, int index)
        {
            //Logger.WriteLog("Inicia ImprimirDocumento", _logFilePath);
            var root = new XElement(_documento + "-" + index);
            doc.Element(_transaccion).Add(root);

            ImprimirEncabezado(doc, path, index);
            doc.Save(path);
            //Logger.WriteLog("Finaliza ImprimirDocumento", _logFilePath);
        }

        #region Imprimir Elementos
        public static void ImprimirElementosEncabezado(XDocument doc, string filePath, MicrosCheckEncabezado enc, int index)
        {
            //Logger.WriteLog("Inicia ImprimirElementosEncabezado", _logFilePath);

            ImprimirElementosIdDoc(doc, filePath, enc.IdDoc, index);
            ImprimirElementosEmisor(doc, filePath, enc.Emisor, index);
            ImprimirElementosReceptor(doc, filePath, enc.Receptor, index);

            //Logger.WriteLog("Finaliza ImprimirElementosEncabezado", _logFilePath);
        }

        #region Elementos Encabezado
        public static void ImprimirElementosIdDoc(XDocument doc, string filePath, IdDoc idDoc, int index)
        {
            //Logger.WriteLog("Inicia ImprimirElementosIdDoc", _logFilePath);

            try
            {
                var documento = _documento + "-" + index;
                var idDocElemento = new XElement(_idDoc);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Add(idDocElemento);

                doc.Save(filePath);

                var elementoIdDoc = new XElement("BLART", idDoc.Blart);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("ZNUMD", idDoc.Znumd);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("ZARRI", idDoc.Zarri);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("WAERS", idDoc.Waers);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("BLDAT", idDoc.Bldat);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("ZHORA", idDoc.Zhora);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("BUDAT", idDoc.Budat);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("BUPLA", idDoc.Bupla);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("ZSECT", idDoc.Zsect);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("TVORG", idDoc.Tvorg);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("MWSKZ", idDoc.Mwskz);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                elementoIdDoc = new XElement("VKONT", idDoc.Vkont);
                doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_idDoc).Add(elementoIdDoc);

                doc.Save(filePath);
            }
            catch (Exception e)
            {
                Logger.WriteLog(string.Format("Error en ImprimirElementosIdDoc: {0}", e.Message), _logFilePath);
            }

            //Logger.WriteLog("Finaliza ImprimirElementosIdDoc", _logFilePath);
        }
        public static void ImprimirElementosEmisor(XDocument doc, string filePath, Emisor emisor, int index)
        {
            var documento = _documento + "-" + index;

            var emisorElemento = new XElement(_emisor);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Add(emisorElemento);

            var elementoEmisor = new XElement("RUTEMISOR", emisor.RutEmisor);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("RZNSOCEMISOR", emisor.RznSocEmisor);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("GIROEMISOR", emisor.GiroEmisor);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("CDGSIISUCUR", emisor.CdgSiiSucur);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("DIRORIGEN", emisor.DirOrigen);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("CMNAORIGEN", emisor.CmnaOrigen);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            elementoEmisor = new XElement("CIUDADORIGEN", emisor.CiudadOrigen);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_emisor).Add(elementoEmisor);

            doc.Save(filePath);
        }
        public static void ImprimirElementosReceptor(XDocument doc, string filePath, Receptor receptor, int index)
        {
            var documento = _documento + "-" + index;

            var receptorElemento = new XElement(_receptor);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Add(receptorElemento);

            var elementoReceptor = new XElement("RUTRECEP", receptor.RutRecep);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            elementoReceptor = new XElement("RZNSOCRECEP", receptor.RznSocRecep);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            elementoReceptor = new XElement("CONTACTO", receptor.Contacto);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            elementoReceptor = new XElement("DIRRECEP", receptor.DirRecep);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            elementoReceptor = new XElement("CMNARECEP", receptor.CmnaRecep);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            elementoReceptor = new XElement("CIUDADRECEP", receptor.CiudadRecep);
            doc.Element(_transaccion).Element(documento).Element(_encabezado).Element(_receptor).Add(elementoReceptor);

            doc.Save(filePath);
        }
        #endregion

        public static void ImprimirElementosDetalle(XDocument doc, string filePath, MicrosCheckDetalle detalle, int i, int j)
        {
            var document = _documento + "-" + i;

            var elementoDetalle = new XElement("NROLINDET", j);
            doc.Element(_transaccion).Element(document).Element(_detalle).Add(elementoDetalle);

            ImprimirElementosCdgItem(doc, filePath, detalle.CdgItem, i, j);

            doc.Save(filePath);
        }

        #region Elementos Detalle
        public static void ImprimirElementosCdgItem(XDocument doc, string filePath, CdgItem cdgItem, int i, int j)
        {
            var documento = _documento + "-" + i;
            var codItem = _cdgItem + "-" + j;

            var categoria = "Alimentacion y comida";
            var subcAlimentacion = "Alimentacion";
            var subcBebida = "Bebida";

            var elementoIdDoc = new XElement("ZCANT", cdgItem.ZCant);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZCODP", cdgItem.ZCodp);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZCATE", cdgItem.ZCate);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZSUBC", cdgItem.ZSubc);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZSEGM", cdgItem.ZSegm);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZSUBS", cdgItem.ZSubs);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("BETRW", cdgItem.Betrw);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("DESCUENTOMONTO", cdgItem.DescuentoMonto);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            elementoIdDoc = new XElement("ZIMAD", cdgItem.Zimad);
            doc.Element(_transaccion).Element(documento).Element(_detalle).Element(codItem).Add(elementoIdDoc);

            doc.Save(filePath);
        }
        #endregion

        public static void ImprimirElementosTotales(XDocument doc, string filePath, MicrosCheckTotales totales, int i)
        {
            var document = _documento + "-" + i;

            var elementoTotales = new XElement("MNTNETO", totales.MntNeto.ToString("####"));
            doc.Element(_transaccion).Element(document).Element(_totales).Add(elementoTotales);

            elementoTotales = new XElement("TASAIVA", totales.TasaIva);
            doc.Element(_transaccion).Element(document).Element(_totales).Add(elementoTotales);

            elementoTotales = new XElement("IVA", totales.Iva.ToString("####"));
            doc.Element(_transaccion).Element(document).Element(_totales).Add(elementoTotales);

            elementoTotales = new XElement("MNTTOTAL", totales.MntTotal.ToString("####"));
            doc.Element(_transaccion).Element(document).Element(_totales).Add(elementoTotales);

            doc.Save(filePath);
        }
        public static void ImprimirElementosDscrcgGlobar(XDocument doc, string filePath, MicrosCheckDscrcgGlobal dscrcgGlobal, int i)
        {
            var document = _documento + "-" + i;

            var elementoDscrcgGlobal = new XElement("NROLINDR", i);
            doc.Element(_transaccion).Element(document).Element(_dscrcgglobal).Add(elementoDscrcgGlobal);

            elementoDscrcgGlobal = new XElement("TPOMOV", dscrcgGlobal.TpoMov);
            doc.Element(_transaccion).Element(document).Element(_dscrcgglobal).Add(elementoDscrcgGlobal);

            elementoDscrcgGlobal = new XElement("GLOSADR", dscrcgGlobal.GlosaDr);
            doc.Element(_transaccion).Element(document).Element(_dscrcgglobal).Add(elementoDscrcgGlobal);

            elementoDscrcgGlobal = new XElement("TPOVALOR", "%");
            doc.Element(_transaccion).Element(document).Element(_dscrcgglobal).Add(elementoDscrcgGlobal);

            elementoDscrcgGlobal = new XElement("VALORDR", dscrcgGlobal.ValorDr);
            doc.Element(_transaccion).Element(document).Element(_dscrcgglobal).Add(elementoDscrcgGlobal);

            doc.Save(filePath);
        }
        public static void ImprimirElementosParametros(XDocument doc, string filePath, MicrosCheckParametros parametros, int i)
        {
            var document = _documento + "-" + i;

            var elementoParametros = new XElement("MONTO_ESCRITO", parametros.MontoEscrito);
            doc.Element(_transaccion).Element(document).Element(_parametros).Add(elementoParametros);

            elementoParametros = new XElement("REFER", parametros.Refer);
            doc.Element(_transaccion).Element(document).Element(_parametros).Add(elementoParametros);

            elementoParametros = new XElement("NETO", parametros.Neto);
            doc.Element(_transaccion).Element(document).Element(_parametros).Add(elementoParametros);

            elementoParametros = new XElement("ESPECIFICACIONES", parametros.Neto);
            doc.Element(_transaccion).Element(document).Element(_parametros).Add(elementoParametros);

            doc.Save(filePath);
        }
        #endregion

        #endregion

        #region Public methods
        public static XDocument OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Logger.WriteLog("El archivo XML no existe, se genera uno nuevo.", _logFilePath);
                try
                {
                    return new XDocument(new XElement(string.Format("{0}", _transaccion)));
                }
                catch (Exception e)
                {
                    Logger.WriteLog(string.Format("Error en OpenFile: {0}", e.Message), _logFilePath);
                }
            }

            File.Delete(filePath);
            Logger.WriteLog(string.Format("Se borra el archivo {0}.", filePath), _logFilePath);
            return new XDocument(new XElement(string.Format("{0}", _transaccion)));
        }
        public static void ImprimirEncabezado(XDocument doc, string filePath, int index)
        {
            var encabezado = new XElement(_encabezado);
            doc.Element(_transaccion).Element(_documento + "-" + index).Add(encabezado);

            doc.Save(filePath);
        }
        public static void ImprimirDetalle(XDocument doc, string filePath, int index)
        {
            var detalle = new XElement(_detalle);
            doc.Element(_transaccion).Element(_documento + "-" + index).Add(detalle);
            doc.Save(filePath);
        }
        public static void ImprimirCdgItem(XDocument doc, string filePath, int i, int j)
        {
            var cdgItem = new XElement(_cdgItem + "-" + j);
            doc.Element(_transaccion).Element(_documento + "-" + i).Element(_detalle).Add(cdgItem);
            doc.Save(filePath);
        }
        public static void ImprimirTotales(XDocument doc, string filePath, int index)
        {
            var totales = new XElement(_totales);
            doc.Element(_transaccion).Element(_documento + "-" + index).Add(totales);
            doc.Save(filePath);
        }
        public static void ImprimirDscrcgGlobal(XDocument doc, string filePath, int index)
        {
            var dscrcgGlobal = new XElement(_dscrcgglobal);
            doc.Element(_transaccion).Element(_documento + "-" + index).Add(dscrcgGlobal);
            doc.Save(filePath);
        }
        public static void ImprimirParametros(XDocument doc, string filePath, int index)
        {
            var parametros = new XElement(_parametros);
            doc.Element(_transaccion).Element(_documento + "-" + index).Add(parametros);
            doc.Save(filePath);
        }
        public static void RenameXmlNodes(XDocument doc, string filePath)
        {
            //Logger.WriteLog("Renaming " + filePath, );

            foreach (var element in doc.Descendants())
            {
                if (element.Name.LocalName.StartsWith("DOCUMENTO-"))
                    element.Name = _documento;
                if (element.Name.LocalName.StartsWith("DETALLE-"))
                    element.Name = _detalle;
                if (element.Name.LocalName.StartsWith("CDGITEM-"))
                    element.Name = _cdgItem;
            }
            doc.Save(filePath);
        }
        #endregion
    }
}
