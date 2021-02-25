using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssFileExtraction
{
    class Program
    {
        static Logger log = new Logger();
        static void Main(string[] args)
        {
            DatabaseOperations db = new DatabaseOperations(log);
            OutPutModel output = new OutPutModel();
            output = ProcessSettlementFiles(db, "SuccessfulJsonFiles".GetKeyValue(), "SuccessfulTable".GetKeyValue());
            if (!output.IsSuccessful)
            {
                log.Info($"Error Encountered while Extraction Successful Files.. Rolling Back.....");
                db.TruncateTable("SuccessfulTable".GetKeyValue());
                goto Label_Rollback;
            }
                
            output = ProcessSettlementFiles(db, "UnsuccessfulJsonFiles".GetKeyValue(), "UnsuccessfulTable".GetKeyValue());

            if (!output.IsSuccessful)
            {
                log.Info($"Error Encountered while Extraction Unsuccessful Files.. Rolling Back.....");
                db.TruncateTable("UnsuccessfulTable".GetKeyValue());
                goto Label_Rollback;
            }
               



            Label_Rollback:
                {
                    ProcessFiles.WriteOutput(output.IsSuccessful, "downloadResultPath".GetKeyValue());
                    if (!output.IsSuccessful)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Product: {output.Product}");
                        sb.AppendLine($"Direction: {output.Direction}");
                        sb.AppendLine($"File: {output.FileName}");
                        sb.AppendLine($"{output.Message}");
                        ProcessFiles.WriteOutput(sb.ToString(), "messagePath".GetKeyValue());
                    }
                }
            //Delete all from table
            Console.WriteLine("Completed");

        }

        private static OutPutModel ProcessSettlementFiles(DatabaseOperations db, string fileName, string tableName)
        {
            OutPutModel output = new OutPutModel();
            try
            {
                log.Info("Read Json");
                var fileParameters = ProcessFiles.ReadJsonFile(fileName);
                //string fileName, Direction, Product, Message;

                foreach (var parmeter in fileParameters)
                {
                    log.Info("Extract Files");
                    var settlementObject = ProcessFiles.ParseFile(parmeter);
                    if (settlementObject != null)
                    {
                        log.Info($"Checking Columns for :: {parmeter.Product} :: {parmeter.Direction} :: {parmeter.File}");
                        var _ = ProcessFiles.CheckIfColumnsMatch(settlementObject.FirstOrDefault());
                        if (!_.IsMatch)
                        {
                            output.FileName = parmeter.File;
                            output.Direction = parmeter.Direction;
                            output.Product = parmeter.Product;
                            output.Message = _.Message;
                            output.IsSuccessful = false;
                            //goto Label_Rollback;
                        }
                        else
                        {
                            var dtable = db.LINQResultToDataTable<Model>(settlementObject);
                            output.IsSuccessful = db.InsertRecord(dtable, tableName);
                            //output.IsSuccessful = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
            

            return output;
        }

       
    }
}
