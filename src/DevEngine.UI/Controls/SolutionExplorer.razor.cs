using DevEngine.Core.Project;
using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class SolutionExplorer : ComponentBase
    {
        public enum TreeViewItemType
        {
            Folder, Class
        }

        private class SolutionExplorerTreeViewItem
        {
            /// <summary>
            /// Only ever null while it's being created / before it's named the first time
            /// </summary>
            public string? Name { get; set; }

            public TreeViewItemType Type { get; set; }

            public List<SolutionExplorerTreeViewItem> Children { get; } = new List<SolutionExplorerTreeViewItem>();
        }

        private List<SolutionExplorerTreeViewItem> Items { get; } = new List<SolutionExplorerTreeViewItem>();

        private SolutionExplorerTreeViewItem? SelectedTreeViewItem { get; set; }

        [CascadingParameter]
        public RightClickController? RightClickController { get; set; }

        private void OnSelectedTreeViewItemChanged(SolutionExplorerTreeViewItem item)
        {
            SelectedTreeViewItem = item;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                InitializeTreeView();


                StateHasChanged();
            }
        }

        #region Right click menus

        #region OnCreateFolderClicked

        private void OnCreateFolderClicked()
        {
            SelectedTreeViewItem?.Children.Add(new SolutionExplorerTreeViewItem()
            {
                Name = "new folder",
                Type = TreeViewItemType.Folder
            });

            StateHasChanged();
        }

        #endregion

        #region OnDeleteFolderClicked

        private void OnDeleteFolderClicked()
        {

        }

        #endregion

        #region OnCreateClassClicked

        private void OnCreateClassClicked()
        {

        }

        #endregion

        #region OnRenameClassClicked

        private void OnRenameClassClicked()
        {

        }

        #endregion

        #endregion

        #region OnClick

        private void OnSolutionExplorerClick(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == 2 && SelectedTreeViewItem != null)
            {
                List<RightClickController.MenuItem> items;
                if (SelectedTreeViewItem.Type == TreeViewItemType.Folder)
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("Create folder", OnCreateFolderClicked),
                        new RightClickController.MenuItem("Create class", OnCreateClassClicked),
                    };
                    if (SelectedTreeViewItem.Children.Count == 0 && SelectedTreeViewItem != Items[0])
                        items.Add(new RightClickController.MenuItem("Delete folder", OnDeleteFolderClicked));
                }
                else
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("Rename", OnRenameClassClicked)
                    };
                }

                RightClickController?.DisplayRightClickMenu(mouseEventArgs, items);
            }
        }

        #endregion

        #region InitializeTreeView

        private void InitializeTreeView()
        {

            Items.Add(new SolutionExplorerTreeViewItem()
            {
                Name = "/",
                Type = TreeViewItemType.Folder
            });

            foreach (var classToAdd in Program.Project.Classes)
            {
                var treeViewItem = GetTreeViewItemFromFolder(classToAdd.Value.Folder);

                if (treeViewItem != null)
                {
                    treeViewItem.Children.Add(new SolutionExplorerTreeViewItem()
                    {
                        Name = classToAdd.Key.Name,
                        Type = TreeViewItemType.Class
                    });
                }
            }
        }

        #endregion

        #region GetTreeViewItemFromFolder

        private SolutionExplorerTreeViewItem? GetTreeViewItemFromFolder(string folder, bool create = true)
        {
            var currentFolder = Items[0]; // we assume the top one always exist

            var subFolders = folder.Split("/");

            foreach (var subFolder in subFolders)
            {
                var subFolderItem = currentFolder.Children.FirstOrDefault(x => x.Name == subFolder);
                if (subFolderItem != null)
                    currentFolder = subFolderItem;
                else if (create)
                {
                    currentFolder = new SolutionExplorerTreeViewItem()
                    {
                        Name = subFolder,
                        Type = TreeViewItemType.Folder
                    };
                    currentFolder.Children.Add(currentFolder);
                }
                else
                    return null;
            }

            return currentFolder;

        }

        #endregion

        #region GetTreeViewItemParent



        #endregion
    }

}
