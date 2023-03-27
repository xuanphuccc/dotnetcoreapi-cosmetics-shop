namespace web_api_cosmetics_shop.Models.DTO
{
    public class ResponseDTO
    {
        private int status = 200;
        private bool isSuccess = true;
        private int totalPages = 1;

        public int Status { get => status; set => status = value; }
        public dynamic? Data { get; set; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
        public int TotalPages { get => totalPages; set => totalPages = value; }
        public DateTime? Expired { get; set; }
    }
}
