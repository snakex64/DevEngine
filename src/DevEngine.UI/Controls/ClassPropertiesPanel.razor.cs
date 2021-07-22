using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class ClassPropertiesPanel : ComponentBase
    {
        public enum TreeViewItemType
        {
            MethodHeader, Method
        }

        private class TreeViewItem
        {
            /// <summary>
            /// Only ever null while it's being created / before it's named the first time
            /// </summary>
            public string? Text { get; set; }

            public TreeViewItemType Type { get; set; }

            public List<TreeViewItem> Children { get; } = new List<TreeViewItem>();

            public bool IsRenaming { get; set; }

            public FakeTypes.Method.DevMethod? Method { get; internal set; }
        }

        private IList<TreeViewItem> Items { get; } = new List<TreeViewItem>();

        private IList<TreeViewItem> ExpandedTreeViewItems = new List<TreeViewItem>();

        private TreeViewItem? SelectedTreeViewItem { get; set; }

        [CascadingParameter]
        public RightClickController? RightClickController { get; set; }

        [EditorRequired]
        [Parameter]
        public FakeTypes.Class.DevClass DevClass { get; set; } = null!;

        [Parameter]
        public EventCallback<FakeTypes.Method.DevMethod> OnOpenMethodRequested { get; set; }


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if( firstRender )
            {
                Initialize();

                StateHasChanged();
            }
        }

        #region Initialize

        private void Initialize()
        {
            var methodHeader = new TreeViewItem()
            {
                Text = "Methods",
                Type = TreeViewItemType.MethodHeader
            };
            Items.Add(methodHeader);
            ExpandedTreeViewItems.Add(methodHeader);

            foreach( var method in DevClass.Methods )
            {
                methodHeader.Children.Add(new TreeViewItem()
                {
                    Text = method.Name,
                    Method= (FakeTypes.Method.DevMethod)method,
                    Type = TreeViewItemType.Method
                });
            }
        }

        #endregion

        #region OnTreeViewItemRenamed

        private void OnTreeViewItemRenamed(TreeViewItem item, string newValue)
        {
            item.IsRenaming = false;

            if (item.Type == TreeViewItemType.Method && item.Method == null)
            {
                item.Text = newValue;

                item.Method = CreateNewEmptyMethod(newValue);

                OnSelectedTreeViewItemChanged(item);
            }

        }

        #endregion

        #region OnTreeViewRightClick

        private void OnTreeViewRightClick(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == 2 && SelectedTreeViewItem != null)
            {
                List<RightClickController.MenuItem> items;
                if (SelectedTreeViewItem.Type == TreeViewItemType.MethodHeader)
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("Create method", OnCreateMethodClicked),
                    };
                }
                else
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("nothing here yet", () => { }),
                    };
                }
                RightClickController?.DisplayRightClickMenu(mouseEventArgs, items);
            }
        }

        #endregion

        #region OnSelectedTreeViewItemChanged

        private void OnSelectedTreeViewItemChanged(TreeViewItem item)
        {
            SelectedTreeViewItem = item;

            if( SelectedTreeViewItem?.Type == TreeViewItemType.Method)
            {
                if (SelectedTreeViewItem.IsRenaming) // don't open it if we clicked while renaming, we'll open it after it's renamed
                    return;

                OnOpenMethodRequested.InvokeAsync(SelectedTreeViewItem.Method ?? throw new Exception("Method shouldn't be null here"));
            }
        }

        #endregion

        #region OnCreateMethodClicked

        private void OnCreateMethodClicked()
        {
            if (SelectedTreeViewItem == null)
                return;

            var newMethodItem = new TreeViewItem()
            {
                IsRenaming = true,
                Type = TreeViewItemType.Method
            };

            SelectedTreeViewItem.Children.Add(newMethodItem);

            if(!ExpandedTreeViewItems.Contains(SelectedTreeViewItem))
                ExpandedTreeViewItems.Add(SelectedTreeViewItem);

            StateHasChanged();
        }

        #endregion

        #region CreateNewEmptyMethod

        private FakeTypes.Method.DevMethod CreateNewEmptyMethod(string name)
        {
            var method = new FakeTypes.Method.DevMethod(DevClass, name, false, Program.Project.GetVoidType(), Core.Visibility.Public);

            DevClass.Methods.Add(method);

            return method;
        }


        #endregion
    }
}
