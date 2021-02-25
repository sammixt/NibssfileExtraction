using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibssFileExtraction
{
    public class Model
    {
		public string CHANNEL { get; set; }
		public string SESSION_ID { get; set; }
		public string NIBSS_REF_NO { get; set; }
		public string TRANSACTION_TYPE { get; set; }
		public string RESPONSE { get; set; }
		public string AMOUNT { get; set; }
		public string TRANSACTION_TIME { get; set; }
		public string ORIGINATOR_BILLER { get; set; }
		public string DESTINATION_ACCOUNT_NAME { get; set; }
		public string DESTINATION_ACCOUNT_NO { get; set; }
		public string NARRATION { get; set; }
		public string PAYMENT_REFERENCE { get; set; }
		public string LAST_12_DIGITS_OF_SESSION_ID { get; set; }
		public string PRODUCT { get; set; }
		public string DIRECTION { get; set; }
	}
}
