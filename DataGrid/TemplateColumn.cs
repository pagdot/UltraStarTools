using Microsoft.AspNetCore.Components;

namespace DataGrid;

public class TemplateColumn<TGridItem> : BaseColumn<TGridItem>
{
    public override IEnumerable<TGridItem> Sort(IEnumerable<TGridItem> items, bool ascending) => items;

    public override bool Filter(TGridItem item)
    {
        return true;
    }
    
    [Parameter] public RenderFragment<TGridItem>? BodyTemplate { get; set; }
    
    public override RenderFragment Render(TGridItem item) => x => x.AddContent(0, BodyTemplate is null ? default : BodyTemplate(item));
}
