using pdftron;
using pdftron.PDF;
using pdftron.SDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public  class ExportPDF
    {
        public static void GenerateHTMLtoPDF(string PDFPath, string HTML)
        {
            try
            {
                //PDFNet.Initialize();
                PDFNet.Initialize("demo:1650982361576:7ba45564030000000053fe47d8a4097ee0d5c9cbe8329d2697763ea0c4");
                using (PDFDoc doc = new PDFDoc())
                {
                   
                    var pa = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                    HTML2PDF.SetModulePath(AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
                    HTML2PDF converter = new HTML2PDF();
                    // Add html data
                    converter.InsertFromHtmlString(HTML);
                    if (converter.Convert(doc))
                        doc.Save(PDFPath, SDFDoc.SaveOptions.e_linearized);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static string ReplaceHTML( string actualPath)
        {
            try
            {
                string htmlTemplate = System.IO.File.ReadAllText(actualPath);
                return htmlTemplate;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}