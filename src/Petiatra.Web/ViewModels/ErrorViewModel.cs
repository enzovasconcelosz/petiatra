namespace Petiatra.Web.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int? Codigo { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
