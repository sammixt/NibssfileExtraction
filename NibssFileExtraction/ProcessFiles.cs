using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace NibssFileExtraction
{
    class ProcessFiles
    {
        public static List<FileParameters> ReadJsonFile(string filepath)
        {
            try
            {
                string jsonFilePath = File.ReadAllText(filepath);
                List<FileParameters> fileParameters = JsonConvert.DeserializeObject<List<FileParameters>>(jsonFilePath);
                return fileParameters;
            }
            catch (Exception)
            {

                throw;
            }
            
            
        }

        public static List<Model> ParseFile(FileParameters parameter)
        {
            List<Model> models = new List<Model>();
            string[] delimiters = { "," };
            if (!File.Exists(parameter.File)) return null;
            using (TextFieldParser parser = FileSystem.OpenTextFieldParser(parameter.File, delimiters))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                string[] fields;

                while (!parser.EndOfData)
                {
                    Model model = new Model();
                    fields = parser.ReadFields();
                    int fieldLength = fields.Length;
                    if(fieldLength >= 13)
                    {
                        model.CHANNEL = TrimColumnLength(((fieldLength >= 13 && fields[1] != null) ? fields[1] : null),50);
                        model.SESSION_ID = TrimColumnLength(((fieldLength >= 13 && fields[2] != null) ? fields[2].Replace("'", "").Trim() : null),50);
                        model.NIBSS_REF_NO = TrimColumnLength(ExtractReference((fieldLength >= 13 && fields[10] != null) ? fields[10].Replace("'", "").Trim() : null),50);
                        model.TRANSACTION_TYPE = TrimColumnLength(((fieldLength >= 13 && fields[3] != null) ? fields[3].Replace("'", "").Trim() : null),50);
                        model.RESPONSE = TrimColumnLength(((fieldLength >= 13 && fields[4] != null) ? fields[4].Replace("'", "").Trim() : null),50);
                        model.AMOUNT = TrimColumnLength(((fieldLength >= 13 && fields[5] != null) ? fields[5].Replace("'", "").Trim() : null),50);
                        model.TRANSACTION_TIME = TrimColumnLength(((fieldLength >= 13 && fields[6] != null) ? fields[6].Replace("'", "").Trim() : null),50);
                        model.ORIGINATOR_BILLER = TrimColumnLength(((fieldLength >= 13 && fields[7] != null) ? fields[7].Replace("'", "").Trim() : null),500);
                        model.DESTINATION_ACCOUNT_NAME = TrimColumnLength(((fieldLength >= 13 && fields[8] != null) ? fields[8].Replace("'", "").Trim() : null),500);
                        model.DESTINATION_ACCOUNT_NO = TrimColumnLength(((fieldLength >= 13 && fields[9] != null) ? fields[9].Replace("'", "").Trim() : null),50);
                        model.NARRATION = TrimColumnLength(((fieldLength >= 13 && fields[10] != null) ? fields[10].Replace("'", "").Trim() : null),500);
                        model.PAYMENT_REFERENCE = TrimColumnLength(((fieldLength >= 13 && fields[11] != null) ? fields[11].Replace("'", "").Trim() : null),500);
                        model.LAST_12_DIGITS_OF_SESSION_ID = TrimColumnLength(((fieldLength >= 13 && fields[12] != null) ? fields[12].Replace("'", "").Trim() : null),50);
                        model.PRODUCT = parameter.Product;
                        model.DIRECTION = parameter.Direction;
                        models.Add(model);
                    }
                        
                    //Console.WriteLine(field);

                }
            }

            return models;
        }

        public static Dictionary<string, string> GetKeysAndColumns()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (CellElement file in RequiredCell.GetCells())
            {
                keyValuePairs.Add(file.CellName, file.ColumnName);
            }

            return keyValuePairs;
        }

        public static MatchModel CheckIfColumnsMatch(Model model)
        {
            bool IsMatch = true;
            string Message = string.Empty;
            var requiredColumns = GetKeysAndColumns();
            if(model != null)
            {
                if (!requiredColumns["F2"].Equals(model.CHANNEL)){IsMatch = false; Message = $"The Column B doesn't Match expected column {requiredColumns["F2"]} {Environment.NewLine}"; }
                if (!requiredColumns["F3"].Equals(model.SESSION_ID)) { IsMatch = false; Message += $"The Column C doesn't Match expected column {requiredColumns["F3"]} {Environment.NewLine}"; }
                if (!requiredColumns["F4"].Equals(model.TRANSACTION_TYPE)) { IsMatch = false; Message += $"The Column D doesn't Match expected column {requiredColumns["F4"]} {Environment.NewLine}"; }
                if (!requiredColumns["F5"].Equals(model.RESPONSE)) { IsMatch = false; Message += $"The Column E doesn't Match expected column {requiredColumns["F5"]} {Environment.NewLine}"; }
                if (!requiredColumns["F6"].Equals(model.AMOUNT)) { IsMatch = false; Message += $"The Column F doesn't Match expected column {requiredColumns["F6"]} {Environment.NewLine}"; }
                if (!requiredColumns["F7"].Equals(model.TRANSACTION_TIME)) { IsMatch = false; Message += $"The Column G doesn't Match expected column {requiredColumns["F7"]} {Environment.NewLine}"; }
                if (!requiredColumns["F8"].Equals(model.ORIGINATOR_BILLER)) { IsMatch = false; Message += $"The Column H doesn't Match expected column {requiredColumns["F8"]} {Environment.NewLine}"; }
                if (!requiredColumns["F9"].Equals(model.DESTINATION_ACCOUNT_NAME)) { IsMatch = false; Message += $"The Column I doesn't Match expected column {requiredColumns["F9"]} {Environment.NewLine}"; }
                if (!requiredColumns["F10"].Equals(model.DESTINATION_ACCOUNT_NO)) { IsMatch = false; Message += $"The Column J doesn't Match expected column {requiredColumns["F10"]} {Environment.NewLine}"; }
                if (!requiredColumns["F11"].Equals(model.NARRATION)) { IsMatch = false; Message += $"The Column K doesn't Match expected column {requiredColumns["F11"]} {Environment.NewLine}"; }
                if (!requiredColumns["F12"].Equals(model.PAYMENT_REFERENCE)) { IsMatch = false; Message += $"The Column L doesn't Match expected column {requiredColumns["F12"]} {Environment.NewLine}"; }
                if (!requiredColumns["F13"].Equals(model.LAST_12_DIGITS_OF_SESSION_ID)) { IsMatch = false; Message += $"The Column M doesn't Match expected column {requiredColumns["F13"]} {Environment.NewLine}"; }


            }
            MatchModel match = new MatchModel
            {
                IsMatch = IsMatch,
                Message = Message
            };
            return match;
        }

        private static string ExtractReference(string narration)
        {
            string pattern = @"REF[:\s]*?[_A-Z\d]+";
            if (!string.IsNullOrEmpty(narration))
            {
                var result = Regex.Match(narration, pattern);
                string extractedRef = result.Groups[0].Value;
                string reference = extractedRef.Replace("REF", "").Replace("ECOART", "").Replace(":", "");
                return reference;
            }

            return narration;
        }

        public static void WriteOutput(bool result, string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
                File.WriteAllText(downloadPath, result.ToString());
            }
            else
            {
                File.WriteAllText(downloadPath, result.ToString());
            }
        }

        public static void WriteOutput(string result, string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
                File.WriteAllText(downloadPath, result);
            }
            else
            {
                File.WriteAllText(downloadPath, result);
            }
        }

        static string TrimColumnLength( string columnValue, int totalLength)
        {
            if (string.IsNullOrEmpty(columnValue))
                return columnValue;
            if (columnValue.Length < totalLength)
            {
                return columnValue;
            }
            else
            {
                string newValue = columnValue.Substring(0, (totalLength - 2));
                return newValue;
            }
        }
    }
}
