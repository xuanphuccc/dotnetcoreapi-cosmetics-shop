namespace web_api_cosmetics_shop.Models.DTO
{
	public class ErrorDTO
	{
		public string? Title { get; set; }
		public int? Status { get; set; }
		public SubError? Errors { get; set; }

	}

	public class SubError
	{
		public List<string>? Message { get; set; }
	}
}
