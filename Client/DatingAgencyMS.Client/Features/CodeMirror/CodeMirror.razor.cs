using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Features.CodeMirror;

public partial class CodeMirror : ComponentBase
{
    [Inject] private IJSRuntime JsRuntime { get; init; } = null!;
    private readonly string _editorId = $"editor-{Guid.NewGuid()}";

    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string Mode { get; set; } = null!;
    [Parameter] public string Theme { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine(Mode);
            await JsRuntime.InvokeVoidAsync("initCodeMirror", _editorId, Mode, Theme, Value, DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    public async Task UpdateValue(string value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(Value);
    }
}