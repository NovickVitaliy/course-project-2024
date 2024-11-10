using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.SqlQueries.Pages;

public partial class SqlQueries : ComponentBase
{
    public string EditorContent { get; set; }


    private async Task Process()
    {
        Console.WriteLine(EditorContent);
    }
}