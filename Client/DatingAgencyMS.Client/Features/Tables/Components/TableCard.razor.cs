using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Tables.Components;

public partial class TableCard : ComponentBase
{
    [Parameter, EditorRequired]
    public string TableName { get; init; }
    
    [Parameter, EditorRequired]
    public string TableLink { get; init; }
}