using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using Admin.DataAccess;

namespace Admin.Util
{
    public static class HtmlDisplayUtils
    {

        
        /// <summary>
        /// Hex encode a string for display
        /// </summary>
        public static String PrettyPrintBin(byte[] data)
        {
            StringBuilder sbx = new StringBuilder();
            Array.Resize(ref data, data.Length + (16 - data.Length % 16));
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 16 == 0) // QWORD
                {
                    sbx.Append("\r\n");
                    sbx.AppendFormat("0x{0:x8}  ", i);
                }
                else if (i % 4 == 0) // WORD
                    sbx.Append("  ");
                
                // Write data
                sbx.AppendFormat("{0:x2} ", data[i]);

                if (i % 16 == 15 || i == data.Length - 1) // Summary in text
                    sbx.Append("   " + System.Text.Encoding.UTF8.GetString(data, i - 15, 16).Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t"));
            }
            sbx.AppendFormat("\r\n\r\n{0:#,###} Bytes total", data.Length);
            return sbx.ToString();
        }


        /// <summary>
        /// Build alert column's glyph
        /// </summary>
        public static string BuildAuditAlertColumnStyle(AuditSummaryVw statusRow)
        {
            if (statusRow.IsAlert.HasValue && statusRow.IsAlert.Value)
                return "<span class=\"glyphicon glyphicon-bell\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"ALERT : This audit is important and may represent a potential security breach\"/>";
            return string.Empty;
        }

        /// <summary>
        /// Build the status column's glyph
        /// </summary>
        public static String BuildAuditStatusColumnStyle(AuditSummaryVw statusRow)
        {
            StringBuilder styleString = new StringBuilder();
            switch (statusRow.StatusCode)
            {
                case "NEW":
                    styleString.Append("<span class=\"glyphicon glyphicon-certificate\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"NEW : This audit has not been reviewed\"></span>");
                    break;
                case "HELD":
                    styleString.Append("<span class=\"glyphicon glyphicon-star\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"STAR : This audit will continue to appear on the summary page\"></span>");
                    break;
                case "ARCHIVED":
                    styleString.Append("<span class=\"glyphicon glyphicon-briefcase\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"ARCHIVED : This audit has been archived\"></span>");
                    break;
                case "OBSOLETE":
                    styleString.Append("<span class=\"glyphicon glyphicon-trash\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"REMOVED : This audit has been removed (deleted)\"></span>");
                    break;
                case "ACTIVE":
                    styleString.Append("<span class=\"glyphicon glyphicon-eye-open\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"REVIEWED : This audit has been reviewed\"></span>");
                    break;
                case "SYSTEM":
                    styleString.Append("<span class=\"glyphicon glyphicon-cog\" data-toggle=\"tooltip\" data-placement=\"right\" data-original-title=\"SYSTEM : This audit has been generated as the result of a normal SYSTEM function\"></span>");
                    break;
            }
            return styleString.ToString();
        }

        /// <summary>
        /// Build column action 
        /// </summary>
        public static string BuildAuditColumnAction(AuditSummaryVw item, bool allowViewClick = true)
        {
            string reviewAnchor = "<a href=\"javascript:\" onclick=\"javascript:AuditTool.View(this, " + item.AuditId + ");\" data-toggle=\"tooltip\" data-placement=\"left\" data-original-title=\"Review this audit\"><span class=\"glyphicon glyphicon-eye-open\"></span></a>",
                pinAnchor = "<a href=\"javascript:\" data-toggle=\"tooltip\" data-placement=\"left\" data-original-title=\"Keeps this object on the summary page\" onClick=\"javascript:AuditTool.Pin(this, " + item.AuditId + ");\"><span class=\"glyphicon glyphicon-star-empty\"></span></a>",
                archiveAnchor = "<a href=\"javascript:\" data-toggle=\"tooltip\" data-placement=\"left\" data-original-title=\"Archives this object and removes it from the summary page\" onClick=\"javascript:AuditTool.Archive(this, " + item.AuditId + ");\"><span class=\"glyphicon glyphicon-briefcase\"></span></a>",
                deleteAnchor = "<a href=\"javascript:\" data-toggle=\"tooltip\" data-placement=\"left\" data-original-title=\"Logically delete the audit so it no longer appears in searches\" onClick=\"javascript:AuditTool.Delete(this, " + item.AuditId + ");\"><span class=\"glyphicon glyphicon-trash\"></span></a>";

            if (!allowViewClick)
                reviewAnchor = "<span class=\"glyphicon glyphicon-eye-open\"></span>";
            switch (item.StatusCode)
            {
                case "NEW":
                    return String.Format("{0} | {1} | {2}", reviewAnchor, pinAnchor, deleteAnchor);
                case "ACTIVE":
                    return String.Format("{0} | {1} | {2} | {3}", reviewAnchor, pinAnchor, archiveAnchor, deleteAnchor);
                case "ARCHIVED":
                    return String.Format("{0} | {1} | {2}", reviewAnchor, pinAnchor, deleteAnchor);
                case "HELD":
                    return String.Format("{0} | {1} | {2}", reviewAnchor, archiveAnchor, deleteAnchor);
                case "OBSOLETE":
                    return String.Format("{0} | {1}", reviewAnchor, archiveAnchor);
                case "SYSTEM":
                    return String.Format("{0}", reviewAnchor);
            }
            return "";

        }

        /// <summary>
        /// Pretty print the XML String
        /// </summary>
        public static string PrettyPrintXml(byte[] data)
        {
            try
            {
                var doc = new XmlDocument();
                using (MemoryStream ms = new MemoryStream(data))
                using (XmlReader xr = XmlReader.Create(ms))
                    doc.Load(xr);
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { IndentChars = "  ", Indent = true }))
                        doc.WriteTo(xw);
                    return sw.ToString();
                }
            }
            catch
            {
                return "This data is not in XML format";
            }

        }

        public static object PrettyPrintTypeName(string p)
        {
            string[] tokens = p.Split(',');
            if (tokens[0].Contains("."))
                return tokens[0].Substring(tokens[0].LastIndexOf(".") + 1);
            return p;
        }


        /// <summary>
        /// Pretty print?
        /// </summary>
        public static HtmlString PrettyPrint(AtnaApi.Model.ObjectDetailType dtl)
        {
            // TODO : Other
            var baseUrl = ConfigurationManager.AppSettings["XslBaseUrl"];
            XslCompiledTransform xslt = new XslCompiledTransform();
            try
            {

                xslt.Load(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Content", "XSL", dtl.Type));
            }
            catch
            {
                xslt.Load(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Content", "XSL", "Default.xslt"));
            }

            try
            {
                // Transform
                StringWriter sw = new StringWriter();
                using (XmlReader rdr = XmlReader.Create(new MemoryStream(dtl.Value)))
                {
                    using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                        xslt.Transform(rdr, xw);
                }

                return new HtmlString(sw.ToString());
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
                return new HtmlString("This data is not in a renderable format");
            }
        }

        /// <summary>
        /// Build an item link
        /// </summary>
        public static HtmlString BuildItemLink(AtnaApi.Model.AuditableObject ao)
        {
            if (ao.IDTypeCode.StrongCode == AtnaApi.Model.AuditableObjectIdType.Uri)
                return new HtmlString(String.Format("<a target=\"_blank\" href=\"{0}\">{0}</a>", ao.ObjectId));
            else if (ao.ObjectId.Contains("^^^Audit"))
                return new HtmlString(String.Format("<a target=\"_blank\" href=\"/Audit#id={0}&action=View\">{1}</a>", ao.ObjectId.Substring(0, ao.ObjectId.IndexOf("^")), ao.ObjectId));
            else
                return new HtmlString(string.Format("<a href=\"javascript:\" onclick=\"AuditTool.ShowAdvancedSearchView({{ 'Object::ExternalIdentifier' : '{1}'}});\">{0}</a>", ao.ObjectId.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"), ao.ObjectId.Replace("&", "%26")));

        }
    }
}