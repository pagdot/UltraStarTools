using Microsoft.AspNetCore.Components;

namespace DataGrid;

public abstract partial class BaseColumn<TGridItem> : ComponentBase
{
    private string _filterText = string.Empty;

    protected override Task OnInitializedAsync()
    {
        GridContext.AddColumn(this, IsDefaultSort);
        return Task.CompletedTask;
    }

    [Parameter] public bool IsDefaultSort { get; set; }
    [Parameter] public bool IsSortable { get; set; }
    [Parameter] public bool IsFilterable { get; set; }

    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string HeaderCssClass { get; set; } = string.Empty;
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }

    public string FilterText
    {
        get => _filterText;
        set
        {
            _filterText = value;
            GridContext.RefreshFilteredItems();
        }
    }

    public abstract RenderFragment Render(TGridItem item);

    public abstract IEnumerable<TGridItem> Sort(IEnumerable<TGridItem> items, bool ascending);
    public abstract bool Filter(TGridItem item);
    
    [CascadingParameter] public DataGrid<TGridItem> GridContext { get; set; } = default!;
    [Parameter] public string Width { get; set; } = "auto";
}