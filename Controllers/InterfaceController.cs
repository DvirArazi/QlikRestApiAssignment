using Microsoft.AspNetCore.Mvc;

namespace QuikRestApiAssignment.Controllers;

[ApiController]
[Route("")]
public class InterfaceController : ControllerBase {
    [HttpGet]
    public ContentResult Home() {
        return base.Content(
            System.IO.File.ReadAllText("wwwroot/index.html"),
            "text/html"
        );
    }
}