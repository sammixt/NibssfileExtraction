namespace NibssFileExtraction
{
    public class MatchModel 
    {
        public bool IsMatch { get; set; }
        public string Message { get; set; }
    }

    public class OutPutModel
    {
        public string Message { get; set; }
        public string Direction { get; set; }
        public string Product { get; set; }
        public string FileName { get; set; }
        public bool IsSuccessful { get; set; }
    }

}
