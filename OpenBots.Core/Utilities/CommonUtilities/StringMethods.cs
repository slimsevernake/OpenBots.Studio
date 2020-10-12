using Microsoft.Office.Interop.Outlook;
using MimeKit;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenBots.Core.Utilities.CommonUtilities
{
    public static class StringMethods
    {
        public static string ToBase64(this string text)
        {
            return ToBase64(text, Encoding.UTF8);
        }

        public static string ToBase64(this string text, Encoding encoding)
        {
            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        public static bool TryParseBase64(this string text, out string decodedText)
        {
            return TryParseBase64(text, Encoding.UTF8, out decodedText);
        }

        public static bool TryParseBase64(this string text, Encoding encoding, out string decodedText)
        {
            if (string.IsNullOrEmpty(text))
            {
                decodedText = text;
                return false;
            }

            try
            {
                byte[] textAsBytes = Convert.FromBase64String(text);
                decodedText = encoding.GetString(textAsBytes);
                return true;
            }
            catch (System.Exception)
            {
                decodedText = null;
                return false;
            }
        }

        public static string ConvertObjectToString(object obj)
        {
            string type = "";
            if (obj != null)
                type = obj.GetType().FullName;

            switch (type)
            {
                case "System.String":
                    return obj.ToString();
                case "System.Security.SecureString":
                    return "*Secure String*";
                case "System.Data.DataTable":
                    return ConvertDataTableToString((DataTable)obj);
                case "System.Data.DataRow":
                    return ConvertDataRowToString((DataRow)obj);
                case "System.__ComObject":
                    return ConvertMailItemToString((MailItem)obj);
                case "MimeKit.MimeMessage":
                    return ConvertMimeMessageToString((MimeMessage)obj);
                case "OpenQA.Selenium.Remote.RemoteWebElement":
                    return ConvertIWebElementToString((IWebElement)obj);
                case "System.Drawing.Bitmap":
                    return ConvertBitmapToString((Bitmap)obj);
                case string a when a.Contains("System.Collections.Generic.List`1[[System.String"):
                case string b when b.Contains("System.Collections.Generic.List`1[[System.Data.DataTable"):
                case string c when c.Contains("System.Collections.Generic.List`1[[Microsoft.Office.Interop.Outlook.MailItem"):
                case string d when d.Contains("System.Collections.Generic.List`1[[MimeKit.MimeMessage"):
                case string e when e.Contains("System.Collections.Generic.List`1[[OpenQA.Selenium.IWebElement"):
                    return ConvertListToString(obj);
                case string a when a.Contains("System.Collections.Generic.Dictionary`2[[System.String") && a.Contains("],[System.String"):
                case string b when b.Contains("System.Collections.Generic.Dictionary`2[[System.String") && b.Contains("],[System.Object"):
                    return ConvertDictionaryToString(obj);
                case "":
                    return "null";
                default:
                    return "*Type Not Yet Supported*";
            }
        }

        public static string ConvertDataTableToString(DataTable dt)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("[[");

            for (int i = 0; i < dt.Columns.Count - 1; i++)
                stringBuilder.AppendFormat("{0}, ", dt.Columns[i].ColumnName);

            stringBuilder.AppendFormat("{0}]]", dt.Columns[dt.Columns.Count - 1].ColumnName);
            stringBuilder.AppendLine();

            foreach (DataRow rows in dt.Rows)
            {
                stringBuilder.Append("[");

                for (int i = 0; i < dt.Columns.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, ", rows[i]);

                stringBuilder.AppendFormat("{0}]", rows[dt.Columns.Count - 1]);
            }
            return stringBuilder.ToString();
        }

        public static string ConvertDataRowToString(DataRow row)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");

            for (int i = 0; i < row.ItemArray.Length - 1; i++)
                stringBuilder.AppendFormat("{0}, ", row.ItemArray[i]);

            stringBuilder.AppendFormat("{0}]", row.ItemArray[row.ItemArray.Length - 1]);
            return stringBuilder.ToString();
        }

        public static string ConvertMailItemToString(MailItem mail)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"[Subject: {mail.Subject}, \n" +
                                  $"Sender: {mail.SenderName}, \n" +
                                  $"Sent On: {mail.SentOn}, \n" +
                                  $"Unread: {mail.UnRead}, \n" +
                                  $"Attachments({mail.Attachments.Count})");

            if (mail.Attachments.Count > 0)
            {
                stringBuilder.Append(" [");
                foreach (Attachment attachment in mail.Attachments)
                    stringBuilder.Append($"{attachment.FileName}, ");

                //trim final comma
                stringBuilder.Length = stringBuilder.Length - 2;
                stringBuilder.Append("]");
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        public static string ConvertMimeMessageToString(MimeMessage message)
        {
            int attachmentCount = 0;
            foreach (var attachment in message.Attachments)
                attachmentCount += 1;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"[Subject: {message.Subject}, \n" +
                                  $"Sender: {message.From}, \n" +
                                  $"Sent On: {message.Date}, \n" +
                                  $"Attachments({attachmentCount})");

            if (attachmentCount > 0)
            {
                stringBuilder.Append(" [");
                foreach (var attachment in message.Attachments)
                    stringBuilder.Append($"{attachment.ContentDisposition?.FileName}, " ??
                                         "attached-message.eml, ");

                //trim final comma
                stringBuilder.Length = stringBuilder.Length - 2;
                stringBuilder.Append("]");
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        public static string ConvertIWebElementToString(IWebElement element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"[Text: {element.Text}, \n" +
                                 $"Tag Name: {element.TagName}, \n" +
                                 $"Location: {element.Location}, \n" +
                                 $"Size: {element.Size}, \n" +
                                 $"Displayed: {element.Displayed}, \n" +
                                 $"Enabled: {element.Enabled}, \n" +
                                 $"Selected: {element.Selected}]");
            return stringBuilder.ToString();
        }

        public static string ConvertBitmapToString(Bitmap bitmap)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Size({bitmap.Width}, {bitmap.Height})");
            return stringBuilder.ToString();
        }

        public static string ConvertListToString(object list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type type = list.GetType().GetGenericArguments()[0];

            if (type == typeof(string))
            {
                List<string> stringList = (List<string>)list;
                stringBuilder.Append($"Count({stringList.Count}) [");

                for (int i = 0; i < stringList.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, ", stringList[i]);

                if (stringList.Count > 0)
                    stringBuilder.AppendFormat("{0}]", stringList[stringList.Count - 1]);
                else
                    stringBuilder.Length = stringBuilder.Length - 2;
            }
            else if (type == typeof(DataTable))
            {
                List<DataTable> dataTableList = ((List<DataTable>)list).Take(3).ToList();
                stringBuilder.Append($"Count({dataTableList.Count}) \n[");

                for (int i = 0; i < dataTableList.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, \n", ConvertDataTableToString(dataTableList[i]));

                if (dataTableList.Count > 0)
                    stringBuilder.AppendFormat("{0}]", ConvertDataTableToString(dataTableList[dataTableList.Count - 1]));
                else
                    stringBuilder.Length = stringBuilder.Length - 3;
            }
            else if (type == typeof(MailItem))
            {
                List<MailItem> mailItemList = ((List<MailItem>)list).Take(3).ToList();

                stringBuilder.Append($"Count({mailItemList.Count}) \n[");

                for (int i = 0; i < mailItemList.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, \n", ConvertMailItemToString(mailItemList[i]));

                if (mailItemList.Count > 0)
                    stringBuilder.AppendFormat("{0}]", ConvertMailItemToString(mailItemList[mailItemList.Count - 1]));
                else
                    stringBuilder.Length = stringBuilder.Length - 3;
            }
            else if (type == typeof(MimeMessage))
            {
                List<MimeMessage> mimeMessageList = ((List<MimeMessage>)list).Take(3).ToList();
                stringBuilder.Append($"Count({mimeMessageList.Count}) \n[");

                for (int i = 0; i < mimeMessageList.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, \n", ConvertMimeMessageToString(mimeMessageList[i]));

                if (mimeMessageList.Count > 0)
                    stringBuilder.AppendFormat("{0}]", ConvertMimeMessageToString(mimeMessageList[mimeMessageList.Count - 1]));
                else
                    stringBuilder.Length = stringBuilder.Length - 3;
            }
            else if (type == typeof(IWebElement))
            {
                List<IWebElement> elementList = ((List<IWebElement>)list).Take(3).ToList();
                stringBuilder.Append($"Count({elementList.Count}) \n[");

                for (int i = 0; i < elementList.Count - 1; i++)
                    stringBuilder.AppendFormat("{0}, \n", ConvertIWebElementToString(elementList[i]));

                if (elementList.Count > 0)
                    stringBuilder.AppendFormat("{0}]", ConvertIWebElementToString(elementList[elementList.Count - 1]));
                else
                    stringBuilder.Length = stringBuilder.Length - 3;
            }

            return stringBuilder.ToString();
        }

        public static string ConvertDictionaryToString(object dictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type type = dictionary.GetType().GetGenericArguments()[1];
            dynamic stringDictionary = null;

            if (type == typeof(string))
            {
                stringDictionary = (Dictionary<string, string>)dictionary;
                stringBuilder.Append($"Count({stringDictionary.Count}) [");

                foreach (KeyValuePair<string, string> pair in stringDictionary)
                    stringBuilder.AppendFormat("[{0}, {1}], ", pair.Key, pair.Value);
            }
            else if (type == typeof(object))
            {
                stringDictionary = (Dictionary<string, object>)dictionary;
                stringBuilder.Append($"Count({stringDictionary.Count}) [");

                foreach (KeyValuePair<string, object> pair in stringDictionary)
                    stringBuilder.AppendFormat("[{0}, {1}], ", pair.Key, pair.Value == null ?
                                                string.Empty : pair.Value.ToString());
            }

            if (stringDictionary.Count > 0)
            {
                stringBuilder.Length = stringBuilder.Length - 2;
                stringBuilder.Append("]");
            }
            else
                stringBuilder.Length = stringBuilder.Length - 2;

            return stringBuilder.ToString();
        }
    }
}
