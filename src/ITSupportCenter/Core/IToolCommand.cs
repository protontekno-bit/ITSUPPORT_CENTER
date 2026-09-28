using System.Drawing;
using System.Threading.Tasks;

namespace ITSupportCenter.Core
{
    public interface IToolCommand
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        string Category { get; }
        string Keywords { get; }
        string Icon { get; }
        string ButtonText { get; }
        Color ButtonColor { get; }

        Task ExecuteAsync();
    }
}
